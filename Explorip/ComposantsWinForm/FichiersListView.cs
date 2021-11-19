using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Explorip.ExploripEventArgs;
using Explorip.Helpers;
using Explorip.Sorters;

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

        public FichiersListView() : base()
        {
            InitializeComponent();
            _cms = new ContextMenuStrip();
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
            LargeImageList.ImageSize = new Size(32, 32);
            SmallImageList.ImageSize = new Size(16, 16);
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
                if (item.Tag.GetType() == typeof(FileInfo))
                {
                    FileInfo fileInfo = (FileInfo)item.Tag;
                    ManagedShell.Common.Helpers.ShellHelper.ExecuteProcess(fileInfo.FullName);
                }
                else if (item.Tag.GetType() == typeof(DirectoryInfo))
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

        private void AjouterElement(string nom, FileSystemInfo element)
        {
            if (element.GetType() == typeof(DirectoryInfo))
            {
                DirectoryInfo sousDirInfo = (DirectoryInfo)element;
                LargeImageList.Images.Add(nom, Icones.GetIcone(sousDirInfo.FullName, sousDirInfo.IsShortcut(), true, false));
                SmallImageList.Images.Add(nom, Icones.GetIcone(sousDirInfo.FullName, sousDirInfo.IsShortcut(), true, true));
                Items.Add(new ListViewItem(nom, nom) { Tag = sousDirInfo });
            }
            else if (element.GetType() == typeof(FileInfo))
            {
                FileInfo file = (FileInfo)element;
                LargeImageList.Images.Add(nom, Icones.GetIcone(file.FullName, file.IsShortcut(), true, false));
                SmallImageList.Images.Add(nom, Icones.GetIcone(file.FullName, file.IsShortcut(), true, true));
                Items.Add(new ListViewItem(nom, nom) { Tag = file });
            }
        }

        private void VideListe()
        {
            Items.Clear();
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

            FileInfo[] files;
            try
            {
                files = dirInfo.GetFiles();
            }
            catch (Exception) { return; }
            if (files != null)
            {
                Array.Sort(files, new TriAlphabetique());
                foreach (FileInfo file in files)
                {
                    AjouterElement(file.Name, file);
                }
            }

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
                    foreach (ListViewItem item in SelectedItems)
                    {
                        _fileOperation.DeleteItem(((FileSystemInfo)item.Tag).FullName);
                    }
                    _fileOperation.PerformOperations();
                }
            }
            else if (e.Modifiers.HasFlag(Keys.Control))
            {
                // TODO : Couper/copier/coller raccourcis
                // docs pour support entre applications : https://stackoverflow.com/questions/2077981/cut-files-to-clipboard-in-c-sharp
                if ((e.KeyCode == Keys.C) || (e.KeyCode == Keys.X))
                {
                    if (SelectedItems.Count > 0)
                    {
                    }
                }
            }
        }

        // TODO : Implémenter couper/copier/coller par drag & drop
        // docs : https://stackoverflow.com/questions/28139791/pass-drop-event-on-to-a-folder-using-c

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
    }
}
