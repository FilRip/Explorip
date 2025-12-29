using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using static VirtualDesktop.Utils.SafeInvokeHelper;

using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Build10240;
using VirtualDesktop.Interop.Build20348;
using VirtualDesktop.Interop.Build22000;
using VirtualDesktop.Interop.Build22621;
using VirtualDesktop.Interop.Build26100;
using VirtualDesktop.Interop.Proxy;
using VirtualDesktop.Utils;

namespace VirtualDesktop;

public static class VirtualDesktopManager
{
    #region Fields

    private static int _numDesktop;
    private static readonly VirtualDesktopProvider _provider = CreateProvider();
    private static readonly ConcurrentDictionary<Guid, Models.VirtualDesktop> _knownDesktops = new();
    private static ExplorerRestartListenerWindow? _explorerRestartListener;
    private static VirtualDesktopCompilerConfiguration _configuration = new();
    private static ComInterfaceAssembly? _assembly;
    private static IDisposable? _notificationListener;

    #endregion

    #region Properties

    public static VirtualDesktopCompilerConfiguration GetCurrentConfiguration
    {
        get { return _configuration; }
    }

    internal static VirtualDesktopProvider GetProvider
    {
        get { return _provider; }
    }

    /// <summary>
    /// Gets a value indicating virtual desktops are supported by the host.
    /// </summary>
    public static bool IsSupported
        => _provider.IsSupported;

    public static bool IsInitialized
        => _provider.IsInitialized;

