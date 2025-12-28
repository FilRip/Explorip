using System;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build20348;

internal class VirtualDesktop(ComInterfaceAssembly assembly, object comObject) : ComWrapperBase<IVirtualDesktop>(assembly, comObject), IVirtualDesktop
{
    private Guid? _id;

    public bool IsViewVisible(IntPtr hWnd)
        => this.InvokeMethod<bool>(Args(hWnd));

    public Guid GetID()
        => this._id ?? (Guid)(this._id = this.InvokeMethod<Guid>());

    public IntPtr GetMonitor(IntPtr monitor)
        => this.InvokeMethod<IntPtr>(Args(monitor));

    public string GetName()
        => this.InvokeMethod<HString>();

    public string GetWallpaperPath()
    {
        return "";
    }
}
