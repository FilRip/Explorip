﻿using System.Windows.Media;

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

    internal static void Init()
    {
        CancelImage = IconManager.GetIconFromFile("imageres.dll", 100);
        OkImage = IconManager.GetIconFromFile("imageres.dll", 101);
        Folder = IconManager.GetIconFromFile("shell32.dll", 4);
        Shortcut = IconManager.GetIconFromFile("shell32.dll", 146);
        IconXSize = NativeMethods.GetSystemMetrics(NativeMethods.SM.CXICON);
        IconYSize = NativeMethods.GetSystemMetrics(NativeMethods.SM.CYICON);
    }
}