    /// <summary>
    /// Gets the virtual desktop that is currently displayed.
    /// </summary>
    public static Models.VirtualDesktop Current
    {
        get
        {
            InitializeIfNeeded();

            return _provider.VirtualDesktopManagerInternal
                .GetCurrentDesktop()
                .ToVirtualDesktop();
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    ///  Create provider for current OS version.
    ///  Test order matters. Make sure you test the highest version first.
    /// </summary>
    /// <returns></returns>
    private static VirtualDesktopProvider CreateProvider()
    {
        Version v = OS.Build;

        if (v >= new Version(10, 0, 26100, 0))
        {
            return new VirtualDesktopProvider26100();
        }

        if (v >= new Version(10, 0, 22621, 2215))
        {
            return new VirtualDesktopProvider22621();
        }

        if (v >= new Version(10, 0, 22000, 0))
        {
            return new VirtualDesktopProvider22000();
        }

        if (v >= new Version(10, 0, 20348, 0))
        {
            return new VirtualDesktopProvider20348();
        }

        if (v >= new Version(10, 0, 10240, 0))
        {
            return new VirtualDesktopProvider10240();
        }

        return new VirtualDesktopProvider.NotSupported();
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Returns an array of available virtual desktops.
    /// </summary>
    public static Models.VirtualDesktop[] GetDesktops()
    {
        InitializeIfNeeded();

        return [.. _provider.VirtualDesktopManagerInternal
            .GetDesktops()
            .Select(x => x.ToVirtualDesktop())];
    }

    /// <summary>
    /// Returns a new virtual desktop.
    /// </summary>
    public static Models.VirtualDesktop Create()
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopManagerInternal
            .CreateDesktop()
            .ToVirtualDesktop();
    }

    /// <summary>
    /// Returns a virtual desktop matching the specified identifier.
    /// </summary>
    /// <param name="desktopId">The identifier of the virtual desktop to return.</param>
    /// <remarks>Returns <see langword="null" /> if the identifier is not associated with any available desktops.</remarks>
    public static Models.VirtualDesktop? FromId(Guid desktopId)
    {
        InitializeIfNeeded();

        return SafeInvoke(() => _provider.VirtualDesktopManagerInternal
            .FindDesktop(desktopId)
            .ToVirtualDesktop());
    }

    /// <summary>
    /// Returns the virtual desktop the window is located on.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <remarks>Returns <see langword="null" /> if the handle is not associated with any open windows.</remarks>
    public static Models.VirtualDesktop? FromHwnd(IntPtr hWnd)
    {
        InitializeIfNeeded();

        if (hWnd == IntPtr.Zero || IsPinnedWindow(hWnd))
            return null;

        return SafeInvoke(() =>
        {
            Guid desktopId = _provider.VirtualDesktopManager.GetWindowDesktopId(hWnd);
            return _provider.VirtualDesktopManagerInternal
                .FindDesktop(desktopId)
                .ToVirtualDesktop();
        }, HResult.REGDB_E_CLASSNOTREG, HResult.TYPE_E_ELEMENTNOTFOUND);
    }

    /// <summary>
    /// Apply the specified wallpaper to all desktops.
    /// </summary>
    /// <remarks>
    /// This is not supported on Windows 10.
    /// </remarks>
    /// <param name="path">Wallpaper image path.</param>
    public static void UpdateWallpaperForAllDesktops(string path)
    {
        InitializeIfNeeded();

        _provider.VirtualDesktopManagerInternal.UpdateWallpaperPathForAllDesktops(path);
    }

    /// <summary>
    /// Moves a window to the specified virtual desktop.
    /// </summary>
    /// <param name="hWnd">The handle of the window to be moved.</param>
    /// <param name="virtualDesktop">The virtual desktop to move the window to.</param>
    public static void MoveToDesktop(IntPtr hWnd, Models.VirtualDesktop virtualDesktop)
    {
        InitializeIfNeeded();

        int result = NativeMethods.GetWindowThreadProcessId(hWnd, out int processId);
        if (result < 0)
            throw new VirtualDesktopException($"The process associated with '{hWnd}' not found.");

        if (processId == Process.GetCurrentProcess().Id)
        {
            _provider.VirtualDesktopManager.MoveWindowToDesktop(hWnd, virtualDesktop.Id);
        }
        else
        {
            _provider.VirtualDesktopManagerInternal.MoveViewToDesktop(hWnd, virtualDesktop._source);
        }
    }

    /// <summary>
    /// Determines whether this window is on the current virtual desktop.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    public static bool IsCurrentVirtualDesktop(IntPtr hWnd)
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopManager.IsWindowOnCurrentVirtualDesktop(hWnd);
    }

    /// <summary>
    /// Try gets the App User Model ID with the specified foreground window.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <param name="appUserModelId">App User Model ID.</param>
    /// <returns><see langword="true" /> if the App User Model ID is available, <see langword="false" /> otherwise.</returns>
    public static bool TryGetAppUserModelId(IntPtr hWnd, out string appUserModelId)
    {
        InitializeIfNeeded();

        try
        {
            appUserModelId = _provider.ApplicationViewCollection
                .GetViewForHwnd(hWnd)
                .GetAppUserModelId();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{nameof(TryGetAppUserModelId)} failed.");
            Debug.WriteLine(ex);
        }

        appUserModelId = "";
        return false;
    }

    #endregion

    #region Init/configure

    /// <summary>
    /// Initialize using the default settings. This method should always be called first.
    /// </summary>
    public static void Configure()
    {
        _configuration = new VirtualDesktopCompilerConfiguration();
        InitializeIfNeeded();
    }

    /// <summary>
    /// Sets the behavior for compiling the assembly. This method should always be called first.
    /// </summary>
    public static void Configure(VirtualDesktopCompilerConfiguration configuration)
    {
        _configuration = configuration;
        InitializeIfNeeded();
    }

    internal static Models.VirtualDesktop FromComObject(IVirtualDesktop desktop)
        => _knownDesktops.GetOrAdd(desktop.GetID(), _ => new Models.VirtualDesktop(desktop));

    internal static void InitializeIfNeeded()
    {
        if (!IsSupported)
            throw new NotSupportedException("You must target Windows 10 or later in your 'app.manifest' and run without debugging.");
        if (_provider.IsInitialized)
            return;

        if (_configuration.StartHIddenWindowOfExplorerSpy)
        {
            _explorerRestartListener = new ExplorerRestartListenerWindow(() => HandleExplorerRestarted());
            _explorerRestartListener.Show();
        }
        InitializeCore();
    }

    public static void StopExplorerSpy()
    {
        _explorerRestartListener?.Close();
        _explorerRestartListener = null;
    }

    private static void HandleExplorerRestarted()
    {
        _numDesktop = 0;
        _knownDesktops.Clear();
        _provider.IsInitialized = false;
        InitializeCore();
    }

    private static void InitializeCore()
    {
        _assembly ??= new ComInterfaceAssemblyBuilder(_configuration).GetAssembly();
        _provider.Initialize(_assembly);

        _notificationListener?.Dispose();
        _notificationListener = _provider.VirtualDesktopNotificationService.Register(new VirtualDesktopEvents());
    }

    internal static int GetNextNum()
    {
        return ++_numDesktop;
    }

    #endregion

    #region static members (pinned apps)

    /// <summary>
    /// Determines whether the specified window is pinned.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <returns><see langword="true" /> if pinned, <see langword="false" /> otherwise.</returns>
    public static bool IsPinnedWindow(IntPtr hWnd)
    {
        InitializeIfNeeded();

        return SafeInvoke(() => _provider.VirtualDesktopPinnedApps.IsViewPinned(hWnd));
    }

    /// <summary>
    /// Pins the specified window, showing it on all virtual desktops.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <returns><see langword="true" /> if already pinned or successfully pinned, <see langword="false" /> otherwise (most of the time, the target window is not found or not ready).</returns>
    public static bool PinWindow(IntPtr hWnd)
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopPinnedApps.IsViewPinned(hWnd)
            || SafeInvoke(() => _provider.VirtualDesktopPinnedApps.PinView(hWnd));
    }

