using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using Explorip.ExploripEventArgs;
using Explorip.Helpers;
using Explorip.Sorters;
using Explorip.WinAPI.Modeles;

namespace Explorip.ComposantsWinForm
{
    public class FichiersListView : FilRipListView.FilRipListView, IDropSource
    {
        private ContextMenuStrip _cms;
        private DirectoryInfo _repCourant;
        public delegate void DelegateEntrerRepertoire(object sender, SelectionneRepertoireEventArgs e);
        public event DelegateEntrerRepertoire SelectionneRepertoire;
        private DirectoryTreeView _liensRepertoire;
        private FilesOperations.FileOperation _fileOperation = new FilesOperations.FileOperation();
        private FileSystemWatcher _repCourantChangement;
        private Dictionary<int, string> _listeIcones;

        public FichiersListView() : base()
        {
            InitializeComponent();
            _cms = new ContextMenuStrip();
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
            LargeImageList.ImageSize = new Size(32, 32);
            SmallImageList.ImageSize = new Size(16, 16);
            _listeIcones = new Dictionary<int, string>();
            this.MouseUp += FichiersListView_MouseUp;
            MouseDoubleClick += FichiersListView_MouseDoubleClick;
            InitialiseSurveillance();
        }

        private void InitialiseSurveillance()
        {
            _repCourantChangement = new FileSystemWatcher();
            _repCourantChangement.Changed += ChangementDansRepCourant;
            _repCourantChangement.Created += ChangementDansRepCourant;
            _repCourantChangement.Deleted += ChangementDansRepCourant;
            _repCourantChangement.Renamed += ChangementDansRepCourant_Renamed;
            _repCourantChangement.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _repCourantChangement.Error += ErreurChangementDansRepCourant;
        }

        private void ChangementDansRepCourant_Renamed(object sender, RenamedEventArgs e)
        {
            Rafraichir(_repCourant);
        }

        private void ErreurChangementDansRepCourant(object sender, ErrorEventArgs e)
        {
            InitialiseSurveillance();
        }

        private void ChangementDansRepCourant(object sender, FileSystemEventArgs e)
        {
            Rafraichir(_repCourant);
        }

