using System.Runtime.InteropServices;

namespace ExploripSharedCopy.WinAPI;

public static class Uxtheme
{
    public enum PreferredAppMode
    {
        APPMODE_DEFAULT = 0,
        APPMODE_ALLOWDARK = 1,
        APPMODE_FORCEDARK = 2,
        APPMODE_FORCELIGHT = 3,
        APPMODE_MAX = 4,
    }

    [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);

    [DllImport("uxtheme.dll", EntryPoint = "#136", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern void FlushMenuThemes();
}
