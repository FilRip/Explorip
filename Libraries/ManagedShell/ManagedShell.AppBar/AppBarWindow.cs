﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

namespace ManagedShell.AppBar;

public class AppBarWindow : Window, INotifyPropertyChanged
{
    protected readonly AppBarManager _appBarManager;
    protected readonly ExplorerHelper _explorerHelper;
    protected readonly FullScreenHelper _fullScreenHelper;
    public AppBarScreen Screen { get; set; }
    public double DpiScale { get; set; } = 1.0;
    protected bool ProcessScreenChanges = true;

    // Window properties
    protected WindowInteropHelper windowInteropHelper;
    private bool IsRaising;
    public IntPtr Handle { get; set; }
    public bool AllowClose { get; set; }
    public bool IsClosing { get; set; }
    protected double DesiredHeight;
    protected double DesiredWidth;
    private bool EnableBlur;

    // AppBar properties
    private int AppBarMessageId = -1;

    private AppBarEdge _appBarEdge;
    public AppBarEdge AppBarEdge
    {
        get
        {
            return _appBarEdge;
        }
        set
        {
            _appBarEdge = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Orientation));
        }
    }
    protected internal bool EnableAppBar = true;
    protected internal bool RequiresScreenEdge;

    public Orientation Orientation
    {
        get => (AppBarEdge == AppBarEdge.Left || AppBarEdge == AppBarEdge.Right) ? Orientation.Vertical : Orientation.Horizontal;
    }

    public AppBarWindow(AppBarManager appBarManager, ExplorerHelper explorerHelper, FullScreenHelper fullScreenHelper, AppBarScreen screen, AppBarEdge edge, double size)
    {
        _explorerHelper = explorerHelper;
        _fullScreenHelper = fullScreenHelper;
        _appBarManager = appBarManager;

        Closing += OnClosing;
        SourceInitialized += OnSourceInitialized;

        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Title = "";
        Topmost = true;
        UseLayoutRounding = true;
        WindowStyle = WindowStyle.None;

        Screen = screen;
        AppBarEdge = edge;

        if (Orientation == Orientation.Vertical)
            DesiredWidth = size;
        else
            DesiredHeight = size;
    }

    #region Events
    protected virtual void OnSourceInitialized(object sender, EventArgs e)
    {
        // set up helper and get handle
        windowInteropHelper = new WindowInteropHelper(this);
        Handle = windowInteropHelper.EnsureHandle();

        // set up window procedure
        HwndSource source = HwndSource.FromHwnd(Handle);
        source.AddHook(WndProc);

        // set initial DPI. We do it here so that we get the correct value when DPI has changed since initial user logon to the system.
        if (Screen.Primary)
        {
            DpiHelper.DpiScale = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice.M11;
        }

        DpiScale = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
        DpiScale = Screen.DpiScale;

        SetPosition();

        if (EnvironmentHelper.IsAppRunningAsShell)
        {
            // set position again, on a delay, in case one display has a different DPI. for some reason the system overrides us if we don't wait
            DelaySetPosition();
        }

        RegisterAppBar();

        // hide from alt-tab etc
        WindowHelper.HideWindowFromTasks(Handle);

        // register for full-screen notifications
        _fullScreenHelper.FullScreenApps.CollectionChanged += FullScreenApps_CollectionChanged;
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        IsClosing = true;

        CustomClosing();

        if (AllowClose)
        {
            UnregisterAppBar();

            // unregister full-screen notifications
            _fullScreenHelper.FullScreenApps.CollectionChanged -= FullScreenApps_CollectionChanged;
        }
        else
        {
            IsClosing = false;
            e.Cancel = true;
        }
    }

    private void FullScreenApps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        bool found = false;

        foreach (ScreenInfo app in _fullScreenHelper.FullScreenApps.Select(item => item.Screen))
        {
            if (app.DeviceName == Screen.DeviceName || app.IsVirtualScreen)
            {
                // we need to not be on top now
                found = true;
                break;
            }
        }

        if (found && Topmost)
        {
            SetFullScreenMode(true);
        }
        else if (!found && !Topmost)
        {
            SetFullScreenMode(false);
        }
    }

    protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == AppBarMessageId && AppBarMessageId != -1)
        {
            switch ((NativeMethods.AppBarNotifications)wParam.ToInt32())
            {
                case NativeMethods.AppBarNotifications.PosChanged:
                    if (Orientation == Orientation.Vertical)
                    {
                        _appBarManager.ABSetPos(this, DesiredWidth * DpiScale, ActualHeight * DpiScale, AppBarEdge);
                    }
                    else
                    {
                        _appBarManager.ABSetPos(this, ActualWidth * DpiScale, DesiredHeight * DpiScale, AppBarEdge);
                    }
                    break;

                case NativeMethods.AppBarNotifications.WindowArrange:
                    if ((int)lParam != 0) // before
                    {
                        Visibility = Visibility.Collapsed;
                    }
                    else // after
                    {
                        Visibility = Visibility.Visible;
                    }

                    break;
            }
            handled = true;
        }
        else if (msg == (int)NativeMethods.WM.ACTIVATE && EnableAppBar && !EnvironmentHelper.IsAppRunningAsShell && !AllowClose)
        {
            _appBarManager.AppBarActivate(hwnd);
        }
        else if (msg == (int)NativeMethods.WM.WINDOWPOSCHANGING)
        {
            // Extract the WINDOWPOS structure corresponding to this message
            NativeMethods.WindowPos wndPos = NativeMethods.WindowPos.FromMessage(lParam);

            // Determine if the z-order is changing (absence of SWP_NOZORDER flag)
            // If we are intentionally trying to become topmost, make it so
            if (IsRaising && (wndPos.flags & NativeMethods.SWP.SWP_NOZORDER) == 0)
            {
                // Sometimes Windows thinks we shouldn't go topmost, so poke here to make it happen.
                wndPos.hwndInsertAfter = (IntPtr)NativeMethods.WindowZOrder.HWND_TOPMOST;
                wndPos.UpdateMessage(lParam);
            }
        }
        else if (msg == (int)NativeMethods.WM.WINDOWPOSCHANGED && EnableAppBar && !EnvironmentHelper.IsAppRunningAsShell && !AllowClose && !_positionAlreadyUnderChanged)
        {
            _appBarManager.AppBarWindowPosChanged(hwnd);
        }
        else if ((msg == (int)NativeMethods.WM.DPICHANGED) && !_positionAlreadyUnderChanged)
        {
            if (Screen.Primary)
            {
                DpiHelper.DpiScale = (wParam.ToInt32() & 0xFFFF) / 96d;
            }

            DpiScale = (wParam.ToInt32() & 0xFFFF) / 96d;
            Screen.ChangeDpi();

            ProcessScreenChange(ScreenSetupReason.DpiChange);
        }
        else if (msg == (int)NativeMethods.WM.DISPLAYCHANGE)
        {
            ProcessScreenChange(ScreenSetupReason.DisplayChange);
            handled = true;
        }
        else if (msg == (int)NativeMethods.WM.DEVICECHANGE && (int)wParam == 0x0007)
        {
            ProcessScreenChange(ScreenSetupReason.DeviceChange);
            handled = true;
        }
        else if (msg == (int)NativeMethods.WM.DWMCOMPOSITIONCHANGED)
        {
            ProcessScreenChange(ScreenSetupReason.DwmChange);
            handled = true;
        }

        return IntPtr.Zero;
    }
    #endregion

    #region Helpers
    protected void DelaySetPosition()
    {
        // delay changing things when we are shell. it seems that explorer AppBars do this too.
        // if we don't, the system moves things to bad places
        DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(0.1) };
        timer.Start();
        timer.Tick += (sender1, args) =>
        {
            SetPosition();
            timer.Stop();
        };
    }

    public void SetScreenPosition()
    {
        // set our position if running as shell, otherwise let AppBar do the work
        if (EnvironmentHelper.IsAppRunningAsShell || !EnableAppBar)
        {
            DelaySetPosition();
        }
        else
        {
            if (Orientation == Orientation.Vertical)
            {
                _appBarManager.ABSetPos(this, DesiredWidth * DpiScale, ActualHeight * DpiScale, AppBarEdge);
            }
            else
            {
                _appBarManager.ABSetPos(this, ActualWidth * DpiScale, DesiredHeight * DpiScale, AppBarEdge);
            }
        }
    }

    private bool _positionAlreadyUnderChanged;

    internal void SetAppBarPosition(NativeMethods.Rect rect)
    {
        try
        {
            _positionAlreadyUnderChanged = true;
            Top = rect.Top / DpiScale;
            Left = rect.Left / DpiScale;
            Width = (rect.Right - rect.Left) / DpiScale;
            Height = (rect.Bottom - rect.Top) / DpiScale;
        }
        catch (Exception) { /* Ignore errors */ }
        finally { _positionAlreadyUnderChanged = false; }
    }

    private void ProcessScreenChange(ScreenSetupReason reason)
    {
        // process screen changes if we are on the primary display and the designated window
        // (or any display in the case of a DPI change, since only the changed display receives that message and not all windows receive it reliably)
        // suppress this if we are shutting down (which can trigger this method on multi-dpi setups due to window movements)
        if (((Screen.Primary && ProcessScreenChanges) || reason == ScreenSetupReason.DpiChange) && !AllowClose)
        {
            SetScreenProperties(reason);
        }
    }

    private void SetFullScreenMode(bool entering)
    {
        if (entering)
        {
            ShellLogger.Debug($"AppBarWindow: {Name} on {Screen.DeviceName} conceding to full-screen app");

            Topmost = false;
            WindowHelper.ShowWindowBottomMost(Handle);
            EnterFullScreen?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            ShellLogger.Debug($"AppBarWindow: {Name} on {Screen.DeviceName} returning to normal state");

            IsRaising = true;
            Topmost = true;
            WindowHelper.ShowWindowTopMost(Handle);
            IsRaising = false;
            ExitFullScreen?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler ExitFullScreen;
    public event EventHandler EnterFullScreen;

    protected void SetBlur(bool enable)
    {
        if (EnableBlur != enable && Handle != IntPtr.Zero && AllowsTransparency)
        {
            EnableBlur = enable;
            WindowHelper.SetWindowBlur(Handle, enable);
        }
    }

    protected void RegisterAppBar()
    {
        if (!EnableAppBar || _appBarManager.AppBars.Contains(this))
        {
            return;
        }

        if (Orientation == Orientation.Vertical)
        {
            AppBarMessageId = _appBarManager.RegisterBar(this, DesiredWidth * DpiScale, ActualHeight * DpiScale, AppBarEdge);
        }
        else
        {
            AppBarMessageId = _appBarManager.RegisterBar(this, ActualWidth * DpiScale, DesiredHeight * DpiScale, AppBarEdge);
        }
    }

    protected void UnregisterAppBar()
    {
        if (!_appBarManager.AppBars.Contains(this))
        {
            return;
        }

        if (Orientation == Orientation.Vertical)
        {
            _appBarManager.RegisterBar(this, DesiredWidth * DpiScale, ActualHeight * DpiScale);
        }
        else
        {
            _appBarManager.RegisterBar(this, ActualWidth * DpiScale, DesiredHeight * DpiScale);
        }
    }
    #endregion

    #region Virtual methods
    public virtual void AfterAppBarPos(bool isSameCoords, NativeMethods.Rect rect)
    {
        if (!isSameCoords)
        {
            DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(0.1) };
            timer.Start();
            timer.Tick += (sender1, args) =>
            {
                // set position again, since WPF may have overridden the original change from AppBarHelper
                SetAppBarPosition(rect);

                timer.Stop();
            };
        }
    }

    protected virtual void CustomClosing() { }

    protected virtual void SetScreenProperties(ScreenSetupReason reason)
    {
        FullScreenHelper.NotifyScreensChanged();

        SetScreenPosition();
    }

    public virtual void SetPosition()
    {
        double edgeOffset = 0;

        if (!RequiresScreenEdge)
        {
            edgeOffset = _appBarManager.GetAppBarEdgeWindowsHeight(AppBarEdge, Screen);
        }

        if (Orientation == Orientation.Vertical)
        {
            Top = Screen.Bounds.Top / DpiScale;
            Height = Screen.Bounds.Height / DpiScale;
            Width = DesiredWidth;

            if (AppBarEdge == AppBarEdge.Left)
            {
                Left = Screen.Bounds.Left / DpiScale + edgeOffset;
            }
            else
            {
                Left = Screen.Bounds.Right / DpiScale - Width - edgeOffset;
            }
        }
        else
        {
            Left = Screen.Bounds.Left / DpiScale;
            Width = Screen.Bounds.Width / DpiScale;
            Height = DesiredHeight;

            if (AppBarEdge == AppBarEdge.Top)
            {
                Top = (Screen.Bounds.Top / DpiScale) + edgeOffset;
            }
            else
            {
                Top = Screen.Bounds.Bottom / DpiScale - Height - edgeOffset;
            }
        }


        if (EnvironmentHelper.IsAppRunningAsShell)
        {
            _appBarManager.SetWorkArea(Screen);
        }
    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName()] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
