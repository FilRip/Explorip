using System;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build10240;

internal class ApplicationViewCollection(ComInterfaceAssembly assembly) : ComWrapperBase<IApplicationViewCollection>(assembly), IApplicationViewCollection
{
    public ApplicationView GetViewForHwnd(IntPtr hWnd)
    {
        object view = this.InvokeMethod<object>(Args(hWnd))
            ?? new ArgumentException("ApplicationView is not found.", nameof(hWnd));

        return new ApplicationView(this.ComInterfaceAssembly, view);
    }

    IApplicationView IApplicationViewCollection.GetViewForHwnd(IntPtr hWnd)
        => this.GetViewForHwnd(hWnd);
}
