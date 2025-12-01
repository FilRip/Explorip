using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.WindowsTasks;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.AppBar;

public sealed class FullScreenHelper : IDisposable
{
    private readonly TasksService _tasksService;
    private bool disposableValue;

    public ObservableCollection<FullScreenApp> FullScreenApps { get; set; } = [];
    public ObservableCollection<FullScreenApp> InactiveFullScreenApps { get; set; } = [];

    public bool Disable { get; set; }

    public FullScreenHelper(TasksService tasksService)
    {
        _tasksService = tasksService;

        // On Windows 8 and newer, TasksService will tell us when windows enter and exit full screen
        _tasksService.FullScreenChanged += TasksService_FullScreenChanged;
        _tasksService.MonitorChanged += TasksService_Event;
        _tasksService.DesktopActivated += TasksService_Event;
        _tasksService.WindowActivated += TasksService_Event;
    }

    private void TasksService_Event(object sender, WindowEventArgs e)
    {
        if (Disable)
            return;
        UpdateFullScreenWindows();
    }

    private void TasksService_FullScreenChanged(object sender, FullScreenEventArgs e)
    {
        if (Disable)
            return;

        if (InactiveFullScreenApps.Count > 0 && InactiveFullScreenApps.Any(app => app.HWnd == e.Handle))
        {
            // If this window is in the inactive list, remove it--the message that triggered this event takes precedence
            InactiveFullScreenApps.Remove(InactiveFullScreenApps.First(app => app.HWnd == e.Handle));
        }

        if (FullScreenApps.Any(app => app.HWnd == e.Handle) == e.IsEntering)
        {
            if (e.IsEntering)
            {
                FullScreenApp existingApp = FullScreenApps.First(app => app.HWnd == e.Handle);
                if (!existingApp.FromTasksService)
                {
                    // Grant this app TasksService treatment
                    existingApp.FromTasksService = true;
                }
            }
            return;
        }

        if (e.IsEntering)
        {
            // When TasksService gives us a full-screen window handle, trust that it is full-screen in terms of bounds
            FullScreenApp appNew = GetFullScreenApp(e.Handle, true);

            if (appNew != null)
            {
                ShellLogger.Debug($"FullScreenHelper: Adding full screen app from TasksService {appNew.HWnd} ({appNew.Title})");
                FullScreenApps.Add(appNew);
            }
        }
        else
        {
            foreach (FullScreenApp app in FullScreenApps)
            {
                if (app.HWnd == e.Handle)
                {
                    ShellLogger.Debug($"FullScreenHelper: Removing full screen app from TasksService {app.HWnd} ({app.Title})");
                    FullScreenApps.Remove(app);
                    break;
                }
            }
        }
    }

