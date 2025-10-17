using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray;

public class NotificationArea(TrayService trayService, ExplorerTrayService explorerTrayService) : DependencyObject, IDisposable
{
    const string HEALTH_GUID = "7820ae76-23e3-4229-82c1-e41cb67d5b9c";
    const string MEETNOW_GUID = "7820ae83-23e3-4229-82c1-e41cb67d5b9c";
    const string NETWORK_GUID = "7820ae74-23e3-4229-82c1-e41cb67d5b9c";
    const string POWER_GUID = "7820ae75-23e3-4229-82c1-e41cb67d5b9c";
    const string VOLUME_GUID = "7820ae73-23e3-4229-82c1-e41cb67d5b9c";

    public static readonly string[] DEFAULT_PINNED = [
        HEALTH_GUID,
        POWER_GUID,
        NETWORK_GUID,
        VOLUME_GUID,
    ];

    internal static readonly List<string> Win11ActionCenterIcons =
    [
        NETWORK_GUID,
        POWER_GUID,
        VOLUME_GUID,
    ];

    readonly NativeMethods.Rect defaultPlacement = new()
    {
        Top = 0,
        Left = GetSystemMetrics(0) - 200,
        Bottom = 23,
        Right = 23,
    };

    public IntPtr Handle { get; private set; }
    public bool IsFailed { get; private set; }
    public bool PinDefaultIcons { get; private set; } = true;

    public event EventHandler<NotificationBalloonEventArgs> NotificationBalloonShown;

    private SystrayDelegate trayDelegate;
#pragma warning disable S1450 // False positive from Sonar
    private IconDataDelegate iconDataDelegate;
    private TrayHostSizeDelegate trayHostSizeDelegate;
#pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables
    private readonly object _lockObject = new();
    private ShellServiceObject shellServiceObject;
    private TrayHostSizeData trayHostSizeData = new()
    {
        edge = ABEdge.ABE_TOP,
        rc = new NativeMethods.Rect()
        {
            Top = 0,
            Left = 0,
            Bottom = 23,
            Right = GetSystemMetrics(0),
        },
    };

    public ObservableCollection<NotifyIcon> TrayIcons { get; set; } = [];

    private readonly TrayService _trayService = trayService;
    private readonly ExplorerTrayService _explorerTrayService = explorerTrayService;

    public void Initialize()
    {
        try
        {
            trayDelegate = SysTrayCallback;
            iconDataDelegate = IconDataCallback;
            trayHostSizeDelegate = TrayHostSizeCallback;

            _explorerTrayService.SetSystrayCallback(trayDelegate);
            _explorerTrayService.Run();

            _trayService.SetSystrayCallback(trayDelegate);
            _trayService.SetIconDataCallback(iconDataDelegate);
            _trayService.SetTrayHostSizeCallback(trayHostSizeDelegate);
            Handle = _trayService.Initialize();
            _trayService.Run();

            // load the shell system tray objects (network, power, etc)
            shellServiceObject = new ShellServiceObject();
            shellServiceObject.Start();
        }
        catch
        {
            IsFailed = true;
        }
    }

    public void Suspend()
    {
        _trayService?.Suspend();
    }

    public void Resume()
    {
        _trayService?.Resume();
    }

    public bool Disable
    {
        get { return _trayService?.Disable ?? false; }
        set
        {
            if (_trayService != null)
                _trayService.Disable = value;
        }
    }

    #region Callbacks
    private TrayHostSizeData TrayHostSizeCallback()
    {
        return trayHostSizeData;
    }

