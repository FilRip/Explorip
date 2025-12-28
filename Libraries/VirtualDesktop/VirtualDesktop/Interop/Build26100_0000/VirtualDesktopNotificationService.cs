using System;
using System.Runtime.InteropServices;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;
using VirtualDesktop.Utils;

namespace VirtualDesktop.Interop.Build26100;

public class VirtualDesktopNotificationService : ComWrapperBase<IVirtualDesktopNotificationService>, IVirtualDesktopNotificationService
{
    private readonly ComWrapperFactory _factory;

    internal VirtualDesktopNotificationService(ComInterfaceAssembly assembly, ComWrapperFactory factory)
        : base(assembly, Clsid.VirtualDesktopNotificationService)
    {
        this._factory = factory;
    }

    public IDisposable Register(IVirtualDesktopNotification proxy)
    {
        Type type = this.ComInterfaceAssembly.GetType("VirtualDesktopNotification");
        EventListenerBase listener = Activator.CreateInstance(type) as EventListenerBase
            ?? throw new VirtualDesktopException($"{nameof(EventListenerBase)} inheritance type is not found in the COM interface assembly.");

        listener.Notification = proxy;
        listener.Factory = this._factory;

        uint dwCookie = this.InvokeMethod<uint>(Args(listener));
        return Disposable.Create(() => this.Unregister(dwCookie));
    }

    private void Unregister(uint dwCookie)
    {
        try
        {
            this.InvokeMethod(Args(dwCookie));
        }
        catch (COMException ex) when (ex.Match(HResult.RPC_S_SERVER_UNAVAILABLE))
        {
            // Nothing particular to do.
        }
    }

    public abstract class EventListenerBase
    {
        internal ComWrapperFactory Factory { get; set; } = null!;

        internal IVirtualDesktopNotification Notification { get; set; } = null!;

        protected void CreatedCore(object pDesktop)
            => this.Notification.VirtualDesktopCreated(this.Wrap(pDesktop));

        protected void DestroyBeginCore(object pDesktopDestroyed, object pDesktopFallback)
            => this.Notification.VirtualDesktopDestroyBegin(this.Wrap(pDesktopDestroyed), this.Wrap(pDesktopFallback));

        protected void DestroyFailedCore(object pDesktopDestroyed, object pDesktopFallback)
            => this.Notification.VirtualDesktopDestroyFailed(this.Wrap(pDesktopDestroyed), this.Wrap(pDesktopFallback));

        protected void DestroyedCore(object pDesktopDestroyed, object pDesktopFallback)
            => this.Notification.VirtualDesktopDestroyed(this.Wrap(pDesktopDestroyed), this.Wrap(pDesktopFallback));

        protected void MovedCore(object pDesktop, int nIndexFrom, int nIndexTo)
            => this.Notification.VirtualDesktopMoved(this.Wrap(pDesktop), nIndexFrom, nIndexTo);

        protected void RenamedCore(object pDesktop, HString chName)
            => this.Notification.VirtualDesktopRenamed(this.Wrap(pDesktop), chName);

        protected void ViewChangedCore(object view)
            => this.Notification.ViewVirtualDesktopChanged(this.Factory.ApplicationView(view).Interface);

        protected void CurrentChangedCore(object pDesktopOld, object pDesktopNew)
            => this.Notification.CurrentVirtualDesktopChanged(this.Wrap(pDesktopOld), this.Wrap(pDesktopNew));

        protected void WallpaperChangedCore(object pDesktop, HString chPath)
            => this.Notification.VirtualDesktopWallpaperChanged(this.Wrap(pDesktop), chPath);

        protected void SwitchedCore(object pDesktop)
            => this.Notification.VirtualDesktopSwitched(this.Wrap(pDesktop));

        protected void RemoteConnectedCore(object pDesktop)
            => this.Notification.RemoteVirtualDesktopConnected(this.Wrap(pDesktop));

        private IVirtualDesktop Wrap(object desktop)
            => this.Factory.VirtualDesktop(desktop).Interface;
    }
}
