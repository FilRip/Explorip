using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray;

public class TrayService : IDisposable
{
    private const string NotifyWndClass = "TrayNotifyWnd";

    private IconDataDelegate iconDataDelegate;
    private TrayHostSizeDelegate trayHostSizeDelegate;
    private SystrayDelegate trayDelegate;
    private WndProcDelegate wndProcDelegate;

    private IntPtr HwndTray;
    private IntPtr HwndNotify;
    private IntPtr HwndFwd;
    private readonly IntPtr hInstance = Marshal.GetHINSTANCE(typeof(TrayService).Module);

    private readonly DispatcherTimer trayMonitor = new(DispatcherPriority.Background);

    public TrayService()
    {
        SetupTrayMonitor();
    }

    #region Set callbacks
    internal void SetSystrayCallback(SystrayDelegate theDelegate)
    {
        trayDelegate = theDelegate;
    }

    internal void SetIconDataCallback(IconDataDelegate theDelegate)
    {
        iconDataDelegate = theDelegate;
    }

    internal void SetTrayHostSizeCallback(TrayHostSizeDelegate theDelegate)
    {
        trayHostSizeDelegate = theDelegate;
    }
    #endregion

    internal IntPtr Initialize()
    {
        if (HwndTray != IntPtr.Zero)
            return HwndTray;

        DestroyWindows();

        wndProcDelegate = WndProc;

        RegisterTrayWnd();
        RegisterNotifyWnd();

        return HwndTray;
    }

    /// <summary>
    /// Starts the system tray listener (send the TaskbarCreated message).
    /// </summary>
    internal void Run()
    {
        if (HwndTray != IntPtr.Zero)
        {
            Resume();
            SendTaskbarCreated();
        }
    }

    internal void Suspend()
    {
        // if we go beneath another tray, it will receive messages
        if (HwndTray != IntPtr.Zero)
        {
            trayMonitor.Stop();
            SetWindowPos(HwndTray, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0,
                SWP.SWP_NOMOVE | SWP.SWP_NOACTIVATE |
                SWP.SWP_NOSIZE);
        }
    }

    internal void Resume()
    {
        // if we are above another tray, we will receive messages
        if (HwndTray != IntPtr.Zero)
        {
            SetWindowsTrayBottommost();
            MakeTrayTopmost();
            trayMonitor.Start();
        }
    }

    internal void SetTrayHostSizeData(TrayHostSizeData data)
    {
        ShellLogger.Debug("SetTrayHostSizeData");

        if (HwndTray != IntPtr.Zero)
            SetWindowPos(HwndTray, IntPtr.Zero, data.rc.Left, data.rc.Top, data.rc.Width, data.rc.Height, SWP.SWP_NOACTIVATE | SWP.SWP_NOZORDER);

        if (HwndNotify != IntPtr.Zero)
            SetWindowPos(HwndNotify, IntPtr.Zero, data.rc.Left, data.rc.Top, data.rc.Width, data.rc.Height, SWP.SWP_NOACTIVATE | SWP.SWP_NOZORDER);
    }

    private static void SendTaskbarCreated()
    {
        int msg = RegisterWindowMessage("TaskbarCreated");

        if (msg > 0)
        {
            ShellLogger.Debug("TrayService: Sending TaskbarCreated message");
            SendNotifyMessage(HWND_BROADCAST,
                (uint)msg, UIntPtr.Zero, IntPtr.Zero);
        }
    }

    private void DestroyWindows()
    {
        if (HwndNotify != IntPtr.Zero)
        {
            DestroyWindow(HwndNotify);
            UnregisterClass(NotifyWndClass, hInstance);
            ShellLogger.Debug($"TrayService: Unregistered {NotifyWndClass}");
        }

        if (HwndTray != IntPtr.Zero)
        {
            DestroyWindow(HwndTray);
            UnregisterClass(WindowHelper.TrayWndClass, hInstance);
            ShellLogger.Debug($"TrayService: Unregistered {WindowHelper.TrayWndClass}");
        }
    }

