using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray;

/// <summary>
/// NotifyIcon class representing a notification area icon.
/// </summary>
public sealed class NotifyIcon : IEquatable<NotifyIcon>, INotifyPropertyChanged
{
    private readonly NotificationArea _notificationArea;

    /// <summary>
    /// Initializes a new instance of the TrayIcon class with no hwnd.
    /// </summary>
    public NotifyIcon(NotificationArea notificationArea) : this(notificationArea, IntPtr.Zero)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TrayIcon class with the specified hWnd.
    /// </summary>
    /// <param name="hWnd">The window handle of the icon.</param>
    public NotifyIcon(NotificationArea notificationArea, IntPtr hWnd)
    {
        ShellLogger.Debug($"Create NotifyIcon from hWnd={hWnd}");
        _notificationArea = notificationArea;
        HWnd = hWnd;
        MissedNotifications = [];
        _lastLClick = new Stopwatch();
        _lastLClick.Start();
        _lastRClick = new Stopwatch();
        _lastRClick.Start();
    }

    private ImageSource _icon;

    /// <summary>
    /// Gets or sets the Icon's image.
    /// </summary>
    public ImageSource Icon
    {
        get
        {
            return _icon;
        }
        set
        {
            _icon = value;
            OnPropertyChanged();
        }
    }

    private string _title;

    /// <summary>
    /// Gets or sets the Icon's title (tool tip).
    /// </summary>
    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the path to the application that created the icon.
    /// </summary>
    public string Path { get; set; }

    private bool _isPinned;

    /// <summary>
    /// Gets or sets whether or not the icon is pinned.
    /// </summary>
    public bool IsPinned
    {
        get
        {
            return _isPinned;
        }
        set
        {
            _isPinned = value;
            OnPropertyChanged();
        }
    }

    private bool _isHidden;

    /// <summary>
    /// Gets or sets whether or not the icon is hidden.
    /// </summary>
    public bool IsHidden
    {
        get
        {
            return _isHidden;
        }
        set
        {
            _isHidden = value;
            OnPropertyChanged();
        }
    }

    private int _pinOrder;

