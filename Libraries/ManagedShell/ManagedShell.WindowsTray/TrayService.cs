using System;
using System.Runtime.InteropServices;
using System.Windows.Media;
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

    private IntPtr _hWndTray;
    private IntPtr _hWdNotify;
    private IntPtr _hWndFwd;
    private readonly IntPtr _hInstance = Marshal.GetHINSTANCE(typeof(TrayService).Module);

    private readonly DispatcherTimer _trayMonitor = new(DispatcherPriority.Background);

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
        if (_hWndTray != IntPtr.Zero)
            return _hWndTray;

        DestroyWindows();

        wndProcDelegate = WndProc;

        RegisterTrayWnd();
        RegisterNotifyWnd();

        return _hWndTray;
    }

    /// <summary>
    /// Starts the system tray listener (send the TaskbarCreated message).
    /// </summary>
    internal void Run()
    {
        if (_hWndTray != IntPtr.Zero)
        {
            Resume();
            SendTaskbarCreated();
        }
    }

    internal void Suspend()
    {
        // if we go beneath another tray, it will receive messages
        if (_hWndTray != IntPtr.Zero)
        {
            _trayMonitor.Stop();
            SetWindowPos(_hWndTray, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0,
                EShowWindowPos.SWP_NOMOVE | EShowWindowPos.SWP_NOACTIVATE |
                EShowWindowPos.SWP_NOSIZE);
        }
    }

    internal void Resume()
    {
        // if we are above another tray, we will receive messages
        if (_hWndTray != IntPtr.Zero)
        {
            SetWindowsTrayBottomMost();
            MakeTrayTopmost();
            _trayMonitor.Start();
        }
    }

    internal void SetTrayHostSizeData(TrayHostSizeData data)
    {
        ShellLogger.Debug("SetTrayHostSizeData");

        if (_hWndTray != IntPtr.Zero)
            SetWindowPos(_hWndTray, IntPtr.Zero, data.rc.Left, data.rc.Top, data.rc.Width, data.rc.Height, EShowWindowPos.SWP_NOACTIVATE | EShowWindowPos.SWP_NOZORDER);

        if (_hWdNotify != IntPtr.Zero)
            SetWindowPos(_hWdNotify, IntPtr.Zero, data.rc.Left, data.rc.Top, data.rc.Width, data.rc.Height, EShowWindowPos.SWP_NOACTIVATE | EShowWindowPos.SWP_NOZORDER);
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
        if (_hWdNotify != IntPtr.Zero)
        {
            DestroyWindow(_hWdNotify);
            UnregisterClass(NotifyWndClass, _hInstance);
            ShellLogger.Debug($"TrayService: Unregistered {NotifyWndClass}");
        }

        if (_hWndTray != IntPtr.Zero)
        {
            DestroyWindow(_hWndTray);
            UnregisterClass(WindowHelper.TrayWndClass, _hInstance);
            ShellLogger.Debug($"TrayService: Unregistered {WindowHelper.TrayWndClass}");
        }
    }

    public bool Disable { get; set; }

    public Color DefaultThemeColor { get; set; }

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
                _trayMonitor.Stop();
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

                    if ((wndPos.flags & EShowWindowPos.SWP_SHOWWINDOW) != 0)
                    {
                        SetWindowLong(_hWndTray, EGetWindowLong.GWL_STYLE,
                            GetWindowLong(_hWndTray, EGetWindowLong.GWL_STYLE) &
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
        if (_hWndFwd == IntPtr.Zero || !IsWindow(_hWndFwd))
            _hWndFwd = WindowHelper.FindWindowsTray(_hWndTray);

        if (_hWndFwd != IntPtr.Zero)
            return SendMessage(_hWndFwd, msg, wParam, lParam);

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }
    #endregion

    #region Window helpers
    private ushort RegisterWndClass(string name)
    {
        WndClass newClass = new()
        {
            lpszClassName = name,
            hInstance = _hInstance,
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

        _hWndTray = CreateWindowEx(
            ExtendedWindowStyles.WS_EX_TOPMOST |
            ExtendedWindowStyles.WS_EX_TOOLWINDOW, trayClassReg, "",
            WindowStyles.WS_POPUP | WindowStyles.WS_CLIPCHILDREN |
            WindowStyles.WS_CLIPSIBLINGS, 0, 0, GetSystemMetrics(0),
            (int)(23 * DpiHelper.DpiScale), IntPtr.Zero, IntPtr.Zero, _hInstance, IntPtr.Zero);

        if (_hWndTray == IntPtr.Zero)
            ShellLogger.Info($"TrayService: Error creating {WindowHelper.TrayWndClass} window ({Marshal.GetLastWin32Error()})");
        else
            ShellLogger.Debug($"TrayService: Created {WindowHelper.TrayWndClass}");
    }

    private void RegisterNotifyWnd()
    {
        ushort trayNotifyClassReg = RegisterWndClass(NotifyWndClass);
        if (trayNotifyClassReg == 0)
            ShellLogger.Info($"TrayService: Error registering {NotifyWndClass} class ({Marshal.GetLastWin32Error()})");

        _hWdNotify = CreateWindowEx(0, trayNotifyClassReg, null,
            WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN |
            WindowStyles.WS_CLIPSIBLINGS, 0, 0, GetSystemMetrics(0),
            (int)(23 * DpiHelper.DpiScale), _hWndTray, IntPtr.Zero, _hInstance, IntPtr.Zero);

        if (_hWdNotify == IntPtr.Zero)
            ShellLogger.Info($"TrayService: Error creating {NotifyWndClass} window ({Marshal.GetLastWin32Error()})");
        else
            ShellLogger.Debug($"TrayService: Created {NotifyWndClass}");
    }

    private void SetupTrayMonitor()
    {
        _trayMonitor.Interval = new TimeSpan(0, 0, 0, 0, 100);
        _trayMonitor.Tick += TrayMonitor_Tick;
    }

    private void TrayMonitor_Tick(object sender, EventArgs e)
    {
        if (_hWndTray == IntPtr.Zero || Disable)
            return;

        IntPtr taskbarHwnd = FindWindow(WindowHelper.TrayWndClass, "");

        if (taskbarHwnd == _hWndTray)
            return;

        ShellLogger.Debug("TrayService: Raising Shell_TrayWnd");
        MakeTrayTopmost();
    }

    private void SetWindowsTrayBottomMost()
    {
        IntPtr taskbarHwnd = WindowHelper.FindWindowsTray(_hWndTray);

        if (taskbarHwnd != IntPtr.Zero)
        {
            try
            {
                SetWindowPos(taskbarHwnd, (IntPtr)WindowZOrder.HWND_BOTTOM, 0, 0, 0, 0,
                    EShowWindowPos.SWP_NOMOVE | EShowWindowPos.SWP_NOSIZE |
                    EShowWindowPos.SWP_NOACTIVATE);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }

    private void MakeTrayTopmost()
    {
        if (_hWndTray != IntPtr.Zero)
        {
            try
            {
                SetWindowPos(_hWndTray, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 0, 0, 0,
                    EShowWindowPos.SWP_NOMOVE | EShowWindowPos.SWP_NOACTIVATE |
                    EShowWindowPos.SWP_NOSIZE);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }
    #endregion
}
