using System.Windows.Media.Imaging;

using ExploripCopy.Helpers;

namespace ExploripCopy.Constants
{
    public static class Icons
    {
        public static BitmapSource MainIconSource { get; private set; }

        public static void LoadIcons()
        {
            MainIconSource = IconManager.Convert(IconManager.Extract("shell32.dll", 249, true));
        }
    }
}
