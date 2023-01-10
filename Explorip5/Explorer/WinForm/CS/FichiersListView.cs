using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using Explorip.ExploripEventArgs;
using Explorip.Helpers;
using Explorip.Sorters;

using ManagedShell.ShellFolders;

using ShellContextMenu = Explorip.Helpers.ShellContextMenu;

namespace Explorip.ComposantsWinForm
{
    public class FichiersListView : FilRipListView.FilRipListView
    {
        private DirectoryInfo _repCourant;
        public delegate void DelegateEntrerRepertoire(object sender, SelectionneRepertoireEventArgs e);
        public event DelegateEntrerRepertoire SelectionneRepertoire;
        private FilesOperations.FileOperation _fileOperation = new();
        private FileSystemWatcher _repCourantChangement;

        public FichiersListView() : base()
        {
            InitializeComponent();
            MenuContextuel = new ContextMenuStrip();
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
            LargeImageList.ImageSize = new Size(32, 32);
            SmallImageList.ImageSize = new Size(16, 16);
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
                if (item.Tag is FileInfo fileInfo)
                {
                    ManagedShell.Common.Helpers.ShellHelper.ExecuteProcess(fileInfo.FullName);
                }
                else if (item.Tag is DirectoryInfo directoryInfo)
                {
                    if (SelectionneRepertoire != null)
                        Task.Run(() => SelectionneRepertoire.Invoke(this, new SelectionneRepertoireEventArgs(directoryInfo)));
                    if (LiensRepertoires != null)
                    {
                        LiensRepertoires.RafraichirRepertoire(directoryInfo);
                    }
                    else
                    {
                        Rafraichir(directoryInfo);
                    }
                }
            }
        }

        public DirectoryTreeView LiensRepertoires { get; set; }

        public ContextMenuStrip MenuContextuel { get; set; }

        public DirectoryInfo RepertoireCourant
        {
            get { return _repCourant; }
        }

        public void Initialise(TreeNode noeud)
        {
            VideListe();
            if (noeud != null)
            {
                List<ListViewItem> liste = new();
                foreach (TreeNode noeudEnfant in noeud.Nodes)
                {
                    liste.Add(AjouterElement((FileSystemInfo)noeudEnfant.Tag));
                }
                Items.AddRange(liste.ToArray());
            }
        }

        private ListViewItem AjouterElement(FileSystemInfo element)
        {
            int numIcone = Icones.GetNumIcon(element.FullName, element.IsShortcut(), true, out IntPtr pidl);
            string nomIcone = numIcone.ToString();
            if (!SmallImageList.Images.ContainsKey(nomIcone))
            {
                Bitmap petiteIcone, largeIcone;
                petiteIcone = Icones.GetFileIcon(pidl, numIcone, WinAPI.Shell32.SHIL.SMALL);
                largeIcone = Icones.GetFileIcon(pidl, numIcone, WinAPI.Shell32.SHIL.LARGE);
                if (largeIcone != null)
                    LargeImageList.Images.Add(nomIcone, largeIcone);
                if (petiteIcone != null)
                    SmallImageList.Images.Add(nomIcone, petiteIcone);
            }

            return new ListViewItem(element.Name, nomIcone) { Tag = element };
        }

