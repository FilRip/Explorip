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
    }
}