    /// <summary>
    /// Unpins the specified window.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <returns><see langword="true" /> if already unpinned or successfully unpinned, <see langword="false" /> otherwise (most of the time, the target window is not found or not ready).</returns>
    public static bool UnpinWindow(IntPtr hWnd)
    {
        InitializeIfNeeded();

        return !_provider.VirtualDesktopPinnedApps.IsViewPinned(hWnd)
            || SafeInvoke(() => _provider.VirtualDesktopPinnedApps.UnpinView(hWnd));
    }

    /// <summary>
    /// Determines whether the specified app is pinned.
    /// </summary>
    /// <param name="appUserModelId">App User Model ID. <see cref="TryGetAppUserModelId"/> method may be helpful.</param>
    /// <returns><see langword="true" /> if pinned, <see langword="false" /> otherwise.</returns>
    public static bool IsPinnedApplication(string appUserModelId)
    {
        InitializeIfNeeded();

        return SafeInvoke(() => _provider.VirtualDesktopPinnedApps.IsAppIdPinned(appUserModelId));
    }

    /// <summary>
    /// Pins the specified app, showing it on all virtual desktops.
    /// </summary>
    /// <param name="appUserModelId">App User Model ID. <see cref="TryGetAppUserModelId"/> method may be helpful.</param>
    /// <returns><see langword="true" /> if already pinned or successfully pinned, <see langword="false" /> otherwise (most of the time, app id is incorrect).</returns>
    public static bool PinApplication(string appUserModelId)
    {
        InitializeIfNeeded();

        return _provider.VirtualDesktopPinnedApps.IsAppIdPinned(appUserModelId)
            || SafeInvoke(() => _provider.VirtualDesktopPinnedApps.PinAppID(appUserModelId));
    }

    /// <summary>
    /// Unpins the specified app.
    /// </summary>
    /// <param name="appUserModelId">App User Model ID. <see cref="TryGetAppUserModelId"/> method may be helpful.</param>
    /// <returns><see langword="true" /> if already unpinned or successfully unpinned, <see langword="false" /> otherwise (most of the time, app id is incorrect).</returns>
    public static bool UnpinApplication(string appUserModelId)
    {
        InitializeIfNeeded();

        return !_provider.VirtualDesktopPinnedApps.IsAppIdPinned(appUserModelId)
            || SafeInvoke(() => _provider.VirtualDesktopPinnedApps.UnpinAppID(appUserModelId));
    }

    #endregion
}