    private void UpdateFullScreenWindows()
    {
        IntPtr hWnd = GetForegroundWindow();

        List<FullScreenApp> removeApps = [];
        List<FullScreenApp> removeInactiveApps = [];
        bool skipAdd = false;

        // first check if this window is already in our list. if so, remove it if necessary
        foreach (FullScreenApp app in FullScreenApps)
        {
            FullScreenApp appCurrentState = GetFullScreenApp(app.HWnd, app.FromTasksService);

            if (appCurrentState != null)
            {
                // App is still full-screen
                if (app.HWnd == hWnd)
                {
                    // App is still foreground
                    if (app.Screen.DeviceName != appCurrentState.Screen.DeviceName)
                    {
                        // The app moved to another monitor
                        // Remove and add back to collection to trigger change events
                        // This will be added back immediately because it is foreground
                        ShellLogger.Debug($"FullScreenHelper: Monitor changed for full screen app {app.HWnd} ({app.Title})");
                    }
                    else
                    {
                        // Still same screen, do nothing
                        skipAdd = true;
                        continue;
                    }
                }
                else if (Screen.FromHandle(hWnd).DeviceName != appCurrentState.Screen.DeviceName)
                {
                    // if the full-screen window is no longer foreground, keep it
                    // as long as the foreground window is on a different screen.
                    continue;
                }
                else
                {
                    // Still full screen but no longer active
                    if ((GetWindowLong(hWnd, EGetWindowLong.GWL_EXSTYLE) & (int)ExtendedWindowStyles.WS_EX_TOPMOST) == (int)ExtendedWindowStyles.WS_EX_TOPMOST)
                    {
                        // If the new foreground window is a topmost window, don't consider this full-screen app inactive
                        continue;
                    }
                    ShellLogger.Debug($"FullScreenHelper: Inactive full screen app {app.HWnd} ({app.Title})");
                }
                InactiveFullScreenApps.Add(app);
            }

            removeApps.Add(app);
        }

        // remove any changed windows we found
        if (removeApps.Count > 0)
        {
            foreach (FullScreenApp existingApp in removeApps)
            {
                ShellLogger.Debug($"FullScreenHelper: Removing full screen app {existingApp.HWnd} ({existingApp.Title})");
                FullScreenApps.Remove(existingApp);
            }
        }

        // clean up any inactive windows that are no longer full-screen
        if (InactiveFullScreenApps.Count > 0)
        {
            foreach (FullScreenApp app in InactiveFullScreenApps)
            {
                FullScreenApp appCurrentState = GetFullScreenApp(app.HWnd, app.FromTasksService);
                if (appCurrentState == null)
                {
                    // No longer a full-screen window
                    removeInactiveApps.Add(app);
                }
                else if (appCurrentState.Screen.DeviceName != app.Screen.DeviceName)
                {
                    // The app moved to another monitor while inactive
                    app.Screen = appCurrentState.Screen;
                }
            }
        }

        // remove any changed inactive windows we found
        if (removeInactiveApps.Count > 0)
        {
            foreach (FullScreenApp existingApp in removeInactiveApps)
            {
                ShellLogger.Debug($"FullScreenHelper: Removing inactive full screen app {existingApp.HWnd} ({existingApp.Title})");
                InactiveFullScreenApps.Remove(existingApp);
            }
        }

        // check if this is a new full screen app
        if (!skipAdd)
        {
            FullScreenApp appAdd;
            bool wasInactive = false;
            if (InactiveFullScreenApps.Count > 0 && InactiveFullScreenApps.Any(app => app.HWnd == hWnd))
            {
                // This is a previously-active full-screen app that became active again.
                wasInactive = true;
                appAdd = InactiveFullScreenApps.First(app => app.HWnd == hWnd);
            }
            else
            {
                appAdd = GetFullScreenApp(hWnd);
            }

            if (appAdd != null)
            {
                ShellLogger.Debug($"FullScreenHelper: Adding{(wasInactive ? " reactivated" : "")} full screen app {appAdd.HWnd} ({appAdd.Title})");
                FullScreenApps.Add(appAdd);
                if (wasInactive)
                {
                    InactiveFullScreenApps.Remove(appAdd);
                }
            }
        }
    }

    private FullScreenApp GetFullScreenApp(IntPtr hWnd, bool fromTasksService = false)
    {
        ScreenInfo screenInfo = null;
        Rect rect = GetEffectiveWindowRect(hWnd);

        if (!fromTasksService)
        {
            List<ScreenInfo> allScreens = [.. Screen.AllScreens.Select(ScreenInfo.Create)];
            if (allScreens.Count > 1) allScreens.Add(ScreenInfo.CreateVirtualScreen());

            foreach (var screen in allScreens)
            {
                if (rect.Top == screen.Bounds.Top && rect.Left == screen.Bounds.Left &&
                    rect.Bottom == screen.Bounds.Bottom && rect.Right == screen.Bounds.Right)
                {
                    screenInfo = screen;
                    break;
                }
            }

            if (screenInfo == null)
            {
                // If the window rect does not match any screen's bounds, it's not full screen
                return null;
            }
        }

        ApplicationWindow win = new(_tasksService, hWnd);
        if (!CanFullScreen(win))
            return null;

        screenInfo ??= ScreenInfo.Create(Screen.FromHandle(hWnd));

        // this is a full screen app
        return new FullScreenApp { HWnd = hWnd, Screen = screenInfo, Rect = rect, Title = win.Title, FromTasksService = fromTasksService };
    }

