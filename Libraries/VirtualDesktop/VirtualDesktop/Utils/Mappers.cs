using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Utils;

public static class Mappers
{
    internal static Models.VirtualDesktop ToVirtualDesktop(this IVirtualDesktop desktop)
        => VirtualDesktopManager.FromComObject(desktop);
}
