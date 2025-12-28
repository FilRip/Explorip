using System;

namespace VirtualDesktop.Interop.Proxy;

[ComInterface()]
public interface IVirtualDesktopNotificationService
{
    IDisposable Register(IVirtualDesktopNotification proxy);
}