        private void VideListe()
        {
            Items.Clear();
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

            if (dirInfo == null || !dirInfo.Exists)
                return;

            Stopwatch _chronometre = new();
            _chronometre.Restart();

            _repCourant = dirInfo;
            _repCourantChangement.EnableRaisingEvents = false;
            _repCourantChangement.Path = dirInfo.FullName;

            // TODO : Utiliser les tuiles https://github.com/dbarros/WindowsAPICodePack
            VideListe();
            List<ListViewItem> _liste = new();

            DirectoryInfo[] dirs = null;
            try
            {
                dirs = dirInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(this, "Accès non autorisé", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception) { /* Ignore others errors */ }
            if (dirs != null)
            {
                Array.Sort(dirs, new TriAlphabetique());
                foreach (DirectoryInfo sousDirInfo in dirs)
                {
                    _liste.Add(AjouterElement(sousDirInfo));
                }
            }

            FileInfo[] files = null;
            try
            {
                files = dirInfo.GetFiles();
            }
            catch (Exception) { /* Ignore errors */ }
            if (files != null)
            {
                Array.Sort(files, new TriAlphabetique());
                foreach (FileInfo file in files)
                {
                    _liste.Add(AjouterElement(file));
                }
            }

            Items.AddRange(_liste.ToArray());
            _chronometre.Stop();
            Console.WriteLine("Temps d'exécution : " + _chronometre.ElapsedMilliseconds.ToString() + " millisecondes");
            _repCourantChangement.EnableRaisingEvents = true;
        }

        private void FichiersListView_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (!DragDropHelper.GetInstance().DragDropEnCours))
            {
                if (SelectedItems.Count > 0)
                {
                    List<FileInfo> arrFI = new();
                    List<DirectoryInfo> arrDI = new();
                    foreach (ListViewItem item in SelectedItems)
                    {
                        if (item.Tag is FileInfo fi)
                            arrFI.Add(fi);
                        else if (item.Tag is DirectoryInfo di)
                            arrDI.Add(di);
                    }
                    // TODO : Menu contextuel quand on a sélectionné des fichiers ET des dossiers
                    if (arrFI.Count > 0)
                    {
                        ShellContextMenu ctxMnu = new();
                        ctxMnu.ShowContextMenu(arrFI.ToArray(), PointToScreen(new Point(e.X, e.Y)), MenuContextuel);
                    }
                    else if (arrDI.Count > 0)
                    {
                        ShellContextMenu ctxMnu = new();
                        ctxMnu.ShowContextMenu(arrDI.ToArray(), PointToScreen(new Point(e.X, e.Y)), MenuContextuel);
                    }
                }
                else
                {
                    DefinirRepDest(_repCourant);
                    ShellContextMenu ctxMenu = new();
                    ctxMenu.ShowContextMenu(_repCourant, PointToScreen(new Point(e.X, e.Y)), MenuContextuel);
                }
            }
            else if (DragDropHelper.GetInstance().DragDropEnCours)
            {
                DragDropHelper.GetInstance().DragDropEnCours = false;
            }
        }

        private static void DefinirRepDest(DirectoryInfo directoryInfo)
        {
            FilesOperations.ContextMenuPaste.RepDestination = directoryInfo.FullName;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // FichiersListView
            // 
            AllowDrop = true;
            ItemDrag += new System.Windows.Forms.ItemDragEventHandler(FichiersListView_ItemDrag);
            DragDrop += new System.Windows.Forms.DragEventHandler(FichiersListView_DragDrop);
            DragEnter += new System.Windows.Forms.DragEventHandler(FichiersListView_DragEnter);
            DragOver += new System.Windows.Forms.DragEventHandler(FichiersListView_DragOver);
            DragLeave += new System.EventHandler(FichiersListView_DragLeave);
            KeyUp += new System.Windows.Forms.KeyEventHandler(FichiersListView_KeyUp);
            MouseUp += new System.Windows.Forms.MouseEventHandler(FichiersListView_MouseUp);
            ResumeLayout(false);

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
                    Task.Run(() => SelectionneRepertoire.Invoke(this, new SelectionneRepertoireEventArgs(_repCourant)));
                if (LiensRepertoires != null)
                {
                    LiensRepertoires.RafraichirRepertoire(_repCourant);
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
                if (((e.KeyCode == Keys.C) || (e.KeyCode == Keys.X)) &&
                    SelectedItems.Count > 0)
                {
                    // Couper/copier
                    ExtensionsClipboard.AjouterFichiersDossiers(RetourneListeFichiersDossiersSelectionnes(), e.KeyCode == Keys.X);
                }
                else if (e.KeyCode == Keys.V &&
                    ExtensionsClipboard.LireFichiersDossiers(true, out List<FileSystemInfo> listeFichiersDossiers, out bool deplace))
                {
                    // Coller
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

        private List<FileSystemInfo> RetourneListeFichiersDossiersSelectionnes()
        {
            List<FileSystemInfo> retour = new();
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
            if ((MenuContextuel != null) && (!MenuContextuel.IsDisposed))
            {
                MenuContextuel.Dispose();
            }
            _fileOperation = null;
            LiensRepertoires = null;
            _repCourant = null;
            VideListe();
        }

        #region Drag'n Drop

        public void SelectionChange(bool nouvelEtat)
        {
            LiensRepertoires.SelectionChange = nouvelEtat;
        }

        public ShellItem SelectedItem
        {
            get
            {
                return new ShellItem(((FileSystemInfo)SelectedItems[0].Tag).FullName);
            }
        }

        // TODO : Implémenter couper/copier/coller par drag & drop
        // docs : https://stackoverflow.com/questions/28139791/pass-drop-event-on-to-a-folder-using-c

        private void FichiersListView_DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropHelper.GetInstance().StartButton == MouseButtons.Left)
            {
                bool operationAFaire = false;
                if (e.Effect == DragDropEffects.Copy)
                {
                    foreach (FileSystemInfo item in DragDropHelper.GetInstance().ListeFichiersDossiers)
                        if (Path.GetDirectoryName(item.FullName) != _repCourant.FullName)
                        {
                            _fileOperation.CopyItem(item.FullName, _repCourant.FullName, item.Name);
                            operationAFaire = true;
                        }
                }
                else if (e.Effect == DragDropEffects.Move)
                {
                    foreach (FileSystemInfo item in DragDropHelper.GetInstance().ListeFichiersDossiers)
                        if (Path.GetDirectoryName(item.FullName) != _repCourant.FullName)
                        {
                            _fileOperation.MoveItem(item.FullName, _repCourant.FullName, item.Name);
                            operationAFaire = true;
                        }
                }
                else if (e.Effect == DragDropEffects.Link)
                {
                    // TODO : Creer des raccourcis
                }
                if (operationAFaire)
                    _fileOperation.PerformOperations();
            }
            else if (DragDropHelper.GetInstance().StartButton == MouseButtons.Right)
            {
                DragDropHelper.GetInstance().DragDrop(sender, e);
            }
            DragDropHelper.GetInstance().DragDropEnCours = false;
        }

        private void FichiersListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropHelper.effetDragDrop;
            }
            DragDropHelper.GetInstance().DragEnter(sender, e);
        }

        private void FichiersListView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.KeyState.EtatBit(4)) // Touche Ctrl
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if (e.KeyState.EtatBit(6)) // Touche Alt
                {
                    e.Effect = DragDropEffects.Link;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
                DragDropHelper.GetInstance().DragOver(sender, e);
            }
        }

        private void FichiersListView_DragLeave(object sender, EventArgs e)
        {
            DragDropHelper.GetInstance().DragLeave(sender, e);
            SelectionChange(true);
        }

        private void FichiersListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DragDropHelper.GetInstance().ItemDrag(RetourneListeFichiersDossiersSelectionnes(), e.Button, _repCourant);
        }

        #endregion
    }
}
