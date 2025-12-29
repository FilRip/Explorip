using System;

using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build10240;

internal class VirtualDesktopPinnedApps(ComInterfaceAssembly assembly, ComWrapperFactory factory) : ComWrapperBase<IVirtualDesktopPinnedApps>(assembly, Clsid.VirtualDesktopPinnedApps), IVirtualDesktopPinnedApps
{
    private readonly ComWrapperFactory _factory = factory;

    public bool IsViewPinned(IntPtr hWnd)
        => this.InvokeMethod<bool>(this.ArgsWithApplicationView(hWnd));

    public void PinView(IntPtr hWnd)
        => this.InvokeMethod(this.ArgsWithApplicationView(hWnd));

    public void UnpinView(IntPtr hWnd)
        => this.InvokeMethod(this.ArgsWithApplicationView(hWnd));

    public bool IsAppIdPinned(string appId)
        => this.InvokeMethod<bool>(Args(appId));

    public void PinAppID(string appId)
        => this.InvokeMethod(Args(appId));

    public void UnpinAppID(string appId)
        => this.InvokeMethod(Args(appId));

    private object?[] ArgsWithApplicationView(IntPtr hWnd)
        => Args(this._factory.ApplicationViewFromHwnd(hWnd).ComObject);
}
