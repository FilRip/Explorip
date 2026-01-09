using System;
using System.IO;
using System.Windows.Media;

using Explorip.Helpers;

namespace Explorip.Constants;

internal static class Icons
{
    private const string ImageResDll = "imageres.dll";
    private const string Shell32Dll = "shell32.dll";

    public static ImageSource CancelImage { get; set; }
    public static ImageSource OkImage { get; set; }
    public static ImageSource Folder { get; set; }
    public static ImageSource Shortcut { get; set; }
    public static ImageSource Pin { get; private set; }
    public static ImageSource Unpin { get; private set; }
    public static ImageSource Close { get; private set; }
    public static ImageSource Lock { get; private set; }
    public static ImageSource UpdateAndRestart { get; private set; }
    public static ImageSource UpdateAndShutdown { get; private set; }
    public static ImageSource Shutdown { get; private set; }
    public static ImageSource Refresh { get; private set; }
    public static ImageSource Taskmgr { get; private set; }
    public static ImageSource Keyboard { get; private set; }
    public static ImageSource KeyboardSettings { get; private set; }
    public static ImageSource DateTime { get; private set; }
    public static ImageSource SwitchUser { get; private set; }
    public static ImageSource SignOut { get; private set; }
    public static ImageSource Sleep { get; private set; }
    public static ImageSource SearchButton { get; private set; }
    public static ImageSource ExpandButtonToRight { get; private set; }
    public static ImageSource ExpandButtonToLeft { get; private set; }

    internal static void Init()
    {
        CancelImage = IconManager.GetIconFromFile(ImageResDll, 100);
        OkImage = IconManager.GetIconFromFile(ImageResDll, 101);
        Folder = IconManager.GetIconFromFile(Shell32Dll, 4);
        Shortcut = IconManager.GetIconFromFile(Shell32Dll, 146);
        Pin = IconManager.StringToImageSource("\uE840", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Unpin = IconManager.StringToImageSource("\uE77A", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Close = IconManager.GetIconFromFile(ImageResDll, 84);
        Lock = IconManager.GetIconFromFile(Shell32Dll, 47);
        Refresh = IconManager.StringToImageSource("\uE72C", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        UpdateAndRestart = IconManager.StringToImageSource("\uE72C", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush, overlay: IconManager.GetIconFromFile(ImageResDll, 73), offsetOverlay: 2);
        Shutdown = IconManager.GetIconFromFile(Shell32Dll, 27);
        UpdateAndShutdown = IconManager.GetIconFromFile(Shell32Dll, 27, overlay: ImageResDll, indexOverlay: 73);
        Taskmgr = IconManager.GetIconFromFile(Path.Combine(Environment.SystemDirectory, "taskmgr.exe"), 0, false);
        Keyboard = IconManager.StringToImageSource("\uE765", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        KeyboardSettings = IconManager.StringToImageSource("\uF210", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        DateTime = IconManager.GetIconFromFile(ImageResDll, 138);
        SwitchUser = IconManager.GetIconFromFile(ImageResDll, 74);
        SignOut = IconManager.StringToImageSource("\uF3B1", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Sleep = IconManager.GetIconFromFile(ImageResDll, 96);
        SearchButton = IconManager.StringToImageSource("\uE721", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush, width: 32, height: 32, fontSize: 32);
        ExpandButtonToRight = IconManager.StringToImageSource("\uE970", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        ExpandButtonToLeft = IconManager.StringToImageSource("\uE96F", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
    }
}
