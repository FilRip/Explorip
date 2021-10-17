using System.IO;

namespace Explorip.Helpers
{
    public static class ExtensionsDossiers
    {
        public static bool IsShortcut(this DirectoryInfo repertoire)
        {
            // TODO : Gérer répertoire raccourcis (link)
            if (Path.GetExtension(repertoire.Name).ToLower().Trim() == ".lnk")
                return true;
            else
                return false;
        }

        public static string Repertoire(this System.Environment.SpecialFolder specialFolder)
        {
            return System.Environment.GetFolderPath(specialFolder);
        }
    }
}
