using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

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

    private void TasksService_Event(object sender, EventArgs e)
    {
        updateFullScreenWindows();
    }

    private FullScreenApp GetFullScreenApp(IntPtr hWnd)
    {
        int style = GetWindowLong(hWnd, GWL.GWL_STYLE);
        Rect rect;

        if ((((int)WindowStyles.WS_CAPTION | (int)WindowStyles.WS_THICKFRAME) & style) == ((int)WindowStyles.WS_CAPTION | (int)WindowStyles.WS_THICKFRAME))
        {
            GetClientRect(hWnd, out rect);
            MapWindowPoints(hWnd, IntPtr.Zero, ref rect, 2);
        }
        else
        {
            GetWindowRect(hWnd, out rect);
        }

        List<ScreenInfo> allScreens = [.. Screen.AllScreens.Select(ScreenInfo.Create)];
        if (allScreens.Count > 1) allScreens.Add(ScreenInfo.CreateVirtualScreen());

        // check if this is a fullscreen app
        foreach (ScreenInfo screen in allScreens)
        {
            if (rect.Top == screen.Bounds.Top && rect.Left == screen.Bounds.Left &&
                rect.Bottom == screen.Bounds.Bottom && rect.Right == screen.Bounds.Right)
            {
                // make sure this is not us
                GetWindowThreadProcessId(hWnd, out uint hwndProcId);
                if (hwndProcId == GetCurrentProcessId())
                {
                    return null;
                }

                // make sure this is fullscreen-able
                if (!IsWindow(hWnd) || !IsWindowVisible(hWnd) || IsIconic(hWnd))
                {
                    return null;
                }

                // make sure this is not the shell desktop
                StringBuilder cName = new(256);
                GetClassName(hWnd, cName, cName.Capacity);
                if (cName.ToString() == "Progman" || cName.ToString() == "WorkerW")
                {
                    return null;
                }

                // make sure this is not a cloaked window
                if (EnvironmentHelper.IsWindows8OrBetter)
                {
                    int cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));
                    DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out uint cloaked, cbSize);
                    if (cloaked > 0)
                    {
                        return null;
                    }
                }

                // this is a full screen app on this screen
                return new FullScreenApp { HWnd = hWnd, Screen = screen, Rect = rect };
            }
        }

        return null;
    }

    private static void ResetScreenCache()
    {
        // use reflection to empty screens cache
#pragma warning disable S3011
        typeof(Screen).GetField("screens", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).SetValue(null, null);
#pragma warning restore S3011
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
                fullscreenCheck.Stop();
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
