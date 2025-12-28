using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Common.SupportingClasses;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTasks;

public class TasksService(IconSize iconSize) : DependencyObject, IDisposable
{
    public static readonly IconSize DEFAULT_ICON_SIZE = IconSize.Small;

    private NativeWindowEx _HookWin;
    private readonly object _windowsLock = new();
    internal bool IsInitialized;
    public IconSize TaskIconSize { get; set; } = iconSize;

    private static int WM_SHELLHOOKMESSAGE = -1;
    private static int WM_TASKBARCREATEDMESSAGE = -1;
    private static int TASKBARBUTTONCREATEDMESSAGE = -1;
    private static IntPtr uncloakEventHook = IntPtr.Zero;
#pragma warning disable IDE0079
#pragma warning disable S1450 // Private fields only used as local variables in methods should become local variables
    private WinEventProc uncloakEventProc;
#pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables
#pragma warning restore IDE0079

    internal ITaskCategoryProvider TaskCategoryProvider;
    private TaskCategoryChangeDelegate CategoryChangeDelegate;

    public TasksService() : this(DEFAULT_ICON_SIZE)
    {
    }

    private static void RegisterWindowsMessages()
    {
        WM_SHELLHOOKMESSAGE = RegisterWindowMessage("SHELLHOOK");
        WM_TASKBARCREATEDMESSAGE = RegisterWindowMessage("TaskbarCreated");
        TASKBARBUTTONCREATEDMESSAGE = RegisterWindowMessage("TaskbarButtonCreated");
    }

    public event EventHandler<WindowEventArgs> WindowActivated;
    public event EventHandler<WindowEventArgs> WindowUncloaked;
    public event EventHandler<WindowEventArgs> DesktopActivated;
    public event EventHandler<FullScreenEventArgs> FullScreenChanged;
    public event EventHandler<WindowEventArgs> MonitorChanged;
    public event EventHandler<WindowEventArgs> TaskbarListChanged;
    public event EventHandler<WindowEventArgs> WindowDestroy;
    public event EventHandler<WindowEventArgs> WindowCreate;

    internal void Initialize(bool initialWindows)
    {
        if (IsInitialized)
            return;

        try
        {
            ShellLogger.Debug("TasksService: Starting");

            // create window to receive task events
            _HookWin = new NativeWindowEx();
            _HookWin.CreateHandle(new CreateParams());

            // prevent other shells from working properly
            SetTaskmanWindow(_HookWin.Handle);

            // register to receive task events
            RegisterShellHookWindow(_HookWin.Handle);
            RegisterWindowsMessages();
            _HookWin.MessageReceived += ShellWinProc;

            if (EnvironmentHelper.IsWindows8OrBetter)
            {
                // set event hook for uncloak events
                uncloakEventProc = UncloakEventCallback;

                if (uncloakEventHook == IntPtr.Zero)
                    SetUncloakEventHook(uncloakEventProc);
            }

            // set window for ITaskbarList
            SetTaskbarListHwnd(_HookWin.Handle);

            // adjust minimize animation
            SetMinimizedMetrics();

            // enumerate windows already opened and set active window
            if (initialWindows)
                GetInitialWindows();

            IsInitialized = true;
        }
        catch (Exception ex)
        {
            ShellLogger.Info("TasksService: Unable to start: " + ex.Message);
        }
    }

