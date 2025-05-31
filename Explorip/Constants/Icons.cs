using System;
using System.IO;
using System.Windows.Media;

using Explorip.Helpers;

using ManagedShell.Interop;

namespace Explorip.Constants;

internal static class Icons
{
    public static ImageSource CancelImage { get; set; }
    public static ImageSource OkImage { get; set; }
    public static ImageSource Folder { get; set; }
    public static ImageSource Shortcut { get; set; }
    public static int IconXSize { get; set; }
    public static int IconYSize { get; set; }
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

    internal static void Init()
    {
        CancelImage = IconManager.GetIconFromFile("imageres.dll", 100);
        OkImage = IconManager.GetIconFromFile("imageres.dll", 101);
        Folder = IconManager.GetIconFromFile("shell32.dll", 4);
        Shortcut = IconManager.GetIconFromFile("shell32.dll", 146);
        IconXSize = NativeMethods.GetSystemMetrics(NativeMethods.SM.CXICON);
        IconYSize = NativeMethods.GetSystemMetrics(NativeMethods.SM.CYICON);
        Pin = IconManager.StringToImageSource("\uE840", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Unpin = IconManager.StringToImageSource("\uE77A", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Close = IconManager.StringToImageSource("\uE8BB", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Lock = IconManager.StringToImageSource("\uE72E", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Refresh = IconManager.StringToImageSource("\uE72C", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        UpdateAndRestart = IconManager.StringToImageSource("\uF83E", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Shutdown = IconManager.StringToImageSource("\uE7E8", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        UpdateAndShutdown = IconManager.StringToImageSource("\uF83D", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        Taskmgr = IconManager.GetIconFromFile(Path.Combine(Environment.SystemDirectory, "taskmgr.exe"), 0, false);
        Keyboard = IconManager.StringToImageSource("\uE765", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
        KeyboardSettings = IconManager.StringToImageSource("\uF210", foregroundColor: ExploripSharedCopy.Constants.Colors.ForegroundColorBrush);
    }
}
