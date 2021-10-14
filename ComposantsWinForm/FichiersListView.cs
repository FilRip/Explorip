using System;
using System.IO;
using System.Windows.Forms;

using Explorip.Helpers;
using Explorip.Sorters;

namespace Explorip.ComposantsWinForm
{
    public class FichiersListView : FilRipListView.FilRipListView
    {
        public FichiersListView() : base()
        {
            LargeImageList = new ImageList();
            SmallImageList = new ImageList();
        }

        public void Rafraichir(DirectoryInfo dirInfo)
        {
            Items.Clear();
            FileInfo[] files = dirInfo.GetFiles();
            Array.Sort(files, new TriAlphabetique());
            foreach (FileInfo file in files)
            {
                LargeImageList.Images.Add(Icones.GetFileIcon(file.FullName, file.IsShortcut(), false, true));
                SmallImageList.Images.Add(Icones.GetFileIcon(file.FullName, file.IsShortcut(), false, true));
                Items.Add(new ListViewItem(file.Name, LargeImageList.Images.Count - 1));
                Items[Items.Count - 1].Tag = file.FullName;
            }
        }
    }
}
