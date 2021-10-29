using System.IO;

namespace Explorip.Helpers
{
    public static class ExtensionsFichiers
    {
        public static bool IsShortcut(this FileInfo fichier)
        {
            if (Path.GetExtension(fichier.Name).ToLower().Trim() == ".lnk")
                return true;
            else
                return false;
        }
    }
}
