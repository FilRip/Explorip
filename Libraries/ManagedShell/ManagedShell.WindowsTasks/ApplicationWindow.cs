using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using HookTaskbarList.TaskbarList.Interfaces;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

namespace ManagedShell.WindowsTasks;

[DebuggerDisplay("Title: {Title}, Handle: {_windows[0]}")]
public sealed class ApplicationWindow : IEquatable<ApplicationWindow>, INotifyPropertyChanged, IDisposable
{
    public const int MAX_STRING_SIZE = 255;
    private readonly TasksService _tasksService;

    private bool _iconLoading;
    private ImageSource _icon;
    private IntPtr _hIcon;
    private string _appUserModelId;
    private bool? _isUWP;
    private string _winFileName;
    private uint? _procId;
    private string _category;
    private string _title;
    private string _className;
    private ImageSource _overlayIcon;
    private string _overlayIconDescription;
    private NativeMethods.TBPFLAG _progressState;
    private int _progressValue;
    private WindowState _state;
    private bool? _showInTaskbar;
    private DateTime? _dateStart;
    private readonly List<IntPtr> _windows;
    private readonly object _lockUpdate;
    private int _position;
    private readonly Guid _id;
    private ThumbButton[] _thumbButtons;

    public delegate void GetButtonRectEventHandler(ref NativeMethods.ShortRect rect);
    public event GetButtonRectEventHandler GetButtonRect;