    private IntPtr IconDataCallback(int dwMessage, uint hWnd, uint uID, Guid guidItem)
    {
        if (Disable)
            return IntPtr.Zero;

        NotifyIcon icon = TrayIcons.SingleOrDefault(ti => (guidItem != Guid.Empty && guidItem == ti.GUID) || (ti.HWnd == (IntPtr)hWnd && ti.UID == uID));

        if (icon != null)
        {
            if (dwMessage == 1)
                return MakeParam(icon.Placement.Left, icon.Placement.Top);
            else if (dwMessage == 2)
                return MakeParam(icon.Placement.Right, icon.Placement.Bottom);
        }
        else if (guidItem == new Guid(VOLUME_GUID))
        {
            if (dwMessage == 1)
                return MakeParam(defaultPlacement.Left, defaultPlacement.Top);
            else if (dwMessage == 2)
                return MakeParam(defaultPlacement.Right, defaultPlacement.Bottom);
        }

        return IntPtr.Zero;
    }

#if DEBUG
    private DateTime _lastLogged = DateTime.UtcNow;
#endif
    private bool SysTrayCallback(uint message, SafeNotifyIconData nicData)
    {
        lock (_lockObject)
        {
            if (nicData.hWnd == IntPtr.Zero || Disable)
                return false;

            NotifyIcon trayIcon = TrayIcons.FirstOrDefault(ti => ti.Equals(nicData));
            bool exists = true;

            if (trayIcon == null)
            {
                ShellLogger.Debug($"No NotifyIcon found for hWnd={nicData.hWnd}, uID={nicData.uID}, Guid={nicData.guidItem}, Title={nicData.szTip}");
#if DEBUG
                if (DateTime.UtcNow.Subtract(_lastLogged).TotalSeconds > 60)
                {
                    _lastLogged = DateTime.UtcNow;
                    StringBuilder sb = new();
                    sb.AppendLine("List of NotifyIcon actually in memory :");
                    if (TrayIcons.Count > 0)
                        foreach (NotifyIcon ni in TrayIcons)
                            sb.AppendLine($"NotifyIcon hWnd={ni.HWnd}, uID={ni.UID}, Guid={ni.GUID}, Title={ni.Title}");
                    else
                        sb.AppendLine("No notifyIcon in actual list");
                    ShellLogger.Debug(sb.ToString());
                }
#endif
                trayIcon = new(this, nicData.hWnd)
                {
                    UID = nicData.uID,
                };
                exists = false;
            }

            if ((NIM)message == NIM.NIM_ADD || (NIM)message == NIM.NIM_MODIFY)
            {
                try
                {
                    // hide icons while we are shell which require UWP support & we have a separate implementation for
                    if (nicData.guidItem == new Guid(VOLUME_GUID) && ((EnvironmentHelper.IsAppRunningAsShell && EnvironmentHelper.IsWindows10OrBetter) || GroupPolicyHelper.HideScaVolume))
                        return false;

                    // hide icons per group policy
                    if ((nicData.guidItem == new Guid(HEALTH_GUID) && GroupPolicyHelper.HideScaHealth) ||
                        (nicData.guidItem == new Guid(MEETNOW_GUID) && GroupPolicyHelper.HideScaMeetNow) ||
                        (nicData.guidItem == new Guid(NETWORK_GUID) && GroupPolicyHelper.HideScaNetwork) ||
                        (nicData.guidItem == new Guid(POWER_GUID) && GroupPolicyHelper.HideScaPower))
                    {
                        return false;
                    }

                    if ((NIF.STATE & nicData.uFlags) != 0)
                        trayIcon.IsHidden = nicData.dwState == NIS.NIS_HIDDEN;

                    if ((NIF.TIP & nicData.uFlags) != 0 && !string.IsNullOrEmpty(nicData.szTip))
                        trayIcon.Title = nicData.szTip;

                    if ((NIF.ICON & nicData.uFlags) != 0)
                    {
                        if (nicData.hIcon != IntPtr.Zero)
                        {
                            System.Windows.Media.ImageSource icon = IconImageConverter.GetImageFromHIcon(nicData.hIcon, false);

                            if (icon != null)
                            {
                                trayIcon.Icon = icon;
                            }
                            else if (icon == null && trayIcon.Icon == null)
                            {
                                // Use default only if we don't have a valid icon already
                                trayIcon.Icon = IconImageConverter.GetDefaultIcon();
                            }
                        }
                        else
                        {
                            trayIcon.Icon = null;
                        }
                    }

                    trayIcon.HWnd = nicData.hWnd;
                    trayIcon.UID = nicData.uID;

                    if ((NIF.GUID & nicData.uFlags) != 0)
                        trayIcon.GUID = nicData.guidItem;

                    if (nicData.uVersion > 0 && nicData.uVersion <= 4)
                        trayIcon.Version = nicData.uVersion;

                    if ((NIF.MESSAGE & nicData.uFlags) != 0)
                        trayIcon.CallbackMessage = nicData.uCallbackMessage;

                    if (!exists)
                    {
                        // default placement to a menu bar-like rect
                        trayIcon.Placement = defaultPlacement;

                        // set properties used for pinning
                        trayIcon.Path = ShellHelper.GetPathForHandle(trayIcon.HWnd);
                        if (PinDefaultIcons && (nicData.guidItem == new Guid(HEALTH_GUID) ||
                            nicData.guidItem == new Guid(MEETNOW_GUID) ||
                            nicData.guidItem == new Guid(NETWORK_GUID) ||
                            nicData.guidItem == new Guid(POWER_GUID) ||
                            nicData.guidItem == new Guid(VOLUME_GUID)))
                        {
                            trayIcon.IsPinned = true;
                        }

                        trayIcon.Icon ??= IconImageConverter.GetDefaultIcon();

                        TrayIcons.Add(trayIcon);

                        if (nicData.uFlags.HasFlag(NIF.INFO))
                            HandleBalloonData(nicData, trayIcon);

                        ShellLogger.Debug($"NotificationArea: Added: {trayIcon.Title} Path: {trayIcon.Path} Hidden: {trayIcon.IsHidden} GUID: {trayIcon.GUID} UID: {trayIcon.UID} Version: {trayIcon.Version} hWnd: {trayIcon.HWnd}");

                        if ((NIM)message == NIM.NIM_MODIFY)
                        {
                            // return an error to the notifyicon as we received a modify for an icon we did not yet have
                            return false;
                        }
                    }
                    else
                    {
                        if (nicData.uFlags.HasFlag(NIF.INFO))
                            HandleBalloonData(nicData, trayIcon);

                        ShellLogger.Debug($"NotificationArea: Modified: {trayIcon.Title}");
                    }
                }
                catch (Exception ex)
                {
                    ShellLogger.Error("NotificationArea: Unable to modify the icon in the collection.", ex);
                }
            }
            else if ((NIM)message == NIM.NIM_DELETE)
            {
                try
                {
                    if (trayIcon != null)
                    {
                        TrayIcons.Remove(trayIcon);
                        ShellLogger.Debug($"NotificationArea: Removed: {trayIcon.Title}");
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    ShellLogger.Error("NotificationArea: Unable to remove the icon from the collection.", ex);
                }
            }
            else if ((NIM)message == NIM.NIM_SETVERSION)
            {
                if (nicData.uVersion > 4)
                    return false;

                if (trayIcon != null)
                {
                    trayIcon.Version = nicData.uVersion;
                    ShellLogger.Debug($"NotificationArea: Modified version to {trayIcon.Version} on: {trayIcon.Title}");
                }
            }
        }
        return true;
    }
    #endregion

    private void HandleBalloonData(SafeNotifyIconData nicData, NotifyIcon notifyIcon)
    {
        if (Disable)
            return;

        if (string.IsNullOrEmpty(nicData.szInfoTitle) && (string.IsNullOrWhiteSpace(notifyIcon.Title) || string.IsNullOrWhiteSpace(notifyIcon.Path)))
            return;

        NotificationBalloon balloonInfo = new(nicData, notifyIcon);
        NotificationBalloonEventArgs args = new()
        {
            Balloon = balloonInfo,
        };

        ShellLogger.Debug($"NotificationArea: Received notification \"{balloonInfo.Title}\" for {notifyIcon.Title}");

        NotificationBalloonShown?.Invoke(this, args);

        if (!args.Handled)
            notifyIcon.TriggerNotificationBalloon(balloonInfo);
    }

    // The notification area control calls this when an icon is clicked to set the placement of its host (such as for ABM_GETTASKBARPOS usage)
    public void SetTrayHostSizeData(TrayHostSizeData data)
    {
        trayHostSizeData = data;
        _trayService?.SetTrayHostSizeData(trayHostSizeData);
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _isDisposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing && !IsFailed && trayDelegate != null)
            {
                shellServiceObject?.Dispose();
                _trayService.Dispose();
                _trayService.SetIconDataCallback(null);
                _trayService.SetTrayHostSizeCallback(null);
                iconDataDelegate = null;
                trayHostSizeDelegate = null;
            }
            _isDisposed = true;
        }
    }
    public bool IsDisposed
    {
        get { return _isDisposed; }
    }

    #endregion
}
