using System;
using System.IO;
using System.Windows.Forms;

using Explorip.Localization;
using Explorip.WinAPI;

namespace Explorip.Helpers
{
    public static class ExtensionsDossiers
    {
        public static bool IsShortcut(this FileSystemInfo repertoireOuFichier)
        {
            if (repertoireOuFichier is FileInfo)
            {
                return (Path.GetExtension(repertoireOuFichier.Name).ToLower().Trim() == ".lnk");
            }
            else
            {
                return (repertoireOuFichier.Attributes.HasFlag(FileAttributes.ReparsePoint));
            }
        }

        public static string Repertoire(this Environment.SpecialFolder specialFolder)
        {
            return Environment.GetFolderPath(specialFolder);
        }

        public static IntPtr GetPIDL(this Environment.SpecialFolder specialFolder)
        {
            IntPtr pidl = IntPtr.Zero;
            Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, (Shell32.CSIDL)Enum.Parse(typeof(Shell32.CSIDL), ((int)specialFolder).ToString()), ref pidl);
            return pidl;
        }

        public static TreeNode GetTreeNode(this Environment.SpecialFolder specialFolder, ImageList listeIcones, bool petiteIcone = true)
        {
            try
            {
                IntPtr tempPidl = specialFolder.GetPIDL();
                listeIcones.Images.Add(Icones.GetIcone(tempPidl, petiteIcone));
                TreeNode retour;
                retour = new TreeNode()
                {
                    Name = specialFolder.ToString("G"),
                    Tag = new DirectoryInfo(specialFolder.Repertoire()),
                    ImageIndex = listeIcones.Images.Count - 1,
                    SelectedImageIndex = listeIcones.Images.Count - 1,
                    Text = specialFolder.NomTraduit()
                };
                retour.Nodes.Add("");
                return retour;
            }
            catch (Exception) { }
            return null;
        }
    }
}
