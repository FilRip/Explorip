using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using VirtualDesktop.Interop.Proxy;
using VirtualDesktop.Models;
using VirtualDesktop.Utils;

namespace VirtualDesktop;

public sealed class VirtualDesktopEvents : IVirtualDesktopNotification
{
    private static readonly ConcurrentDictionary<IntPtr, ViewChangedListener> _viewChangedEventListeners = new();

    /// <summary>
    /// Register a listener to receive changes in the application view.
    /// </summary>
    /// <param name="targetHwnd">The target window handle to receive events from. If specify <see cref="IntPtr.Zero"/>, all changes will be delivered.</param>
    /// <param name="action">Action to be performed.</param>
    /// <returns><see cref="IDisposable"/> instance for unsubscribing.</returns>
    public static IDisposable RegisterViewChanged(IntPtr targetHwnd, Action<IntPtr> action)
    {
        VirtualDesktopManager.InitializeIfNeeded();

        ViewChangedListener listener = _viewChangedEventListeners.GetOrAdd(targetHwnd, x => new ViewChangedListener(x));
        listener.Listeners.Add(action);

        return Disposable.Create(() => listener.Listeners.Remove(action));
    }

    #region Events

    /// <summary>
    /// Occurs when a virtual desktop is created.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<Models.VirtualDesktop>? Created;

    public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyBegin;

    public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyFailed;

    /// <summary>
    /// Occurs when a virtual desktop is destroyed.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopDestroyEventArgs>? Destroyed;

    /// <summary>
    /// Occurs when the current virtual desktop is changed.
    /// </summary>
    /// <remarks>
    /// The internal initialization is triggered by the call of a static property/method.<br/>
    /// Therefore, events are not fired just by subscribing to them.<br/>
    /// <br/>
    /// If you want to use only event subscription, the following code is recommended.<br/>
    /// <code>
    /// VirtualDesktop.Configuration();
    /// </code>
    /// </remarks>
    public static event EventHandler<VirtualDesktopChangedEventArgs>? CurrentChanged;

    /// <summary>
    /// Occurs when the virtual desktop is moved.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopMovedEventArgs>? Moved;

    /// <summary>
    /// Occurs when a virtual desktop is renamed.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopRenamedEventArgs>? Renamed;

    /// <summary>
    /// Occurs when a virtual desktop wallpaper is changed.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs>? WallpaperChanged;

    /// <summary>
    /// Occurs when a virtual desktop is switched. Seems duplicate to ViewVirtualDesktopChanged, the difference is not yet known. Both are fired when swtiching.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<VirtualDesktopSwitchedEventArgs>? Switched;

    /// <summary>
    /// Occurs when a remote desktop is connected. Should be related to Windows 365 Cloud PC: https://www.microsoft.com/store/productId/9N1F85V9T8BN.
    /// </summary>
    /// <remarks>
    /// See <see cref="CurrentChanged"/> for details.
    /// </remarks>
    public static event EventHandler<RemoteVirtualDesktopConnectedEventArgs>? RemoteConnected;

    #endregion

    public void VirtualDesktopCreated(IVirtualDesktop pDesktop)
        => Created?.Invoke(null, pDesktop.ToVirtualDesktop());

    public void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
        => DestroyBegin?.Invoke(null, new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback));

    public void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
        => DestroyFailed?.Invoke(null, new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback));

    public void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
        => Destroyed?.Invoke(null, new VirtualDesktopDestroyEventArgs(pDesktopDestroyed, pDesktopFallback));

    public void VirtualDesktopIsPerMonitorChanged(int i)
    {
    }

    public void VirtualDesktopMoved(IVirtualDesktop pDesktop, int nIndexFrom, int nIndexTo)
        => Moved?.Invoke(null, new VirtualDesktopMovedEventArgs(pDesktop, nIndexFrom, nIndexTo));

    public void ViewVirtualDesktopChanged(IApplicationView pView)
    {
        if (_viewChangedEventListeners.TryGetValue(IntPtr.Zero, out ViewChangedListener? all))
            all.Call();
        if (_viewChangedEventListeners.TryGetValue(pView.GetThumbnailWindow(), out ViewChangedListener? listener))
            listener.Call();
    }

    public void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
        => CurrentChanged?.Invoke(null, new VirtualDesktopChangedEventArgs(pDesktopOld, pDesktopNew));

    public void VirtualDesktopRenamed(IVirtualDesktop pDesktop, string chName)
    {
        Models.VirtualDesktop desktop = pDesktop.ToVirtualDesktop();
        desktop._name = chName;

        Renamed?.Invoke(null, new VirtualDesktopRenamedEventArgs(desktop, chName));
    }

    public void VirtualDesktopWallpaperChanged(IVirtualDesktop pDesktop, string chPath)
    {
        Models.VirtualDesktop desktop = pDesktop.ToVirtualDesktop();
        desktop._wallpaperPath = chPath;

        WallpaperChanged?.Invoke(null, new VirtualDesktopWallpaperChangedEventArgs(desktop, chPath));
    }

    public void VirtualDesktopSwitched(IVirtualDesktop pDesktop) =>
        Switched?.Invoke(null, new VirtualDesktopSwitchedEventArgs(pDesktop));

    public void RemoteVirtualDesktopConnected(IVirtualDesktop pDesktop) =>
        RemoteConnected?.Invoke(null, new RemoteVirtualDesktopConnectedEventArgs(pDesktop));

    private sealed class ViewChangedListener(IntPtr targetHandle)
    {
        private readonly IntPtr _targetHandle = targetHandle;

        public List<Action<IntPtr>> Listeners { get; } = [];

        public void Call()
        {
            foreach (Action<IntPtr> listener in this.Listeners)
                listener(this._targetHandle);
        }
    }
}
