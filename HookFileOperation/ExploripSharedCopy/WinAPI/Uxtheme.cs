using System.Runtime.InteropServices;

namespace ExploripSharedCopy.WinAPI
{
    public static class Uxtheme
    {
        public enum PreferredAppMode
        {
            APPMODE_DEFAULT = 0,
            APPMODE_ALLOWDARK = 1,
            APPMODE_FORCEDARK = 2,
            APPMODE_FORCELIGHT = 3,
            APPMODE_MAX = 4
        }

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int PrivateSetPreferredAppMode(PreferredAppMode preferredAppMode);

#pragma warning disable S4200 // Native methods should be wrapped
        public static int SetPreferredAppMode(PreferredAppMode preferredAppMode)
        {
            return PrivateSetPreferredAppMode(preferredAppMode);
        }
#pragma warning restore S4200 // Native methods should be wrapped
    }
}
