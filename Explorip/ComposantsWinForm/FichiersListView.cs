﻿using System;
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
    public class FichiersListView : FilRipListView.FilRipListView
    {
        private ContextMenuStrip _cms;
        private DirectoryInfo _repCourant;
        public delegate void DelegateEntrerRepertoire(object sender, SelectionneRepertoireEventArgs e);
        public event DelegateEntrerRepertoire SelectionneRepertoire;
        private DirectoryTreeView _liensRepertoire;
        private FilesOperations.FileOperation _fileOperation = new FilesOperations.FileOperation();
        private FileSystemWatcher _repCourantChangement;
        private readonly Dictionary<int, string> _listeIcones;
        private readonly DragDropHelper _dragDropHelper;

        public FichiersListView() : base()
        {
            InitializeComponent();
            _cms = new ContextMenuStrip();
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
            LargeImageList.ImageSize = new Size(32, 32);
            SmallImageList.ImageSize = new Size(16, 16);
            _listeIcones = new Dictionary<int, string>();
            MouseDoubleClick += FichiersListView_MouseDoubleClick;
            _dragDropHelper = new DragDropHelper();
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
            string nomIcone = nom;
            int numIcone = Icones.GetNumIcon(element.FullName, element.IsShortcut(), true, out IntPtr pidl);
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
            if ((e.Button == MouseButtons.Right) && (!_dragDropHelper.DragDropEnCours))
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
            this.MouseUp += new MouseEventHandler(this.FichiersListView_MouseUp);
            this.DragDrop += new DragEventHandler(this.FichiersListView_DragDrop);
            this.DragEnter += new DragEventHandler(this.FichiersListView_DragEnter);
            this.DragOver += new DragEventHandler(this.FichiersListView_DragOver);
            this.DragLeave += new EventHandler(this.FichiersListView_DragLeave);
            this.ItemDrag += new ItemDragEventHandler(this.FichiersListView_ItemDrag);
            this.KeyUp += new KeyEventHandler(this.FichiersListView_KeyUp);
            this.ResumeLayout(false);

        }

        private void FichiersListView_DragLeave(object sender, EventArgs e)
        {
            Console.WriteLine("DragLeave");
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

        private void FichiersListView_DragDrop(object sender, DragEventArgs e)
        {
            if (_dragDropHelper.StartButton == MouseButtons.Left)
            {
                if (e.Effect == DragDropEffects.Copy)
                {
                    foreach (FileSystemInfo item in _dragDropHelper.ListeFichiersDossiers)
                        _fileOperation.CopyItem(item.FullName, _repCourant.FullName, item.Name);
                    _fileOperation.PerformOperations();
                }
                else if (e.Effect == DragDropEffects.Move)
                {
                    foreach (FileSystemInfo item in _dragDropHelper.ListeFichiersDossiers)
                        _fileOperation.MoveItem(item.FullName, _repCourant.FullName, item.Name);
                    _fileOperation.PerformOperations();
                }
                else
                {
                    // TODO : Creer des raccourcis
                }
            }
            else
            {
                _dragDropHelper.DragDrop(sender, e);
            }
            _dragDropHelper.DragDropEnCours = false;
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
            }
        }

        private void FichiersListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _dragDropHelper.DemarreDrag(RetourneListeFichiersDossiersSelectionnes(), e.Button, _repCourant);
        }

        #endregion
    }
}
