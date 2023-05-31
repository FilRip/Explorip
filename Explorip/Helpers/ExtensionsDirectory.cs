using System;
using System.IO;

namespace Explorip.Helpers
{
    public static class ExtensionsDirectory
    {
        public static bool IsShortcut(this FileSystemInfo repertoireOuFichier)
        {
            // TODO : Recognize symbolic directory shortcut
            if (repertoireOuFichier is FileInfo)
            {
                return (Path.GetExtension(repertoireOuFichier.Name).ToLower().Trim() == ".lnk");
            }
            else
            {
                return (repertoireOuFichier.Attributes.HasFlag(FileAttributes.ReparsePoint));
            }
        }

        public static string FullPath(this Environment.SpecialFolder specialFolder)
        {
            return Environment.GetFolderPath(specialFolder);
        }
    }
}