    private static void SetUncloakEventHook(WinEventProc uncloakEventProc)
    {
        uncloakEventHook = SetWinEventHook(
            EVENT_OBJECT_UNCLOAKED,
            EVENT_OBJECT_UNCLOAKED,
            IntPtr.Zero,
            uncloakEventProc,
            0,
            0,
            WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
    }

    internal void SetTaskCategoryProvider(ITaskCategoryProvider provider)
    {
        TaskCategoryProvider = provider;

        CategoryChangeDelegate ??= CategoriesChanged;

        TaskCategoryProvider.SetCategoryChangeDelegate(CategoryChangeDelegate);
    }

    public void GetInitialWindows()
    {
        EnumWindows((hwnd, lParam) =>
        {
            ApplicationWindow win = new(this, hwnd);

            if (win.CanAddToTaskbar && win.ShowInTaskbar && !Windows.Contains(win))
            {
                Windows.Add(win);
                if (EnvironmentHelper.IsAppRunningAsShell)
                    SendTaskbarButtonCreatedMessage(win.ListWindows[0].Handle);
            }

            return true;
        }, 0);

        IntPtr hWndForeground = GetForegroundWindow();
        ApplicationWindow win = Windows.FirstOrDefault(wnd => wnd.ListWindows?.Count > 0 && wnd.ListWindows[0].Handle == hWndForeground);
        if (win != null && win.ShowInTaskbar)
        {
            win.State = ApplicationWindow.WindowState.Active;
            win.SetShowInTaskbar();
        }
    }

    #region IDisposable

    private bool _isDisposed;
    public bool IsDisposed
    {
        get { return _isDisposed; }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                if (IsInitialized)
                {
                    ShellLogger.Debug("TasksService: Deregistering hooks");
                    DeregisterShellHookWindow(_HookWin.Handle);
                    if (uncloakEventHook != IntPtr.Zero)
                        UnhookWinEvent(uncloakEventHook);
                    _HookWin.DestroyHandle();
                    SetTaskbarListHwnd(IntPtr.Zero);
                    IsInitialized = false;
                    Windows.Clear();
                }

                TaskCategoryProvider?.Dispose();
            }
            _isDisposed = true;
        }
    }

    #endregion

    private void CategoriesChanged()
    {
        foreach (ApplicationWindow window in Windows)
            if (window.ShowInTaskbar)
                window.Category = TaskCategoryProvider?.GetCategory(window);
    }

    private void SetMinimizedMetrics()
    {
        MinimizedMetrics mm = new()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(MinimizedMetrics)),
        };

        IntPtr mmPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MinimizedMetrics)));

        try
        {
            Marshal.StructureToPtr(mm, mmPtr, true);
            SystemParametersInfo(ESystemParametersInfo.GETMINIMIZEDMETRICS, mm.cbSize, mmPtr, SystemParametersInfoUpdateMethods.None);
            mm.iWidth = 140;
            mm.iArrange |= MinimizedMetricsArrangements.Hide;
            Marshal.StructureToPtr(mm, mmPtr, true);
            SystemParametersInfo(ESystemParametersInfo.SETMINIMIZEDMETRICS, mm.cbSize, mmPtr, SystemParametersInfoUpdateMethods.None);
        }
        finally
        {
            Marshal.DestroyStructure(mmPtr, typeof(MinimizedMetrics));
            Marshal.FreeHGlobal(mmPtr);
        }
    }

    public void CloseWindow(ApplicationWindow window)
    {
        if (window.DoClose() != IntPtr.Zero)
        {
            bool raiseEvent = window.ShowInTaskbar;
            ShellLogger.Debug($"TasksService: Removing window {window.Title} from collection due to no response");
            Windows.Remove(window);
            if (raiseEvent)
                WindowDestroy?.Invoke(this, new WindowEventArgs() { Window = window });
            window.Dispose();
        }
    }

    public static void SendTaskbarButtonCreatedMessage(IntPtr hWnd)
    {
        // Server Core doesn't support ITaskbarList, so sending this message on that OS could cause some assuming apps to crash
        if (!EnvironmentHelper.IsServerCore)
            SendNotifyMessage(hWnd, (uint)TASKBARBUTTONCREATEDMESSAGE, UIntPtr.Zero, IntPtr.Zero);
    }

    public ApplicationWindow AddWindow(IntPtr hWnd, ApplicationWindow.WindowState initialState = ApplicationWindow.WindowState.Inactive, bool sanityCheck = false)
    {
        ApplicationWindow win;
        win = new(this, hWnd);

        // set window state if a non-default value is provided
        if (initialState != ApplicationWindow.WindowState.Inactive)
            win.State = initialState;

        // add window unless we need to validate it is eligible to show in taskbar
        if ((!sanityCheck || win.CanAddToTaskbar) && win.ClassName != "Windows.UI.Core.CoreWindow")
            Windows.Add(win);

        // Only send TaskbarButtonCreated if we are shell, and if OS is not Server Core
        // This is because if Explorer is running, it will send the message, so we don't need to
        if (EnvironmentHelper.IsAppRunningAsShell)
            SendTaskbarButtonCreatedMessage(win.ListWindows[0].Handle);

        if (win.ShowInTaskbar)
            WindowCreate?.Invoke(this, new WindowEventArgs() { Handle = hWnd, Window = win });

        return win;
    }

    internal void RemoveWindow(ApplicationWindow window, IntPtr hwnd)
    {
        Windows.Remove(window);
        if (window.ShowInTaskbar)
            WindowDestroy?.Invoke(this, new WindowEventArgs() { Handle = hwnd, Window = window });
    }

    public void RemoveWindow(IntPtr hWnd)
    {
        ApplicationWindow win = Windows.FirstOrDefault(wnd => wnd.ListWindows.Any(w => w.Handle == hWnd));
        if (win != null)
        {
            bool disposeWindow = (win.ListWindows.Count == 1);
            if (win.ListWindows.Count == 2)
                win.State = ApplicationWindow.WindowState.Active;
            if (disposeWindow && !win.IsPinnedApp)
            {
                Windows.Remove(win);
                win.Dispose();
            }
            else
            {
                ApplicationWindowsProperty appWinProp = win.ListWindows.SingleOrDefault(w => w.Handle == hWnd);
                if (appWinProp != null)
                    win.ListWindows.Remove(appWinProp);
                if (win.IsPinnedApp && !GroupApplicationsWindows)
                {
                    ApplicationWindow nextPinned = Windows.FirstOrDefault(aw => aw != win && aw.WinFileName == win.WinFileName && aw.Arguments == win.Arguments && !aw.IsPinnedApp);
                    if (nextPinned != null)
                    {
                        nextPinned.IsPinnedApp = true;
                        nextPinned.PinnedShortcut = win.PinnedShortcut;
                        nextPinned.Position = win.Position;
                        Windows.Remove(win);
                        win.Dispose();
                        return;
                    }
                }
                win.SetTitle();
                win.OnPropertyChanged(nameof(ApplicationWindow.Launched));
                win.OnPropertyChanged(nameof(ApplicationWindow.MultipleInstanceLaunched));
            }
            WindowDestroy?.Invoke(this, new WindowEventArgs() { Handle = hWnd, Window = win });
        }
    }

    private void RedrawWindow(ApplicationWindow win)
    {
        win.UpdateProperties();

        foreach (ApplicationWindow wind in Windows)
            if (wind.WinFileName == win.WinFileName && wind.ListWindows.Count > 0 && win.ListWindows.Count > 0 && wind.ListWindows[0] != win.ListWindows[0])
                wind.UpdateProperties();
    }

    private void ShellWinProc(Message msg)
    {
        if (msg.Msg == WM_SHELLHOOKMESSAGE)
        {
            try
            {
                lock (_windowsLock)
                {
                    string winFileName = ShellHelper.GetPathForHandle(msg.LParam);
                    ApplicationWindow win = null;
                    switch ((HShell)msg.WParam.ToInt32())
                    {
                        case HShell.WINDOWCREATED:
                            ShellLogger.Debug("TasksService: Created: " + msg.LParam);
                            if (GroupApplicationsWindows)
                                win = Windows.FirstOrDefault(wnd => wnd.ListWindows.Any(w => w.Handle == msg.LParam) || wnd.WinFileName == winFileName);
                            else
                                win = Windows.FirstOrDefault(wnd => wnd.ListWindows.Any(w => w.Handle == msg.LParam) || (wnd.IsPinnedApp && wnd.WinFileName == winFileName && wnd.ListWindows.Count == 0));
                            if (win == null)
                                AddWindow(msg.LParam);
                            else
                                win.UpdateProperties(msg.LParam);
                            break;
                        case HShell.WINDOWDESTROYED:
                            ShellLogger.Debug("TasksService: Destroyed: " + msg.LParam);
                            RemoveWindow(msg.LParam);
                            break;
                        case HShell.WINDOWREPLACING:
                            ShellLogger.Debug("TasksService: Replacing: " + msg.LParam);
                            win = Windows.FirstOrDefault(i => i.ListWindows.Any(w => w.Handle == msg.LParam));
                            if (win != null)
                            {
                                win.State = ApplicationWindow.WindowState.Inactive;
                                win.SetShowInTaskbar();
                            }
                            else
                                AddWindow(msg.LParam);
                            break;
                        case HShell.WINDOWREPLACED:
                            ShellLogger.Debug("TasksService: Replaced: " + msg.LParam);
                            // TODO: If a window gets replaced, we lose app-level state such as overlay icons.
                            RemoveWindow(msg.LParam);
                            break;
                        case HShell.WINDOWACTIVATED:
                        case HShell.RUDEAPPACTIVATED:
                            ShellLogger.Debug("TasksService: Activated: " + msg.LParam);

                            foreach (ApplicationWindow aWin in Windows.Where(w => w.State == ApplicationWindow.WindowState.Active))
                                aWin.State = ApplicationWindow.WindowState.Inactive;

                            if (msg.LParam != IntPtr.Zero)
                            {
                                win = Windows.FirstOrDefault(i => i.ListWindows.Any(w => w.Handle == msg.LParam));
                                if (win != null)
                                {
                                    win.State = ApplicationWindow.WindowState.Active;
                                    win.SetTitle(msg.LParam);
                                    win.SetShowInTaskbar();
                                }
                                else
                                    win = AddWindow(msg.LParam, ApplicationWindow.WindowState.Active);

                                if (win != null)
                                    foreach (ApplicationWindow wind in Windows)
                                        if (wind.WinFileName == win.WinFileName && wind.ListWindows.Count > 0 && win.ListWindows.Count > 0 && wind.ListWindows[0] != win.ListWindows[0])
                                            wind.SetShowInTaskbar();
                                WindowActivated?.Invoke(this, new WindowEventArgs() { Handle = msg.LParam, Window = win });
                            }
                            else
                                DesktopActivated?.Invoke(this, new WindowEventArgs() { Handle = IntPtr.Zero, Window = null });
                            break;
                        case HShell.FLASH:
                            ShellLogger.Debug("TasksService: Flashing window: " + msg.LParam);
                            win = Windows.FirstOrDefault(i => i.ListWindows.Any(w => w.Handle == msg.LParam));
                            if (win != null)
                            {
                                if (win.State != ApplicationWindow.WindowState.Active)
                                    win.State = ApplicationWindow.WindowState.Flashing;

                                RedrawWindow(win);
                            }
                            else
                                AddWindow(msg.LParam, ApplicationWindow.WindowState.Flashing, true);
                            break;
                        case HShell.ACTIVATESHELLWINDOW:
                            ShellLogger.Debug("TasksService: Activate shell window called.");
                            break;
                        case HShell.ENDTASK:
                            ShellLogger.Debug("TasksService: EndTask called: " + msg.LParam);
                            RemoveWindow(msg.LParam);
                            break;
                        case HShell.REDRAW:
                            ShellLogger.Debug("TasksService: Redraw called: " + msg.LParam);
                            win = Windows.FirstOrDefault(wnd => wnd.ListWindows.Any(w => w.Handle == msg.LParam) || wnd.WinFileName == winFileName);
                            if (win != null)
                            {
                                if (win.State == ApplicationWindow.WindowState.Flashing)
                                    win.State = ApplicationWindow.WindowState.Inactive;
                                RedrawWindow(win);
                            }
                            else
                                AddWindow(msg.LParam, ApplicationWindow.WindowState.Inactive, true);
                            break;
                        case HShell.MONITORCHANGED:
                            if (Windows.Any(i => i.ListWindows.Any(w => w.Handle == msg.LParam)))
                            {
                                win = Windows.First(wnd => wnd.ListWindows.Any(w => w.Handle == msg.LParam));
                                WindowEventArgs winArgs = new()
                                {
                                    Window = win,
                                    Handle = msg.LParam,
                                };
                                MonitorChanged?.Invoke(this, winArgs);
                            }
                            break;
                        case HShell.FULLSCREENENTER:
                            FullScreenEventArgs args = new()
                            {
                                Handle = msg.LParam,
                                IsEntering = true,
                            };
                            FullScreenChanged?.Invoke(this, args);
                            ShellLogger.Debug($"TasksService: Full screen entered by window {msg.LParam}");
                            break;
                        case HShell.FULLSCREENEXIT:
                            FullScreenEventArgs fsArgs = new()
                            {
                                Handle = msg.LParam,
                                IsEntering = false,
                            };
                            FullScreenChanged?.Invoke(this, fsArgs);
                            ShellLogger.Debug($"TasksService: Full screen exited by window {msg.LParam}");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShellLogger.Error("TasksService: Error in ShellWinProc. ", ex);
                Debugger.Break();
            }
        }
        else if (msg.Msg == WM_TASKBARCREATEDMESSAGE)
        {
            ShellLogger.Debug("TasksService: TaskbarCreated received, setting ITaskbarList window");
            SetTaskbarListHwnd(_HookWin.Handle);
        }
        else
        {
            // Handle ITaskbarList functions, most not implemented yet

            ApplicationWindow win = Windows.FirstOrDefault(wnd => wnd.ListWindows.Any(w => w.Handle == msg.WParam));

            switch (msg.Msg)
            {
                case (int)WM.TTM_ADDTOOLW:
                    // ActivateTab
                    // Also sends WM_SHELLHOOK message
                    ShellLogger.Debug("TasksService: ITaskbarList: ActivateTab HWND:" + msg.LParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_SETMAXTEXTROWS:
                    // MarkFullscreenWindow
                    ShellLogger.Debug("TasksService: ITaskbarList: MarkFullscreenWindow HWND:" + msg.LParam + " Entering? " + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_SETBUTTONINFOW:
                    // SetProgressValue
                    ShellLogger.Debug("TasksService: ITaskbarList: SetProgressValue HWND:" + msg.WParam + " Progress: " + msg.LParam);
#pragma warning disable IDE0079
#pragma warning disable S1121
                    win?.ProgressValue = (int)msg.LParam;
#pragma warning restore S1121
#pragma warning restore IDE0079
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_GETBUTTONINFOA:
                    // SetProgressState
                    ShellLogger.Debug("TasksService: ITaskbarList: SetProgressState HWND:" + msg.WParam + " Flags: " + msg.LParam);
#pragma warning disable IDE0079
#pragma warning disable S1121
                    win?.ProgressState = (ToolbarProgressBarState)msg.LParam;
#pragma warning restore S1121
#pragma warning restore IDE0079
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_INSERTBUTTONW:
                    // RegisterTab
                    ShellLogger.Debug("TasksService: ITaskbarList: RegisterTab MDI HWND:" + msg.LParam + " Tab HWND: " + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_ADDBUTTONSW:
                    // UnregisterTab
                    ShellLogger.Debug("TasksService: ITaskbarList: UnregisterTab Tab HWND: " + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_GETHOTITEM:
                    // SetTabOrder
                    ShellLogger.Debug("TasksService: ITaskbarList: SetTabOrder HWND:" + msg.WParam + " Before HWND: " + msg.LParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_SETHOTITEM:
                    // SetTabActive
                    ShellLogger.Debug("TasksService: ITaskbarList: SetTabActive HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_GETBUTTONTEXTW:
                    // Unknown
                    ShellLogger.Debug("TasksService: ITaskbarList: Unknown HWND:" + msg.WParam + " LParam: " + msg.LParam);
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_SAVERESTOREW:
                    // ThumbBarAddButtons
                    ShellLogger.Debug("TasksService: ITaskbarList: ThumbBarAddButtons HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_ADDSTRINGW:
                    // ThumbBarUpdateButtons
                    ShellLogger.Debug("TasksService: ITaskbarList: ThumbBarUpdateButtons HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_MAPACCELERATORA:
                    // ThumbBarSetImageList
                    ShellLogger.Debug("TasksService: ITaskbarList: ThumbBarSetImageList HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_GETINSERTMARK:
                    // SetOverlayIcon - Icon
                    ShellLogger.Debug("TasksService: ITaskbarList: SetOverlayIcon - Icon HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    win?.SetOverlayIcon(msg.LParam);
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_SETINSERTMARK:
                    // SetThumbnailTooltip
                    ShellLogger.Debug("TasksService: ITaskbarList: SetThumbnailTooltip HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_INSERTMARKHITTEST:
                    // SetThumbnailClip
                    ShellLogger.Debug("TasksService: ITaskbarList: SetThumbnailClip HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_GETEXTENDEDSTYLE:
                    // SetOverlayIcon - Description
                    ShellLogger.Debug("TasksService: ITaskbarList: SetOverlayIcon - Description HWND:" + msg.WParam);
                    win?.SetOverlayIconDescription(msg.LParam);
                    msg.Result = IntPtr.Zero;
                    return;
                case (int)WM.TB_SETPADDING:
                    // SetTabProperties
                    ShellLogger.Debug("TasksService: ITaskbarList: SetTabProperties HWND:" + msg.WParam);
                    TaskbarListChanged?.Invoke(this, new WindowEventArgs() { Handle = msg.WParam, Window = win });
                    msg.Result = IntPtr.Zero;
                    return;
            }
        }

        msg.Result = DefWindowProc(msg.HWnd, msg.Msg, msg.WParam, msg.LParam);
    }

    private void UncloakEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        ApplicationWindow win = Windows.FirstOrDefault(wnd => wnd.ListWindows.Any(w => w.Handle == hWnd));
        if (win?.IsUWP == true)
        {
            string AppModelId = ShellHelper.GetAppUserModelIdForHandle(hWnd);
            ApplicationWindow winAlreadyExist = Windows.FirstOrDefault(w => w.AppUserModelID == win.AppUserModelID && w != win);
            if (winAlreadyExist != null && winAlreadyExist.ListWindows?.Count == 0)
            {
                win.IsPinnedApp = winAlreadyExist.IsPinnedApp;
                win.Position = winAlreadyExist.Position;
                winAlreadyExist.Dispose();
            }
            if (win.AppUserModelID.IndexOf(AppModelId, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                RemoveWindow(hWnd);
                if (GroupApplicationsWindows)
                    win = Windows.FirstOrDefault(wnd => wnd.AppUserModelID?.IndexOf(AppModelId, StringComparison.InvariantCultureIgnoreCase) >= 0);
                if (win == null)
                    AddWindow(hWnd);
                else
                {
                    ApplicationWindowsProperty appWinProp = win.ListWindows.SingleOrDefault(w => w.Handle == hWnd);
                    if (appWinProp != null)
                        win.ListWindows.Add(appWinProp);
                    win.SetTitle();
                }
            }
        }
        if (hWnd != IntPtr.Zero && idObject == 0 && idChild == 0 && win != null)
        {
            win.Uncloak();
            WindowUncloaked?.Invoke(this, new WindowEventArgs() { Handle = hWnd, Window = win });
        }
    }

    private static void SetTaskbarListHwnd(IntPtr hwndHook)
    {
        // set property on hook window that should receive ITaskbarList messages

        IntPtr taskbarHwnd = FindWindow(WindowHelper.TrayWndClass, "");
        if (taskbarHwnd != IntPtr.Zero)
        {
            if (hwndHook == IntPtr.Zero)
            {
                // Try to find and use the handle of the Explorer hook window
                EnumChildWindows(taskbarHwnd, (hwnd, lParam) =>
                {
                    StringBuilder cName = new(256);
                    GetClassName(hwnd, cName, cName.Capacity);
                    if (cName.ToString() == "MSTaskSwWClass")
                    {
                        hwndHook = hwnd;
                        return false;
                    }

                    return true;
                }, 0);
            }

            if (hwndHook != IntPtr.Zero)
                SetProp(taskbarHwnd, "TaskbandHWND", hwndHook);
        }
    }

    public ObservableCollection<ApplicationWindow> Windows
    {
        get
        {
            return base.GetValue(windowsProperty) as ObservableCollection<ApplicationWindow>;
        }
        set
        {
            SetValue(windowsProperty, value);
        }
    }

    private readonly DependencyProperty windowsProperty = DependencyProperty.Register(nameof(Windows),
        typeof(ObservableCollection<ApplicationWindow>), typeof(TasksService),
        new PropertyMetadata(new ObservableCollection<ApplicationWindow>()));

    public bool GroupApplicationsWindows { get; set; } = true;
}
