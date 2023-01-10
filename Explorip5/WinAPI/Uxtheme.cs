using System.Runtime.InteropServices;

namespace Explorip.WinAPI
{
    internal static class Uxtheme
    {
        internal enum PreferredAppMode
        {
            APPMODE_DEFAULT = 0,
            APPMODE_ALLOWDARK = 1,
            APPMODE_FORCEDARK = 2,
            APPMODE_FORCELIGHT = 3,
            APPMODE_MAX = 4
        }

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);
    }
}
