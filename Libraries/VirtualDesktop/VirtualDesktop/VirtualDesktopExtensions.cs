using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop;

public static class VirtualDesktopExtensions
{
    internal static VirtualDesktop ToVirtualDesktop(this IVirtualDesktop desktop)
        => VirtualDesktop.FromComObject(desktop);
}