        private void FichiersListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                ListViewItem item = SelectedItems[0];
                if (item.Tag is FileInfo)
                {
                    FileInfo fileInfo = (FileInfo)item.Tag;
                    ManagedShell.Common.Helpers.ShellHelper.ExecuteProcess(fileInfo.FullName);
                }
                else if (item.Tag is DirectoryInfo)
                {
                    DirectoryInfo directoryInfo = (DirectoryInfo)item.Tag;
                    if (SelectionneRepertoire != null)
                        SelectionneRepertoire.BeginInvoke(this, new SelectionneRepertoireEventArgs(directoryInfo), null, null);
                    if (_liensRepertoire != null)
                    {
                        _liensRepertoire.RafraichirRepertoire(directoryInfo);
                    }
                    else
                    {
                        Rafraichir(directoryInfo);
                    }
                }
            }
        }

        public DirectoryTreeView LiensRepertoires
        {
            get { return _liensRepertoire; }
            set { _liensRepertoire = value; }
        }

        public ContextMenuStrip MenuContextuel
        {
            get { return _cms; }
            set { _cms = value; }
        }

        public DirectoryInfo RepertoireCourant
        {
            get { return _repCourant; }
        }

        public void Initialise(TreeNode noeud)
        {
            VideListe();
            if (noeud != null)
            {
                foreach (TreeNode noeudEnfant in noeud.Nodes)
                {
                    AjouterElement(noeudEnfant.Text, (FileSystemInfo)noeudEnfant.Tag);
                }
            }
        }

        private delegate void DelegateAjouteItem(ListViewItem item);
        private void AjouteItem(ListViewItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateAjouteItem(AjouteItem), item);
                return;
            }
            Items.Add(item);
        }

        private void AjouterElement(string nom, FileSystemInfo element)
        {
            IntPtr pidl;
            string nomIcone = nom;
            int numIcone = Icones.GetNumIcon(element.FullName, element.IsShortcut(), true, out pidl);
            if (_listeIcones.ContainsKey(numIcone))
                nomIcone = _listeIcones[numIcone];
            else
            {
                _listeIcones.Add(numIcone, nom);
                Bitmap petiteIcone, largeIcone;
                petiteIcone = Icones.GetFileIcon(pidl, numIcone, WinAPI.Shell32.SHIL.SMALL);
                largeIcone = Icones.GetFileIcon(pidl, numIcone, WinAPI.Shell32.SHIL.LARGE);
                if (largeIcone != null)
                    LargeImageList.Images.Add(nomIcone, largeIcone);
                if (petiteIcone != null)
                    SmallImageList.Images.Add(nomIcone, petiteIcone);
            }

            Task maTache = new Task(() => { AjouteItem(new ListViewItem(nom, nomIcone) { Tag = element }); });
            maTache.Start();
        }

        private void VideListe()
        {
            Items.Clear();
            _listeIcones.Clear();
            if (LargeImageList?.Images != null)
                foreach (Bitmap img in LargeImageList.Images)
                    img.Dispose();
            if (SmallImageList?.Images != null)
                foreach (Bitmap img in SmallImageList.Images)
                    img.Dispose();
            if (LargeImageList != null)
                LargeImageList.Images.Clear();
            if (SmallImageList != null)
                SmallImageList.Images.Clear();
        }

        private delegate void InvokeRafraichir(DirectoryInfo dirInfo);
        public void Rafraichir(DirectoryInfo dirInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new InvokeRafraichir(Rafraichir), dirInfo);
                return;
            }

            Stopwatch _chronometre = new Stopwatch();
            _chronometre.Restart();

            _repCourant = dirInfo;
            _repCourantChangement.EnableRaisingEvents = false;
            _repCourantChangement.Path = dirInfo.FullName;

            // TODO : Utiliser les tuiles https://github.com/dbarros/WindowsAPICodePack
            VideListe();

            DirectoryInfo[] dirs = null;
            try
            {
                dirs = dirInfo.GetDirectories();
            }
            catch (Exception) { }
            if (dirs != null)
            {
                Array.Sort(dirs, new TriAlphabetique());
                foreach (DirectoryInfo sousDirInfo in dirs)
                {
                    AjouterElement(sousDirInfo.Name, sousDirInfo);
                }
            }

            FileInfo[] files = null;
            try
            {
                files = dirInfo.GetFiles();
            }
            catch (Exception) { }
            if (files != null)
            {
                Array.Sort(files, new TriAlphabetique());
                foreach (FileInfo file in files)
                {
                    AjouterElement(file.Name, file);
                }
            }

            _chronometre.Stop();
            Console.WriteLine("Temps d'exécution : " + _chronometre.ElapsedMilliseconds.ToString() + " millisecondes");
            _repCourantChangement.EnableRaisingEvents = true;
        }

        private void FichiersListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SelectedItems.Count > 0)
                {
                    List<FileInfo> arrFI = new List<FileInfo>();
                    List<DirectoryInfo> arrDI = new List<DirectoryInfo>();
                    foreach (ListViewItem item in SelectedItems)
                    {
                        if (item.Tag.GetType() == typeof(FileInfo))
                            arrFI.Add((FileInfo)item.Tag);
                        else if (item.Tag.GetType() == typeof(DirectoryInfo))
                            arrDI.Add((DirectoryInfo)item.Tag);
                    }
                    // TODO : Menu contextuel quand on a sélectionné des fichiers ET des dossiers
                    if (arrFI.Count > 0)
                    {
                        ShellContextMenu ctxMnu = new ShellContextMenu();
                        ctxMnu.ShowContextMenu(arrFI.ToArray(), PointToScreen(new Point(e.X, e.Y)), _cms);
                    }
                    else if (arrDI.Count > 0)
                    {
                        ShellContextMenu ctxMnu = new ShellContextMenu();
                        ctxMnu.ShowContextMenu(arrDI.ToArray(), PointToScreen(new Point(e.X, e.Y)), _cms);
                    }
                }
                else
                {
                    FilesOperations.ContextMenuPaste.RepDestination = _repCourant.FullName;
                    ShellContextMenu ctxMenu = new ShellContextMenu();
                    ctxMenu.ShowContextMenu(_repCourant, PointToScreen(new Point(e.X, e.Y)), _cms);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FichiersListView
            // 
            this.AllowDrop = true;
            this.DragDrop += new DragEventHandler(this.FichiersListView_DragDrop);
            this.DragEnter += new DragEventHandler(this.FichiersListView_DragEnter);
            this.DragLeave += new EventHandler(this.FichiersListView_DragLeave);
            this.DragOver += new DragEventHandler(this.FichiersListView_DragOver);
            this.ItemDrag += new ItemDragEventHandler(this.FichiersListView_ItemDrag);
            this.KeyUp += new KeyEventHandler(this.FichiersListView_KeyUp);
            this.ResumeLayout(false);

        }

        private void FichiersListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Rafraichir(_repCourant);
            }
            else if (e.KeyCode == Keys.Back)
            {
                _repCourant = Directory.GetParent(_repCourant.FullName);
                if (SelectionneRepertoire != null)
                    SelectionneRepertoire.BeginInvoke(this, new SelectionneRepertoireEventArgs(_repCourant), null, null);
                if (_liensRepertoire != null)
                {
                    _liensRepertoire.RafraichirRepertoire(_repCourant);
                }
                else
                {
                    Rafraichir(_repCourant);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (SelectedItems.Count > 0)
                {
                    foreach (FileSystemInfo item in RetourneListeFichiersDossiersSelectionnes())
                    {
                        _fileOperation.DeleteItem(item.FullName);
                    }
                    _fileOperation.PerformOperations();
                }
            }
            else if (e.Modifiers.HasFlag(Keys.Control))
            {
                if ((e.KeyCode == Keys.C) || (e.KeyCode == Keys.X))
                {
                    // Couper/copier
                    bool deplace = (e.KeyCode == Keys.X);
                    if (SelectedItems.Count > 0)
                    {
                        List<FileSystemInfo> listeFichiersDossiers = RetourneListeFichiersDossiersSelectionnes();
                        ExtensionsClipboard.AjouterFichiersDossiers(listeFichiersDossiers, deplace);
                    }
                }
                if (e.KeyCode == Keys.V)
                {
                    // Coller
                    if (ExtensionsClipboard.LireFichiersDossiers(true, out List<FileSystemInfo> listeFichiersDossiers, out bool deplace))
                    {
                        foreach (FileSystemInfo fileSystemInfo in listeFichiersDossiers)
                        {
                            if (deplace)
                                _fileOperation.MoveItem(fileSystemInfo.FullName, _repCourant.FullName, fileSystemInfo.Name);
                            else
                                _fileOperation.CopyItem(fileSystemInfo.FullName, _repCourant.FullName, fileSystemInfo.Name);
                        }
                        _fileOperation.PerformOperations();
                    }
                }
            }
        }

        private List<FileSystemInfo> RetourneListeFichiersDossiersSelectionnes()
        {
            List<FileSystemInfo> retour = new List<FileSystemInfo>();
            if (SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in SelectedItems)
                {
                    retour.Add((FileSystemInfo)item.Tag);
                }
            }
            return retour;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_repCourantChangement != null)
            {
                _repCourantChangement.Changed -= ChangementDansRepCourant;
                _repCourantChangement.Created -= ChangementDansRepCourant;
                _repCourantChangement.Deleted -= ChangementDansRepCourant;
                _repCourantChangement.Renamed -= ChangementDansRepCourant_Renamed;
                _repCourantChangement.Dispose();
            }
            if (_fileOperation != null)
            {
                try
                {
                    // TODO : Verifier que cela ne coupe pas les opérations déjà en cours
                    _fileOperation.Dispose();
                }
                catch (Exception) { }
            }
            if ((_cms != null) && (!_cms.IsDisposed))
            {
                _cms.Dispose();
            }
            _fileOperation = null;
            _liensRepertoire = null;
            _repCourant = null;
            VideListe();
        }

        #region Drag'n Drop

        // TODO : Implémenter couper/copier/coller par drag & drop
        // docs : https://stackoverflow.com/questions/28139791/pass-drop-event-on-to-a-folder-using-c

        private MouseButtons startButton;

        private void FichiersListView_DragLeave(object sender, EventArgs e)
        {

        }

        private void FichiersListView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
            {

            }
            else
            {

            }
        }

        private void FichiersListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropHelper.effetDragDrop;
            }
        }

        private void FichiersListView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void FichiersListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            IntPtr pidlParent;
            IShellFolder shellFolder;
            int erreur;

            // TODO : Penser a libérer les espaces mémoires/pointeurs réservés par cette méthode

            startButton = e.Button;

            if (_repCourant == null)
            {
                erreur = WinAPI.Shell32.SHGetDesktopFolder(out pidlParent);
                if (erreur == (int)WinAPI.Commun.HRESULT.S_OK)
                    return;
                shellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidlParent, typeof(IShellFolder));
            }
            else
            {
                Guid guidSH = typeof(IShellFolder).GUID;
                pidlParent = WinAPI.Shell32.ILCreateFromPath(_repCourant.FullName);
                IntPtr pidlInterface;
                WinAPI.Shell32.SHBindToParent(pidlParent, ref guidSH, out pidlInterface, IntPtr.Zero);
                shellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidlInterface, typeof(IShellFolder));
            }
            if (pidlParent != IntPtr.Zero)
            {
                Guid guid = new Guid("{0000010e-0000-0000-C000-000000000046}");
                IntPtr[] items = new IntPtr[SelectedItems.Count];
                int position = 0;
                foreach (ListViewItem item in SelectedItems)
                    items[position++] = WinAPI.Shell32.ILCreateFromPath(((FileSystemInfo)item.Tag).FullName);
                erreur = shellFolder.GetUIObjectOf(
                    IntPtr.Zero,
                    (uint)SelectedItems.Count,
                    items,
                    ref guid,
                    IntPtr.Zero,
                    out IntPtr data);
                if (erreur == (int)WinAPI.Commun.HRESULT.S_OK)
                {
                    WinAPI.Ole32.DoDragDrop(data, this, DragDropHelper.effetDragDrop, out DragDropEffects effets);
                }
            }
        }

        #region Interface IDropSource

        int IDropSource.QueryContinueDrag(bool fEscapePressed, MK grfKeyState)
        {
            if (fEscapePressed)
                return WinAPI.Commun.DRAGDROP_S_CANCEL;
            else
            {
                if ((startButton & MouseButtons.Left) != 0 && (grfKeyState & MK.LBUTTON) == 0)
                    return WinAPI.Commun.DRAGDROP_S_DROP;
                else if ((startButton & MouseButtons.Right) != 0 && (grfKeyState & MK.RBUTTON) == 0)
                    return WinAPI.Commun.DRAGDROP_S_DROP;
                else
                    return (int)WinAPI.Commun.HRESULT.S_OK;
            }
        }

        int IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            return WinAPI.Commun.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        #endregion

        #endregion
    }
}
