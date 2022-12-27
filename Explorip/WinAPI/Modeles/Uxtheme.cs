using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    internal enum PreferredAppMode
    {
        APPMODE_DEFAULT = 0,
        APPMODE_ALLOWDARK = 1,
        APPMODE_FORCEDARK = 2,
        APPMODE_FORCELIGHT = 3,
        APPMODE_MAX = 4
    }

    internal static class Uxtheme
    {
        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);
    }
}
