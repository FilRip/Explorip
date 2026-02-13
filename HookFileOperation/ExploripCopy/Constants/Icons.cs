using System.Windows.Media;
using System.Windows.Media.Imaging;

using ExploripCopy.Helpers;

namespace ExploripCopy.Constants;

public static class Icons
{
    private const string Shell32Dll = "shell32.dll";
    private const string ImageResDll = "imageres.dll";

    public static BitmapSource MainIconSource { get; private set; }
    public static ImageSource CancelImage { get; private set; }
    public static ImageSource OkImage { get; private set; }
    public static ImageSource Folder { get; private set; }
    public static ImageSource Shortcut { get; private set; }
    public static ImageSource Start { get; private set; }

    public static void LoadIcons()
    {
        MainIconSource = IconManager.Convert(IconManager.Extract(Shell32Dll, 249, true));
        CancelImage = IconManager.Convert(IconManager.Extract(ImageResDll, 100, false));
        OkImage = IconManager.Convert(IconManager.Extract(ImageResDll, 101, false));
        Folder = IconManager.Convert(IconManager.Extract(Shell32Dll, 4, true));
        Shortcut = IconManager.Convert(IconManager.Extract(Shell32Dll, 146, true));
        Start = IconManager.Convert(IconManager.Extract(Shell32Dll, 137, true));
    }
}
