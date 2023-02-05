using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.Localization;
using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

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

        public static IntPtr GetPIDL(this FileSystemInfo fileSystemInfo)
        {
            return Shell32.ILCreateFromPath(fileSystemInfo.FullName);
        }

        public static IntPtr GetPIDL(this Environment.SpecialFolder specialFolder)
        {
            IntPtr pidl = IntPtr.Zero;
            Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, (Shell32.CSIDL)Enum.Parse(typeof(Shell32.CSIDL), ((int)specialFolder).ToString()), ref pidl);
            return pidl;
        }

        public static IShellFolder GetDesktopFolder()
        {
            if (Shell32.SHGetDesktopFolder(out IntPtr pointeurDesktop) != (int)Commun.HRESULT.S_OK)
                return null;
            return (IShellFolder)Marshal.GetTypedObjectForIUnknown(pointeurDesktop, typeof(IShellFolder));
        }

        public static IShellFolder GetShellFolder(this DirectoryInfo directoryInfo)
        {
            IShellFolder sfd = GetDesktopFolder();
            uint pchEaten = 0;
            SFGAO pdwAttributes = 0;
            sfd.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, directoryInfo.FullName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            Guid guid = typeof(IShellFolder).GUID;
            sfd.BindToObject(pPIDL, IntPtr.Zero, ref guid, out object pUnknownParentFolder);
            return (IShellFolder)pUnknownParentFolder;
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
            catch (Exception) { /* Ignore errors */ }
            return null;
        }
    }
}
