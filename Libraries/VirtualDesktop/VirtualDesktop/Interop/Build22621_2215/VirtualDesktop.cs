using System;

using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build22621;

internal class VirtualDesktop(ComInterfaceAssembly assembly, object comObject) : ComWrapperBase<IVirtualDesktop>(assembly, comObject), IVirtualDesktop
{
    private Guid? _id;

    public bool IsViewVisible(IntPtr hWnd)
        => this.InvokeMethod<bool>(Args(hWnd));

    public Guid GetID()
        => this._id ?? (Guid)(this._id = this.InvokeMethod<Guid>());

    public string GetName()
        => this.InvokeMethod<HString>();

    public string GetWallpaperPath()
        => this.InvokeMethod<HString>();

    public bool IsRemote()
        => this.InvokeMethod<bool>();
}
