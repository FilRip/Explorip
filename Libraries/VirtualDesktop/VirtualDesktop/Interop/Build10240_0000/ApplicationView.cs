using System;

using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop.Interop.Build10240;

internal class ApplicationView(ComInterfaceAssembly assembly, object comObject) : ComWrapperBase<IApplicationView>(assembly, comObject), IApplicationView
{
    public IntPtr GetThumbnailWindow()
        => this.InvokeMethod<IntPtr>();

    public string GetAppUserModelId()
        => this.InvokeMethod<string>() ?? throw new VirtualDesktopException("Failed to get AppUserModelId.");

    public Guid GetVirtualDesktopId()
        => this.InvokeMethod<Guid>();
}