    /// <summary>
    /// Gets or sets the order index of the item in the pinned icons
    /// </summary>
    public int PinOrder
    {
        get
        {
            return _pinOrder;
        }
        set
        {
            _pinOrder = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the owners handle.
    /// </summary>
    public IntPtr HWnd { get; set; }

    /// <summary>
    /// Gets or sets the callback message id.
    /// </summary>
    public uint CallbackMessage { get; set; }

    /// <summary>
    /// Gets or sets the UID of the Icon.
    /// </summary>
    public uint UID { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the Icon.
    /// </summary>
    public Guid GUID { get; set; }

    public uint Version { get; set; }

    public Rect Placement { get; set; }

    #region Balloon Notifications

    public ObservableCollection<NotificationBalloon> MissedNotifications { get; set; }

    public event EventHandler<NotificationBalloonEventArgs> NotificationBalloonShown;

    internal void TriggerNotificationBalloon(NotificationBalloon balloonInfo)
    {
        NotificationBalloonEventArgs args = new()
        {
            Balloon = balloonInfo,
        };

        NotificationBalloonShown?.Invoke(this, args);

        if (!args.Handled)
        {
            MissedNotifications.Add(balloonInfo);
        }
    }

    #endregion

    #region Mouse events

    private readonly Stopwatch _lastLClick;
    private readonly Stopwatch _lastRClick;

    public void IconMouseEnter(uint mouse)
    {
        if (RemoveIfInvalid())
            return;

        SendMessage((uint)WM.MOUSEHOVER, mouse);

        if (Version > 3)
            SendMessage((uint)NIN.POPUPOPEN, mouse);
    }

    public void IconMouseLeave(uint mouse)
    {
        if (RemoveIfInvalid())
            return;

        SendMessage((uint)WM.MOUSELEAVE, mouse);

        if (Version > 3)
            SendMessage((uint)NIN.POPUPCLOSE, mouse);
    }

    public void IconMouseMove(uint mouse)
    {
        if (RemoveIfInvalid())
            return;

        SendMessage((uint)WM.MOUSEMOVE, mouse);
    }

    public void IconMouseDown(MouseButton button, uint mouse, int doubleClickTime)
    {
        // allow notify icon to focus so that menus go away after clicking outside
        GetWindowThreadProcessId(HWnd, out uint procId);
        AllowSetForegroundWindow(procId);

        if (button == MouseButton.Left)
        {
            if (HandleClickOverride(false))
                return;

            if (_lastLClick.ElapsedMilliseconds <= doubleClickTime)
                SendMessage((uint)WM.LBUTTONDBLCLK, mouse);
            else
                SendMessage((uint)WM.LBUTTONDOWN, mouse);

            _lastLClick.Restart();
        }
        else if (button == MouseButton.Right)
        {
            if (_lastRClick.ElapsedMilliseconds <= doubleClickTime)
                SendMessage((uint)WM.RBUTTONDBLCLK, mouse);
            else
                SendMessage((uint)WM.RBUTTONDOWN, mouse);

            _lastRClick.Restart();
        }
    }

#pragma warning disable IDE0060
    public void IconMouseUp(MouseButton button, uint mouse, int doubleClickTime)
    {
        ShellLogger.Debug($"NotifyIcon: {button} mouse button clicked: {Title}");

        if (button == MouseButton.Left)
        {
            if (HandleClickOverride(true))
                return;

            SendMessage((uint)WM.LBUTTONUP, mouse);

            // This is documented as version 4, but Explorer does this for version 3 as well
            if (Version >= 3)
                SendMessage((uint)NIN.SELECT, mouse);

            _lastLClick.Restart();
        }
        else if (button == MouseButton.Right)
        {
            SendMessage((uint)WM.RBUTTONUP, mouse);

            // This is documented as version 4, but Explorer does this for version 3 as well
            if (Version >= 3)
                SendMessage((uint)WM.CONTEXTMENU, mouse);

            _lastRClick.Restart();
        }
    }
#pragma warning restore IDE0060

    internal bool SendMessage(uint message, uint mouse)
    {
        return SendNotifyMessage(HWnd, CallbackMessage, GetMessageWParam(mouse), message | (GetMessageHiWord() << 16));
    }

    private uint GetMessageHiWord()
    {
        if (Version > 3)
            return UID;

        return 0;
    }

    private uint GetMessageWParam(uint mouse)
    {
        if (Version > 3)
            return mouse;

        return UID;
    }

    private bool RemoveIfInvalid()
    {
        if (!IsWindow(HWnd))
        {
            ShellLogger.Debug($"Invalid hWnd, Remove systray icon of {Title}");
            _notificationArea.TrayIcons.Remove(this);
            return true;
        }

        return false;
    }

    private bool HandleClickOverride(bool performAction)
    {
        if (NotificationArea.Win11ActionCenterIcons.Contains(GUID.ToString()) && EnvironmentHelper.IsWindows11OrBetter)
        {
            if (performAction)
                ShellHelper.ShowActionCenter();

            return true;
        }

        return false;
    }
    #endregion

    #region IEquatable<NotifyIcon> Members

    /// <summary>
    /// Checks the equality of the icon based on the hWnd and uID;
    /// </summary>
    /// <param name="other">The other NotifyIcon to compare to.</param>
    /// <returns>Indication of equality.</returns>
    public bool Equals(NotifyIcon other)
    {
        if (other == null)
            return false;

        return (HWnd.Equals(other.HWnd) && UID.Equals(other.UID)) || (other.GUID != Guid.Empty && GUID.Equals(other.GUID));
    }

    public bool Equals(SafeNotifyIconData other)
    {
        return (HWnd.Equals(other.hWnd) && UID.Equals(other.uID)) || (other.guidItem != Guid.Empty && GUID.Equals(other.guidItem));
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName()] string PropertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    #endregion
}