    public bool Disable { get; set; }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private bool _isDisposed;
    public bool IsDisposed
    {
        get { return _isDisposed; }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                trayMonitor.Stop();
                DestroyWindows();

                if (!EnvironmentHelper.IsAppRunningAsShell)
                    SendTaskbarCreated();
            }
            _isDisposed = true;
        }
    }

    #endregion

    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        if (!Disable)
        {
            switch ((WM)msg)
            {
                case WM.COPYDATA:
                    if (lParam == IntPtr.Zero)
                    {
                        ShellLogger.Debug("TrayService: CopyData is null");
                        break;
                    }

                    CopyDataStruct copyData =
                        (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));

                    switch ((int)copyData.dwData)
                    {
                        case 0:
                            // AppBar message
                            if (Marshal.SizeOf(typeof(AppBarMsgDataV3)) == copyData.cbData)
                            {
                                AppBarMsgDataV3 amd = (AppBarMsgDataV3)Marshal.PtrToStructure(copyData.lpData,
                                    typeof(AppBarMsgDataV3));

                                if (Marshal.SizeOf(typeof(AppBarDataV2)) != amd.abd.cbSize)
                                {
                                    ShellLogger.Debug("TrayService: Size incorrect for APPBARMSGDATAV3");
                                    break;
                                }

                                IntPtr abmResult = AppBarMessageAction(amd);

                                if (abmResult != IntPtr.Zero)
                                    return abmResult;

                                ShellLogger.Debug($"TrayService: Forwarding AppBar message {(ABMsg)amd.dwMessage} from PID {amd.dwSourceProcessId}");
                            }
                            else
                                ShellLogger.Debug("TrayService: AppBar message received, but with unknown size");
                            break;
                        case 1:
                            ShellTrayData trayData =
                                (ShellTrayData)Marshal.PtrToStructure(copyData.lpData,
                                    typeof(ShellTrayData));
                            if (trayDelegate != null)
                            {
                                if (trayDelegate(trayData.dwMessage, new SafeNotifyIconData(trayData.nid)))
                                    return (IntPtr)1;

                                ShellLogger.Debug("TrayService: Ignored notify icon message");
                            }
                            else
                                ShellLogger.Info("TrayService: TrayDelegate is null");
                            break;
                        case 3:
                            WinNotifyIconIdentifier iconData =
                                (WinNotifyIconIdentifier)Marshal.PtrToStructure(copyData.lpData,
                                    typeof(WinNotifyIconIdentifier));

                            if (iconDataDelegate != null)
                                return iconDataDelegate(iconData.dwMessage, iconData.hWnd, iconData.uID,
                                    iconData.guidItem);

                            ShellLogger.Info("TrayService: IconDataDelegate is null");
                            break;
                    }

                    break;
                case WM.WINDOWPOSCHANGED:
                    WindowPos wndPos = WindowPos.FromMessage(lParam);

                    if ((wndPos.flags & SWP.SWP_SHOWWINDOW) != 0)
                    {
                        SetWindowLong(HwndTray, GWL.GWL_STYLE,
                            GetWindowLong(HwndTray, GWL.GWL_STYLE) &
                            ~(int)WindowStyles.WS_VISIBLE);

                        ShellLogger.Debug($"TrayService: {WindowHelper.TrayWndClass} became visible; hiding");
                    }
                    break;
            }

            if (msg == (int)WM.COPYDATA ||
                msg == (int)WM.ACTIVATEAPP ||
                msg == (int)WM.COMMAND ||
                msg >= (int)WM.USER)
            {
                return ForwardMsg(hWnd, msg, wParam, lParam);
            }
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    #region Event handling
    private IntPtr AppBarMessageAction(AppBarMsgDataV3 amd)
    {
        // only handle ABM_GETTASKBARPOS, send other AppBar messages to default handler
        switch ((ABMsg)amd.dwMessage)
        {
            case ABMsg.ABM_GETTASKBARPOS:
                IntPtr hShared = SHLockShared((IntPtr)amd.hSharedMemory, (uint)amd.dwSourceProcessId);
                AppBarDataV2 abd = (AppBarDataV2)Marshal.PtrToStructure(hShared, typeof(AppBarDataV2));
                FillTrayHostSizeData(ref abd);
                Marshal.StructureToPtr(abd, hShared, false);
                SHUnlockShared(hShared);
                ShellLogger.Debug("TrayService: Responded to ABM_GETTASKBARPOS");
                return (IntPtr)1;
        }
        return IntPtr.Zero;
    }

    private void FillTrayHostSizeData(ref AppBarDataV2 abd)
    {
        if (trayHostSizeDelegate != null)
        {
            TrayHostSizeData msd = trayHostSizeDelegate();
            abd.rc = msd.rc;
            abd.uEdge = (uint)msd.edge;
        }
        else
            ShellLogger.Info("TrayService: TrayHostSizeDelegate is null");
    }

    private IntPtr ForwardMsg(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        if (HwndFwd == IntPtr.Zero || !IsWindow(HwndFwd))
            HwndFwd = WindowHelper.FindWindowsTray(HwndTray);

        if (HwndFwd != IntPtr.Zero)
            return SendMessage(HwndFwd, msg, wParam, lParam);

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }
    #endregion

    #region Window helpers
    private ushort RegisterWndClass(string name)
    {
        WndClass newClass = new()
        {
            lpszClassName = name,
            hInstance = hInstance,
            style = 0x8,
            lpfnWndProc = wndProcDelegate,
        };

        return RegisterClass(ref newClass);
    }

    private void RegisterTrayWnd()
    {
        ushort trayClassReg = RegisterWndClass(WindowHelper.TrayWndClass);
        if (trayClassReg == 0)
        {
            ShellLogger.Info($"TrayService: Error registering {WindowHelper.TrayWndClass} class ({Marshal.GetLastWin32Error()})");
        }

        HwndTray = CreateWindowEx(
            ExtendedWindowStyles.WS_EX_TOPMOST |
            ExtendedWindowStyles.WS_EX_TOOLWINDOW, trayClassReg, "",
            WindowStyles.WS_POPUP | WindowStyles.WS_CLIPCHILDREN |
            WindowStyles.WS_CLIPSIBLINGS, 0, 0, GetSystemMetrics(0),
            (int)(23 * DpiHelper.DpiScale), IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);

        if (HwndTray == IntPtr.Zero)
            ShellLogger.Info($"TrayService: Error creating {WindowHelper.TrayWndClass} window ({Marshal.GetLastWin32Error()})");
        else
            ShellLogger.Debug($"TrayService: Created {WindowHelper.TrayWndClass}");
    }

    private void RegisterNotifyWnd()
    {
        ushort trayNotifyClassReg = RegisterWndClass(NotifyWndClass);
        if (trayNotifyClassReg == 0)
            ShellLogger.Info($"TrayService: Error registering {NotifyWndClass} class ({Marshal.GetLastWin32Error()})");

        HwndNotify = CreateWindowEx(0, trayNotifyClassReg, null,
            WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN |
            WindowStyles.WS_CLIPSIBLINGS, 0, 0, GetSystemMetrics(0),
            (int)(23 * DpiHelper.DpiScale), HwndTray, IntPtr.Zero, hInstance, IntPtr.Zero);

        if (HwndNotify == IntPtr.Zero)
            ShellLogger.Info($"TrayService: Error creating {NotifyWndClass} window ({Marshal.GetLastWin32Error()})");
        else
            ShellLogger.Debug($"TrayService: Created {NotifyWndClass}");
    }

    private void SetupTrayMonitor()
    {
        trayMonitor.Interval = new TimeSpan(0, 0, 0, 0, 100);
        trayMonitor.Tick += TrayMonitor_Tick;
    }

    private void TrayMonitor_Tick(object sender, EventArgs e)
    {
        if (HwndTray == IntPtr.Zero || Disable)
            return;

        IntPtr taskbarHwnd = FindWindow(WindowHelper.TrayWndClass, "");

        if (taskbarHwnd == HwndTray)
            return;

        ShellLogger.Debug("TrayService: Raising Shell_TrayWnd");
        MakeTrayTopmost();
    }

    private void SetWindowsTrayBottommost()
    {
        IntPtr taskbarHwnd = WindowHelper.FindWindowsTray(HwndTray);

        if (taskbarHwnd != IntPtr.Zero)
        {
            try
            {
                SetWindowPos(taskbarHwnd, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0,
                    SWP.SWP_NOMOVE | SWP.SWP_NOSIZE |
                    SWP.SWP_NOACTIVATE);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }

    private void MakeTrayTopmost()
    {
        if (HwndTray != IntPtr.Zero)
        {
            try
            {
                SetWindowPos(HwndTray, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 0, 0, 0,
                    SWP.SWP_NOMOVE | SWP.SWP_NOACTIVATE |
                    SWP.SWP_NOSIZE);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }
    #endregion
}