    private static Rect GetEffectiveWindowRect(IntPtr hWnd)
    {
        int style = GetWindowLong(hWnd, EGetWindowLong.GWL_STYLE);
        Rect rect;

        if ((((int)WindowStyles.WS_CAPTION | (int)WindowStyles.WS_THICKFRAME) & style) == ((int)WindowStyles.WS_CAPTION | (int)WindowStyles.WS_THICKFRAME) ||
            (((uint)WindowStyles.WS_POPUP | (uint)WindowStyles.WS_THICKFRAME) & style) == ((uint)WindowStyles.WS_POPUP | (uint)WindowStyles.WS_THICKFRAME) ||
            (((uint)WindowStyles.WS_POPUP | (uint)WindowStyles.WS_BORDER) & style) == ((uint)WindowStyles.WS_POPUP | (uint)WindowStyles.WS_BORDER))
        {
            GetClientRect(hWnd, out rect);
            MapWindowPoints(hWnd, IntPtr.Zero, ref rect, 2);
        }
        else
            GetWindowRect(hWnd, out rect);

        return rect;
    }

    private static bool CanFullScreen(ApplicationWindow window)
    {
        // make sure this is not us
        GetWindowThreadProcessId(window.ListWindows[0], out uint hwndProcId);
        if (hwndProcId == GetCurrentProcessId())
            return false;

        // make sure this is fullscreen-able
        if (!IsWindow(window.ListWindows[0]) || !IsWindowVisible(window.ListWindows[0]) || IsIconic(window.ListWindows[0]))
            return false;

        // Make sure this isn't explicitly marked as being non-rude
        IntPtr isNonRudeHwnd = GetProp(window.ListWindows[0], "NonRudeHWND");
        if (isNonRudeHwnd != IntPtr.Zero)
            return false;

        // make sure this is not a cloaked window
        if (EnvironmentHelper.IsWindows8OrBetter)
        {
            int cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));
            DwmGetWindowAttribute(window.ListWindows[0], DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out uint cloaked, cbSize);
            if (cloaked > 0)
                return false;
        }

        // make sure this is not a transparent window
        int styles = window.ExtendedWindowStyles;
        if ((styles & (int)ExtendedWindowStyles.WS_EX_LAYERED) != 0 && ((styles & (int)ExtendedWindowStyles.WS_EX_TRANSPARENT) != 0 || (styles & (int)ExtendedWindowStyles.WS_EX_NOACTIVATE) != 0))
            return false;

        return true;
    }

    private static void ResetScreenCache()
    {
        // use reflection to empty screens cache
#pragma warning disable IDE0079
#pragma warning disable S3011
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
#pragma warning restore S3011
#pragma warning restore IDE0079
        var fi = typeof(Screen).GetField("screens", flags) ?? typeof(Screen).GetField("s_screens", flags);

        if (fi == null)
        {
            ShellLogger.Warning("FullScreenHelper: Unable to reset screens cache");
            return;
        }

        fi.SetValue(null, null);
    }

    public static void NotifyScreensChanged()
    {
        ResetScreenCache();
    }

    public bool IsDisposed
    {
        get { return disposableValue; }
    }

    private void Dispose(bool disposing)
    {
        if (!disposableValue)
        {
            if (disposing)
            {
                _tasksService.FullScreenChanged -= TasksService_FullScreenChanged;
                _tasksService.MonitorChanged -= TasksService_Event;
                _tasksService.DesktopActivated -= TasksService_Event;
                _tasksService.WindowActivated -= TasksService_Event;
            }
            disposableValue = true;
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
