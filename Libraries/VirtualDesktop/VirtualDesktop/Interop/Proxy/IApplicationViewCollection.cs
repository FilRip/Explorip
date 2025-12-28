using System;

using VirtualDesktop.Interop;

namespace VirtualDesktop.Interop.Proxy;

[ComInterface()]
public interface IApplicationViewCollection
{
    IApplicationView GetViewForHwnd(IntPtr hWnd);
}
