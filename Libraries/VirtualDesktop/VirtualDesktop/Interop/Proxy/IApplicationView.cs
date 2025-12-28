using System;

namespace VirtualDesktop.Interop.Proxy;

[ComInterface()]
public interface IApplicationView
{
    IntPtr GetThumbnailWindow();

    string GetAppUserModelId();

    Guid GetVirtualDesktopId();
}
