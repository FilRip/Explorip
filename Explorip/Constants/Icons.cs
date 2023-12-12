using System.Windows.Media;

using Explorip.Helpers;

namespace Explorip.Constants
{
    internal static class Icons
    {
        public static ImageSource CancelImage { get; set; }
        public static ImageSource OkImage { get; set; }

        internal static void Init()
        {
            CancelImage = IconManager.GetIconFromFile("imageres.dll", 100);
            OkImage = IconManager.GetIconFromFile("imageres.dll", 101);
        }
    }
}
