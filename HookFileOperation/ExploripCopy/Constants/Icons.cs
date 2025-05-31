using System.Windows.Media;
using System.Windows.Media.Imaging;

using ExploripCopy.Helpers;

namespace ExploripCopy.Constants;

public static class Icons
{
    public static BitmapSource MainIconSource { get; private set; }
    public static ImageSource CancelImage { get; private set; }
    public static ImageSource OkImage { get; private set; }
    public static ImageSource Folder { get; private set; }
    public static ImageSource Shortcut { get; private set; }

    public static void LoadIcons()
    {
        MainIconSource = IconManager.Convert(IconManager.Extract("shell32.dll", 249, true));
        CancelImage = IconManager.Convert(IconManager.Extract("imageres.dll", 100, false));
        OkImage = IconManager.Convert(IconManager.Extract("imageres.dll", 101, false));
        Folder = IconManager.Convert(IconManager.Extract("shell32.dll", 4, true));
        Shortcut = IconManager.Convert(IconManager.Extract("shell32.dll", 146, true));
    }
}
