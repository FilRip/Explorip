using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Explorip.Helpers;
using Explorip.Sorters;

namespace Explorip.ComposantsWinForm
{
    public class FichiersListView : FilRipListView.FilRipListView
    {
        private ContextMenuStrip _cms;

        public FichiersListView() : base()
        {
            _cms = new ContextMenuStrip();
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
            LargeImageList.ImageSize = new Size(32, 32);
            SmallImageList.ImageSize = new Size(16, 16);
            this.MouseUp += new MouseEventHandler(FichiersListView_MouseUp);
        }

        public ContextMenuStrip MenuContextuel
        {
            get { return _cms; }
            set { _cms = value; }
        }

        public void Rafraichir(DirectoryInfo dirInfo)
        {
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
                    // TODO : Gérer répertoire raccourcis (link)
                    LargeImageList.Images.Add(sousDirInfo.Name, Icones.GetFileIcon(sousDirInfo.FullName, false, true, true, false));
                    SmallImageList.Images.Add(sousDirInfo.Name, Icones.GetFileIcon(sousDirInfo.FullName, false, true, true, true));
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
                LargeImageList.Images.Add(file.Name, Icones.GetFileIcon(file.FullName, file.IsShortcut(), false, true, false));
                SmallImageList.Images.Add(file.Name, Icones.GetFileIcon(file.FullName, file.IsShortcut(), false, true, true));
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
            }
        }
    }
}
