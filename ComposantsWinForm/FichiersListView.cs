using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public FichiersListView() : base()
        {
            _cms = new ContextMenuStrip();
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
            LargeImageList.ImageSize = new Size(32, 32);
            SmallImageList.ImageSize = new Size(16, 16);
            this.MouseUp += FichiersListView_MouseUp;
            MouseDoubleClick += FichiersListView_MouseDoubleClick;
        }

        private void FichiersListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                ListViewItem item = SelectedItems[0];
                if (item.Tag.GetType() == typeof(FileInfo))
                {
                    FileInfo fileInfo = (FileInfo)item.Tag;
                    Process.Start(fileInfo.FullName);
                }
                else if (item.Tag.GetType() == typeof(DirectoryInfo))
                {
                    DirectoryInfo directoryInfo = (DirectoryInfo)item.Tag;
                    if (SelectionneRepertoire != null)
                        SelectionneRepertoire.BeginInvoke(this, new SelectionneRepertoireEventArgs(directoryInfo), null, null);
                    _liensRepertoire.RafraichirRepertoire(directoryInfo);
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

        public void Rafraichir(DirectoryInfo dirInfo)
        {
            _repCourant = dirInfo;
            // TODO : Utiliser les tuiles https://github.com/dbarros/WindowsAPICodePack
            Items.Clear();
            LargeImageList.Images.Clear();
            SmallImageList.Images.Clear();

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
                    LargeImageList.Images.Add(sousDirInfo.Name, Icones.GetIcone(sousDirInfo.FullName, sousDirInfo.IsShortcut(), true, true, false));
                    SmallImageList.Images.Add(sousDirInfo.Name, Icones.GetIcone(sousDirInfo.FullName, sousDirInfo.IsShortcut(), true, true, true));
                    Items.Add(new ListViewItem(sousDirInfo.Name, sousDirInfo.Name) { Tag = sousDirInfo });
                }
            }

            FileInfo[] files;
            try
            {
                files = dirInfo.GetFiles();
            }
            catch (Exception) { return; }
            Array.Sort(files, new TriAlphabetique());
            foreach (FileInfo file in files)
            {
                LargeImageList.Images.Add(file.Name, Icones.GetIcone(file.FullName, file.IsShortcut(), false, true, false));
                SmallImageList.Images.Add(file.Name, Icones.GetIcone(file.FullName, file.IsShortcut(), false, true, true));
                Items.Add(new ListViewItem(file.Name, file.Name) { Tag = file });
            }
        }

        private void FichiersListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SelectedItems.Count > 0)
                {
                    List<FileInfo> arrFI = new List<FileInfo>();
                    foreach (ListViewItem item in SelectedItems)
                    {
                        if (item.Tag.GetType() == typeof(FileInfo))
                            arrFI.Add((FileInfo)item.Tag);
                    }
                    if (arrFI.Count > 0)
                    {
                        ShellContextMenu ctxMnu = new ShellContextMenu();
                        ctxMnu.ShowContextMenu(arrFI.ToArray(), PointToScreen(new Point(e.X, e.Y)), _cms);
                    }
                }
                else
                {
                    // TODO : Nouveau fichier/dossier : https://social.msdn.microsoft.com/Forums/vstudio/en-US/5732ce0a-29d8-4e73-ae25-5789e20e9c24/display-file-explorers-new-item-menu-in-a-barsubitem?forum=csharpgeneral
                    // TODO : Clique droit dans une zone vide
                }
            }
        }
    }
}
