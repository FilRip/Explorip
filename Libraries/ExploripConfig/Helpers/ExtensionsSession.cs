using ManagedShell.Interop;

namespace ExploripConfig.Helpers;

internal static class ExtensionsSession
{
    internal static bool IsRdp
    {
        get
        {
            return NativeMethods.GetSystemMetrics(NativeMethods.SM.REMOTESESSION) == 1;
        }
    }
}
