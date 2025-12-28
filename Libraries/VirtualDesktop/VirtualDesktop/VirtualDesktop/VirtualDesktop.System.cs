using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using VirtualDesktop;
using VirtualDesktop.Interop;
using VirtualDesktop.Interop.Proxy;
using VirtualDesktop.Utils;

using VirtualDesktop.Interop.Build10240;
using VirtualDesktop.Interop.Build20348;
using VirtualDesktop.Interop.Build22000;
using VirtualDesktop.Interop.Build22621;
using VirtualDesktop.Interop.Build26100;

namespace VirtualDesktop;

partial class VirtualDesktop
{
    private static readonly VirtualDesktopProvider _provider = CreateProvider();
    private static readonly ConcurrentDictionary<Guid, VirtualDesktop> _knownDesktops = new();
    private static ExplorerRestartListenerWindow? _explorerRestartListener;
    private static VirtualDesktopCompilerConfiguration _configuration = new();
    private static ComInterfaceAssembly? _assembly;
    private static IDisposable? _notificationListener;
    private readonly IVirtualDesktop _source;
    private string _name;
    private string _wallpaperPath;

    /// <summary>
    /// Gets a value indicating virtual desktops are supported by the host.
    /// </summary>
    public static bool IsSupported
        => _provider.IsSupported;

    public static bool IsInitialized
        => _provider.IsInitialized;

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

    private VirtualDesktop(IVirtualDesktop source)
    {
        this._source = source;
        this._name = source.GetName();
        this._wallpaperPath = source.GetWallpaperPath();
        this.Id = source.GetID();
    }

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

    internal static VirtualDesktop FromComObject(IVirtualDesktop desktop)
        => _knownDesktops.GetOrAdd(desktop.GetID(), _ => new VirtualDesktop(desktop));

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
        _knownDesktops.Clear();
        _provider.IsInitialized = false;
        InitializeCore();
    }

    private static void InitializeCore()
    {
        _assembly ??= new ComInterfaceAssemblyBuilder(_configuration).GetAssembly();
        _provider.Initialize(_assembly);

        _notificationListener?.Dispose();
        _notificationListener = _provider.VirtualDesktopNotificationService.Register(new EventProxy());
    }

    private static T? SafeInvoke<T>(Func<T> action, params HResult[] hResult)
    {
        try
        {
            return action();
        }
        catch (COMException ex) when (ex.Match(hResult is { Length: 0 } ? [HResult.TYPE_E_ELEMENTNOTFOUND,] : hResult))
        {
            return default;
        }
    }

    private static bool SafeInvoke(Action action, params HResult[] hResult)
    {
        try
        {
            action();
            return true;
        }
        catch (COMException ex) when (ex.Match(hResult is { Length: 0 } ? [HResult.TYPE_E_ELEMENTNOTFOUND,] : hResult))
        {
            return false;
        }
    }
}