    public ApplicationWindow(TasksService tasksService, IntPtr handle)
    {
        _hIcon = IntPtr.Zero;
        _appUserModelId = null;
        _winFileName = "";
        _isUWP = null;
        _lockUpdate = new object();
        _windows = [];
        if (handle != IntPtr.Zero)
            _windows.Add(handle);
        _tasksService = tasksService;
        State = WindowState.Inactive;
        if (tasksService.Windows.Count > 0)
            _position = tasksService.Windows.Max(aw => aw.Position) + 1;
        _id = Guid.NewGuid();
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
    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _tasksService.RemoveWindow(this, IntPtr.Zero);
                _windows.Clear();
                _icon = null;
                if (_hIcon != IntPtr.Zero)
                    NativeMethods.DestroyIcon(_hIcon);
            }
            _isDisposed = true;
        }
    }

    #endregion

    public string AppUserModelID
    {
        get
        {
            if (string.IsNullOrEmpty(_appUserModelId) && _windows.Count > 0)
                _appUserModelId = ShellHelper.GetAppUserModelIdPropertyForHandle(_windows[0]);

            return _appUserModelId;
        }
    }

    public bool IsUWP
    {
        get
        {
            _isUWP ??= string.Compare(System.IO.Path.GetFileName(WinFileName), "applicationframehost.exe", StringComparison.OrdinalIgnoreCase) == 0;

            return (bool)_isUWP;
        }
    }

    public void SetIsUWP()
    {
        _title = System.IO.Path.GetFileName(WinFileName);
        _winFileName = "applicationframehost.exe";
        _appUserModelId = Arguments.Substring(Arguments.IndexOf("\\") + 1);
        Task.Factory.StartNew(LoadUWPIcon);
    }

    public string WinFileName
    {
        get
        {
            if (string.IsNullOrEmpty(_winFileName) && _windows.Count > 0)
                _winFileName = ShellHelper.GetPathForHandle(_windows[0]);

            return _winFileName;
        }
        set
        {
            if (_winFileName != value)
                _winFileName = value;
        }
    }

    public uint? ProcId
    {
        get
        {
            if (!_procId.HasValue && _windows[0] != IntPtr.Zero)
                _procId = ShellHelper.GetProcIdForHandle(_windows[0]);
            return _procId;
        }
    }

    public int Position
    {
        get { return _position; }
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged();
            }
        }
    }

    public bool MultipleInstanceLaunched
    {
        get { return ListWindows?.Count > 1; }
    }

    public string Category
    {
        get
        {
            return _category;
        }
        set
        {
            if (value != _category)
            {
                _category = value;
                OnPropertyChanged();
            }
        }
    }

    public string Title
    {
        get
        {
            if (_title == null)
                SetTitle();

            return _title;
        }
        private set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    public void SetTitle(IntPtr? ptrWindow = null)
    {
        string title = "";
        try
        {
            if (_windows?.Count > 0)
            {
                StringBuilder stringBuilder = new(MAX_STRING_SIZE);
                NativeMethods.GetWindowText((ptrWindow ?? _windows[0]), stringBuilder, MAX_STRING_SIZE);

                title = stringBuilder.ToString();
            }
        }
        catch { /* Ignore errors */ }

        if (IsPinnedApp && string.IsNullOrWhiteSpace(title))
            title = System.IO.Path.GetFileNameWithoutExtension(PinnedShortcut);

        if (_title != title)
            Title = title;
    }

    public string ClassName
    {
        get
        {
            if (_className == null)
                SetClassName();
            return _className;
        }
        private set
        {
            if (_className != value)
            {
                _className = value;
                OnPropertyChanged();
            }
        }
    }
    private void SetClassName()
    {
        string className = "";
        try
        {
            StringBuilder stringBuilder = new(MAX_STRING_SIZE);
            NativeMethods.GetClassName(_windows[0], stringBuilder, MAX_STRING_SIZE);

            className = stringBuilder.ToString();
        }
        catch { /* Ignore errors */ }

        ClassName = className;
    }

    public ImageSource Icon
    {
        get
        {
            if (_icon == null)
                SetIcon();

            return _icon;
        }
        set
        {
            if (_icon != value)
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
    }

    public ImageSource OverlayIcon
    {
        get
        {
            return _overlayIcon;
        }
        private set
        {
            if ( _overlayIcon != value)
            {
                _overlayIcon = value;
                OnPropertyChanged();
            }
        }
    }

    public string OverlayIconDescription
    {
        get
        {
            return _overlayIconDescription;
        }
        private set
        {
            if (_overlayIconDescription != value)
            {
                _overlayIconDescription = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsInProgress
    {
        get { return ProgressState != NativeMethods.TBPFLAG.TBPF_NOPROGRESS; }
    }

    public NativeMethods.TBPFLAG ProgressState
    {
        get
        {
            return _progressState;
        }
        set
        {
            if (_progressState != value)
            {
                _progressState = value;

                if (value == NativeMethods.TBPFLAG.TBPF_NOPROGRESS)
                    ProgressValue = 0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsInProgress));
                OnPropertyChanged(nameof(ProgressValue));
                OnPropertyChanged(nameof(PercentProgressValue));
            }
        }
    }

    public int ProgressValue
    {
        get
        {
            return _progressValue;
        }
        set
        {
            _progressValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PercentProgressValue));
            OnPropertyChanged(nameof(IsInProgress));
            OnPropertyChanged(nameof(ProgressState));
        }
    }

    public int PercentProgressValue
    {
        get { return (int)(_progressValue / 65535d * 100); }
    }

    public WindowState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value)
            {
                _state = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsMinimized(IntPtr handle = default)
    {
        if (handle == default)
            handle = _windows[0];
        if (handle == IntPtr.Zero)
            return true;
        return NativeMethods.IsIconic(handle);
    }

    public NativeMethods.WindowShowStyle ShowStyle
    {
        get
        {
            if (_windows.Count == 0)
                return NativeMethods.WindowShowStyle.Hide;
            return GetWindowShowStyle(_windows[0]);
        }
    }

    public int WindowStyles
    {
        get
        {
            if (_windows.Count == 0)
                return 0;
            return NativeMethods.GetWindowLong(_windows[0], NativeMethods.GWL.GWL_STYLE);
        }
    }

    public int ExtendedWindowStyles
    {
        get
        {
            if (_windows.Count == 0)
                return 0;
            return NativeMethods.GetWindowLong(_windows[0], NativeMethods.GWL.GWL_EXSTYLE);
        }
    }

    public bool CanAddToTaskbar
    {
        get
        {
            if (_windows.Count == 0)
                return true;
            int extendedWindowStyles = ExtendedWindowStyles;
            bool isWindow = NativeMethods.IsWindow(_windows[0]);
            bool isVisible = NativeMethods.IsWindowVisible(_windows[0]);
            bool isToolWindow = (extendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_TOOLWINDOW) != 0;
            bool isAppWindow = (extendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_APPWINDOW) != 0;
            bool isNoActivate = (extendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_NOACTIVATE) != 0;
            bool isDeleted = NativeMethods.GetProp(_windows[0], "ITaskList_Deleted") != IntPtr.Zero;
            IntPtr ownerWin = NativeMethods.GetWindow(_windows[0], NativeMethods.GetWindow_Cmd.GW_OWNER);

            return isWindow && isVisible && (ownerWin == IntPtr.Zero || isAppWindow) && (!isNoActivate || isAppWindow) && !isToolWindow && !isDeleted;
        }
    }

    // True if this window should be shown in the taskbar
    public bool ShowInTaskbar
    {
        get
        {
            if (_showInTaskbar == null)
                SetShowInTaskbar();

            return (bool)_showInTaskbar;
        }
    }

    public void SetShowInTaskbar()
    {
        bool showInTaskbar = GetShowInTaskbar();

        if (_showInTaskbar != showInTaskbar)
        {
            _showInTaskbar = showInTaskbar;

            // If we are becoming visible in the taskbar, get the category if it hasn't been set yet
            if (_showInTaskbar == true && Category == null)
                Category = _tasksService.TaskCategoryProvider?.GetCategory(this);

            OnPropertyChanged(nameof(ShowInTaskbar));
        }
    }

    private bool GetShowInTaskbar()
    {
        // EnumWindows and ShellHook return UWP app windows that are 'cloaked', which should not be visible in the taskbar.
        if (EnvironmentHelper.IsWindows8OrBetter && _windows.Count > 0)
        {
            int cbSize = Marshal.SizeOf(typeof(uint));
            NativeMethods.DwmGetWindowAttribute(_windows[0], NativeMethods.DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out uint cloaked, cbSize);

            if (cloaked > 0)
            {
                ShellLogger.Debug($"ApplicationWindow: Cloaked ({cloaked}) window ({Title}) hidden from taskbar");
                return false;
            }

            // UWP shell windows that are not cloaked should be hidden from the taskbar, too.
            StringBuilder cName = new(256);
            NativeMethods.GetClassName(_windows[0], cName, cName.Capacity);
            string className = cName.ToString();
            if ((className == "ApplicationFrameWindow" || className == "Windows.UI.Core.CoreWindow") && (ExtendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_WINDOWEDGE) == 0)
            {
                ShellLogger.Debug($"ApplicationWindow: Hiding UWP non-window {Title}");
                return false;
            }
        }

        return CanAddToTaskbar;
    }

    internal void Uncloak()
    {
        ShellLogger.Debug($"ApplicationWindow: Uncloak event received for {Title}");

        SetShowInTaskbar();
    }

    private void LoadUWPIcon()
    {
        // UWP apps
        try
        {
            UWPInterop.StoreApp storeApp = UWPInterop.StoreAppHelper.AppList.GetAppByAumid(AppUserModelID);

            if (storeApp != null)
                Icon = storeApp.GetIconImageSource(_tasksService.TaskIconSize);
            else
                Icon = IconImageConverter.GetDefaultIcon();
        }
        catch (Exception)
        {
            if (_icon == null)
                Icon = IconImageConverter.GetDefaultIcon();
        }
    }

    private void SetIcon()
    {
        if (!_iconLoading && ShowInTaskbar && _windows.Count > 0)
        {
            _iconLoading = true;

            Task.Factory.StartNew(() =>
            {
                if (IsUWP && !string.IsNullOrEmpty(AppUserModelID))
                {
                    LoadUWPIcon();
                }
                else
                {
                    // non-UWP apps
                    IntPtr hIco = default;
                    uint WM_GETICON = (uint)NativeMethods.WM.GETICON;
                    uint WM_QUERYDRAGICON = (uint)NativeMethods.WM.QUERYDRAGICON;
                    int GCL_HICON = -14;
                    int GCL_HICONSM = -34;
                    IconSize sizeSetting = _tasksService.TaskIconSize;

                    if (sizeSetting == IconSize.Small)
                    {
                        NativeMethods.SendMessageTimeout(_windows[0], WM_GETICON, 2, 0, 2, 1000, ref hIco);
                        if (hIco == IntPtr.Zero)
                            NativeMethods.SendMessageTimeout(_windows[0], WM_GETICON, 0, 0, 2, 1000, ref hIco);
                    }
                    else
                        NativeMethods.SendMessageTimeout(_windows[0], WM_GETICON, 1, 0, 2, 1000, ref hIco);

                    if (hIco == IntPtr.Zero && sizeSetting == IconSize.Small)
                    {
                        if (!Environment.Is64BitProcess)
                            hIco = NativeMethods.GetClassLong(_windows[0], GCL_HICONSM);
                        else
                            hIco = NativeMethods.GetClassLongPtr(_windows[0], GCL_HICONSM);
                    }

                    if (hIco == IntPtr.Zero && _windows.Count > 0)
                    {
                        if (!Environment.Is64BitProcess)
                            hIco = NativeMethods.GetClassLong(_windows[0], GCL_HICON);
                        else
                            hIco = NativeMethods.GetClassLongPtr(_windows[0], GCL_HICON);
                    }

                    if (hIco == IntPtr.Zero && _windows.Count > 0)
                        NativeMethods.SendMessageTimeout(_windows[0], WM_QUERYDRAGICON, 0, 0, 0, 1000, ref hIco);

                    if (hIco == IntPtr.Zero && _icon == null && ShellHelper.Exists(WinFileName))
                    {
                        // last resort: find icon by executable. if we already have an icon from a previous fetch, then just skip this
                        IconSize size = IconSize.Small;
                        if (sizeSetting != size)
                            size = IconSize.Large;

                        hIco = IconHelper.GetIconByFilename(WinFileName, size, out _);
                    }

                    if (hIco != IntPtr.Zero)
                    {
                        if (_hIcon != hIco)
                        {
                            _hIcon = hIco;
                            bool returnDefault = (_icon == null); // only return a default icon if we don't already have one. otherwise let's use what we have.
                            ImageSource icon = IconImageConverter.GetImageFromHIcon(hIco, returnDefault);
                            if (icon != null)
                            {
                                icon.Freeze();
                                Icon = icon;
                            }
                        }
                        else
                            NativeMethods.DestroyIcon(hIco);
                    }
                }

                _iconLoading = false;
            }, CancellationToken.None, TaskCreationOptions.None, IconHelper.IconScheduler);
        }
    }

    internal IntPtr GetMonitor(IntPtr? hWnd = null)
    {
        if (!hWnd.HasValue)
            return NativeMethods.MonitorFromWindow(_windows[0], NativeMethods.EMonitorFromWindow.DefaultToNearest);
        return NativeMethods.MonitorFromWindow(hWnd.Value, NativeMethods.EMonitorFromWindow.DefaultToNearest);
    }

    internal NativeMethods.ShortRect GetButtonRectFromShell()
    {
        NativeMethods.ShortRect rect = new();
        GetButtonRect?.Invoke(ref rect);
        return rect;
    }

    public void SetOverlayIcon(IntPtr hIcon)
    {
        if (hIcon == IntPtr.Zero)
        {
            OverlayIcon = null;
            return;
        }

        ImageSource icon = IconImageConverter.GetImageFromHIcon(hIcon, false);
        if (icon != null)
        {
            icon.Freeze();
            OverlayIcon = icon;
        }
    }

    public void SetOverlayIconDescription(IntPtr lParam)
    {
        try
        {
            if (ProcId is uint procId)
            {
                if (lParam == IntPtr.Zero)
                    return;

                IntPtr hShared = NativeMethods.SHLockShared(lParam, procId);

                if (hShared == IntPtr.Zero)
                    return;

                string str = Marshal.PtrToStringAuto(hShared);
                NativeMethods.SHUnlockShared(hShared);

                OverlayIconDescription = str;
            }
        }
        catch (Exception e)
        {
            ShellLogger.Error($"ApplicationWindow: Unable to get overlay icon description from process {Title}: {e.Message}");
        }
    }

    internal void UpdateProperties(IntPtr handle = default)
    {
        lock (_lockUpdate)
        {
            if (handle != IntPtr.Zero && !_windows.Contains(handle))
                _windows.Add(handle);
            if (_windows.Count > 1)
                State = WindowState.Unknown;
            OnPropertyChanged(nameof(Launched));
            OnPropertyChanged(nameof(MultipleInstanceLaunched));
            SetTitle();
            SetShowInTaskbar();
            SetIcon();
            SetClassName();
        }
    }

    public void BringToFront(IntPtr handle = default)
    {
        if (handle == default)
            handle = _windows[0];

        // call restore if window is minimized
        if (IsMinimized(handle))
            Restore(handle);
        else
        {
            NativeMethods.ShowWindow(handle, NativeMethods.WindowShowStyle.Show);
            NativeMethods.SetForegroundWindow(handle);

            // some stubborn windows (Outlook) start flashing while already active, this lets us stop
            if (State == WindowState.Flashing)
                State = WindowState.Active;
        }
    }

    public void Minimize()
    {
        if ((WindowStyles & (int)NativeMethods.WindowStyles.WS_MINIMIZEBOX) != 0)
        {
            IntPtr retval = IntPtr.Zero;
            NativeMethods.SendMessageTimeout(_windows[0], (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_MINIMIZE, 0, 2, 200, ref retval);
        }
    }

    public void Restore(IntPtr handle = default)
    {
        if (handle == default)
            handle = _windows[0];
        IntPtr retval = IntPtr.Zero;
        NativeMethods.SendMessageTimeout(handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_RESTORE, 0, 2, 200, ref retval);

        NativeMethods.SetForegroundWindow(handle);
    }

    public void Maximize()
    {
        bool maximizeResult = NativeMethods.ShowWindow(_windows[0], NativeMethods.WindowShowStyle.Maximize);
        if (!maximizeResult)
        {
            // we don't have a fallback for elevated windows here since our only hope, SC_MAXIMIZE, doesn't seem to work for them. fall back to restore.
            IntPtr retval = IntPtr.Zero;
            NativeMethods.SendMessageTimeout(_windows[0], (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_RESTORE, 0, 2, 200, ref retval);
        }
        NativeMethods.SetForegroundWindow(_windows[0]);
    }

    internal IntPtr DoClose()
    {
        IntPtr retval = IntPtr.Zero;
        NativeMethods.SendMessageTimeout(_windows[0], (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_CLOSE, 0, 2, 200, ref retval);

        return retval;
    }

    public void Close()
    {
        _tasksService.CloseWindow(this);
    }

    public void Move()
    {
        // move window via arrow keys; must be active window to control
        BringToFront();
        IntPtr retval = IntPtr.Zero;
        NativeMethods.SendMessageTimeout(_windows[0], (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_MOVE, 0, 2, 200, ref retval);
    }

    public void Size()
    {
        // size window via arrow keys; must be active window to control
        BringToFront();
        IntPtr retval = IntPtr.Zero;
        NativeMethods.SendMessageTimeout(_windows[0], (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_SIZE, 0, 2, 200, ref retval);
    }

    public DateTime DateStart
    {
        get
        {
            if (!_dateStart.HasValue)
            {
                _dateStart = DateTime.MinValue;
                if (_procId.HasValue)
                {
                    Process process = Process.GetProcessById((int)_procId.Value);
                    if (process != null)
                        _dateStart = process.StartTime;
                }
            }

            return _dateStart.Value;
        }
    }

    /// <summary>
    /// Returns whether a window is normal (1), minimized (2), or maximized (3).
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    private static NativeMethods.WindowShowStyle GetWindowShowStyle(IntPtr hWnd)
    {
        NativeMethods.WindowPlacement placement = new();
        NativeMethods.GetWindowPlacement(hWnd, ref placement);
        return placement.showCmd;
    }

    public bool Launched
    {
        get { return _windows.Count > 0; }
    }

    public List<IntPtr> ListWindows
    {
        get { return _windows; }
    }

    #region IEquatable<Window> Members

    public Guid Id
    {
        get { return _id; }
    }

    public bool Equals(ApplicationWindow other)
    {
        if (_windows.Count == 0 || other.ListWindows.Count == 0)
            return other.Id == Id;
        return _windows[0].Equals(other.ListWindows[0]);
    }

    #endregion

    #region INotifyPropertyChanged Members

#nullable enable

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName()] string? PropertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

#nullable disable

    #endregion

    public string Arguments { get; set; }

    public enum WindowState
    {
        Active,
        Inactive,
        Hidden,
        Flashing,
        Unknown = 999,
    }

    public bool IsPinnedApp { get; set; }

    public string PinnedShortcut { get; set; }

    private NativeMethods.IPropertyStore _propStore;
    public NativeMethods.IPropertyStore PropertyStore
    {
        get
        {
            if (_propStore == null && _windows[0] != IntPtr.Zero)
            {
                Guid guid = typeof(NativeMethods.IPropertyStore).GUID;
                NativeMethods.SHGetPropertyStoreForWindow(_windows[0], ref guid, out _propStore);
            }
            return _propStore;
        }
    }

    public bool StartNewInstance(string arguments = null)
    {
        if (IsUWP)
            return ShellHelper.StartProcess("explorer.exe", $"shell:AppsFolder\\{AppUserModelID}", false);
        else
            return ShellHelper.StartProcess(WinFileName, (string.IsNullOrWhiteSpace(arguments) ? Arguments : arguments), false);
    }

    public int ParentProcessId()
    {
        NativeMethods.ProcessEntry32 pe32 = new()
        {
            dwSize = (uint)Marshal.SizeOf(typeof(NativeMethods.ProcessEntry32)),
        };
        using var hSnapshot = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.Snapshot.Process, _procId.Value);
        if (hSnapshot.IsInvalid)
            throw new Win32Exception();

        if (!NativeMethods.Process32First(hSnapshot, ref pe32))
        {
            int errno = Marshal.GetLastWin32Error();
            if (errno == (int)NativeMethods.HResult.ERROR_NO_MORE_FILES)
                return -1;
            throw new Win32Exception(errno);
        }
        do
        {
            if (pe32.th32ProcessID == _procId.Value)
                return (int)pe32.th32ParentProcessID;
        } while (NativeMethods.Process32Next(hSnapshot, ref pe32));
        return -1;
    }

    public void SetThumbButtons(ThumbButton[] buttons)
    {
        _thumbButtons = buttons;
        OnPropertyChanged(nameof(ThumbButtons));
    }

    public ThumbButton[] ThumbButtons
    {
        get { return _thumbButtons; }
    }
}
