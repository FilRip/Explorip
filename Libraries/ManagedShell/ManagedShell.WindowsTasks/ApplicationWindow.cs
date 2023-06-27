using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

namespace ManagedShell.WindowsTasks
{
    [DebuggerDisplay("Title: {Title}, Handle: {Handle}")]
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

        public ApplicationWindow(TasksService tasksService, IntPtr handle)
        {
            _hIcon = IntPtr.Zero;
            _appUserModelId = null;
            _winFileName = "";
            _isUWP = null;
            _lockUpdate = new object();
            _windows = new List<IntPtr>();
            _tasksService = tasksService;
            Handle = handle;
            State = WindowState.Inactive;
        }

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
                    // no longer required
                    _windows.Clear();
                    _icon = null;
                    if (_hIcon != IntPtr.Zero)
                        NativeMethods.DestroyIcon(_hIcon);
                }
                _isDisposed = true;
            }
        }

        public IntPtr Handle { get; set; }

        public string AppUserModelID
        {
            get
            {
                if (string.IsNullOrEmpty(_appUserModelId) && (Handle != IntPtr.Zero || _windows.Count > 0))
                {
                    _appUserModelId = ShellHelper.GetAppUserModelIdPropertyForHandle(Handle == IntPtr.Zero ? _windows[0] : Handle);
                }

                return _appUserModelId;
            }
        }

        public bool IsUWP
        {
            get
            {
                _isUWP ??= WinFileName.ToLower().Contains("applicationframehost.exe");

                return (bool)_isUWP;
            }
        }

        public string WinFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_winFileName) && Handle != IntPtr.Zero)
                {
                    _winFileName = ShellHelper.GetPathForHandle(Handle);
                }

                return _winFileName;
            }
            set
            {
                if (_winFileName != value)
                {
                    _winFileName = value;
                }
            }
        }

        public uint? ProcId
        {
            get
            {
                if (!_procId.HasValue && Handle != IntPtr.Zero)
                    _procId = ShellHelper.GetProcIdForHandle(Handle);
                return _procId;
            }
        }

        public int Position { get; set; }

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
                {
                    SetTitle();
                }

                return _title;
            }
            private set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private void SetTitle()
        {
            string title = "";
            try
            {
                StringBuilder stringBuilder = new(MAX_STRING_SIZE);
                NativeMethods.GetWindowText(Handle, stringBuilder, MAX_STRING_SIZE);

                title = stringBuilder.ToString();
            }
            catch { /* Ignore errors */ }

            if (_title != title)
            {
                Title = title;
            }
        }

        public void SetTitle(string newTitle)
        {
            Title = newTitle;
        }

        public string ClassName
        {
            get
            {
                if (_className == null)
                {
                    SetClassName();
                }
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
                NativeMethods.GetClassName(Handle, stringBuilder, MAX_STRING_SIZE);

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
                _icon = value;
                OnPropertyChanged();
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
                _overlayIcon = value;
                OnPropertyChanged();
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
                _overlayIconDescription = value;
                OnPropertyChanged();
            }
        }

        public NativeMethods.TBPFLAG ProgressState
        {
            get
            {
                return _progressState;
            }

            set
            {
                _progressState = value;

                if (value == NativeMethods.TBPFLAG.TBPF_NOPROGRESS)
                {
                    ProgressValue = 0;
                }

                OnPropertyChanged();
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
            }
        }

        public WindowState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public bool IsMinimized(IntPtr handle = default)
        {
            if (handle == default)
                handle = Handle;
            if (handle == IntPtr.Zero && _windows.Count == 1)
                handle = _windows[0];
            if (handle == IntPtr.Zero)
                return true;
            return NativeMethods.IsIconic(handle);
        }

        public NativeMethods.WindowShowStyle ShowStyle
        {
            get
            {
                if (Handle == IntPtr.Zero && _windows.Count == 0)
                    return NativeMethods.WindowShowStyle.Hide;
                return GetWindowShowStyle(Handle == IntPtr.Zero ? _windows[0] : Handle);
            }
        }

        public int WindowStyles
        {
            get
            {
                if (Handle == IntPtr.Zero && _windows.Count == 0)
                    return 0;
                return NativeMethods.GetWindowLong(Handle == IntPtr.Zero ? _windows[0] : Handle, NativeMethods.GWL_STYLE);
            }
        }

        public int ExtendedWindowStyles
        {
            get
            {
                if (Handle == IntPtr.Zero && _windows.Count == 0)
                    return 0;
                return NativeMethods.GetWindowLong(Handle == IntPtr.Zero ? _windows[0] : Handle, NativeMethods.GWL_EXSTYLE);
            }
        }

        public bool CanAddToTaskbar
        {
            get
            {
                if (Handle == IntPtr.Zero && _windows.Count == 0)
                    return true;
                int extendedWindowStyles = ExtendedWindowStyles;
                bool isWindow = NativeMethods.IsWindow(Handle == IntPtr.Zero ? _windows[0] : Handle);
                bool isVisible = NativeMethods.IsWindowVisible(Handle == IntPtr.Zero ? _windows[0] : Handle);
                bool isToolWindow = (extendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_TOOLWINDOW) != 0;
                bool isAppWindow = (extendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_APPWINDOW) != 0;
                bool isNoActivate = (extendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_NOACTIVATE) != 0;
                IntPtr ownerWin = NativeMethods.GetWindow(Handle == IntPtr.Zero ? _windows[0] : Handle, NativeMethods.GetWindow_Cmd.GW_OWNER);

                return isWindow && isVisible && (ownerWin == IntPtr.Zero || isAppWindow) && (!isNoActivate || isAppWindow) && !isToolWindow;
            }
        }

        // True if this window should be shown in the taskbar
        public bool ShowInTaskbar
        {
            get
            {
                if (_showInTaskbar == null)
                {
                    SetShowInTaskbar();
                }

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
                {
                    Category = _tasksService.TaskCategoryProvider?.GetCategory(this);
                }

                OnPropertyChanged(nameof(ShowInTaskbar));
            }
        }

        private bool GetShowInTaskbar()
        {
            // EnumWindows and ShellHook return UWP app windows that are 'cloaked', which should not be visible in the taskbar.
            if (EnvironmentHelper.IsWindows8OrBetter && (Handle != IntPtr.Zero || _windows.Count > 0))
            {
                int cbSize = Marshal.SizeOf(typeof(uint));
                NativeMethods.DwmGetWindowAttribute(Handle == IntPtr.Zero ? _windows[0] : Handle, NativeMethods.DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out uint cloaked, cbSize);

                if (cloaked > 0)
                {
                    ShellLogger.Debug($"ApplicationWindow: Cloaked ({cloaked}) window ({Title}) hidden from taskbar");
                    return false;
                }

                // UWP shell windows that are not cloaked should be hidden from the taskbar, too.
                StringBuilder cName = new(256);
                NativeMethods.GetClassName(Handle == IntPtr.Zero ? _windows[0] : Handle, cName, cName.Capacity);
                string className = cName.ToString();
                if (className == "ApplicationFrameWindow" || className == "Windows.UI.Core.CoreWindow")
                {
                    if ((ExtendedWindowStyles & (int)NativeMethods.ExtendedWindowStyles.WS_EX_WINDOWEDGE) == 0)
                    {
                        ShellLogger.Debug($"ApplicationWindow: Hiding UWP non-window {Title}");
                        return false;
                    }
                }
                else if (!EnvironmentHelper.IsWindows10OrBetter && (className == "ImmersiveBackgroundWindow" || className == "SearchPane" || className == "NativeHWNDHost" || className == "Shell_CharmWindow" || className == "ImmersiveLauncher") && WinFileName.ToLower().Contains("explorer.exe"))
                {
                    ShellLogger.Debug($"ApplicationWindow: Hiding immersive shell window {Title}");
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

        private void SetIcon()
        {
            if (!_iconLoading && ShowInTaskbar && (Handle != IntPtr.Zero || _windows.Count > 0))
            {
                _iconLoading = true;

                Task.Factory.StartNew(() =>
                {
                    if (IsUWP && !string.IsNullOrEmpty(AppUserModelID))
                    {
                        // UWP apps
                        try
                        {
                            UWPInterop.StoreApp storeApp = UWPInterop.StoreAppHelper.AppList.GetAppByAumid(AppUserModelID);

                            if (storeApp != null)
                            {
                                Icon = storeApp.GetIconImageSource(_tasksService.TaskIconSize);
                            }
                            else
                            {
                                Icon = IconImageConverter.GetDefaultIcon();
                            }
                        }
                        catch
                        {
                            if (_icon == null) Icon = IconImageConverter.GetDefaultIcon();
                        }
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
                            NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, WM_GETICON, 2, 0, 2, 1000, ref hIco);
                            if (hIco == IntPtr.Zero)
                                NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, WM_GETICON, 0, 0, 2, 1000, ref hIco);
                        }
                        else
                        {
                            NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, WM_GETICON, 1, 0, 2, 1000, ref hIco);
                        }

                        if (hIco == IntPtr.Zero && sizeSetting == IconSize.Small)
                        {
                            if (!Environment.Is64BitProcess)
                                hIco = NativeMethods.GetClassLong(Handle == IntPtr.Zero ? _windows[0] : Handle, GCL_HICONSM);
                            else
                                hIco = NativeMethods.GetClassLongPtr(Handle == IntPtr.Zero ? _windows[0] : Handle, GCL_HICONSM);
                        }

                        if (hIco == IntPtr.Zero && (Handle != IntPtr.Zero || _windows.Count > 0))
                        {
                            if (!Environment.Is64BitProcess)
                                hIco = NativeMethods.GetClassLong(Handle == IntPtr.Zero ? _windows[0] : Handle, GCL_HICON);
                            else
                                hIco = NativeMethods.GetClassLongPtr(Handle == IntPtr.Zero ? _windows[0] : Handle, GCL_HICON);
                        }

                        if (hIco == IntPtr.Zero && (Handle != IntPtr.Zero || _windows.Count > 0))
                        {
                            NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, WM_QUERYDRAGICON, 0, 0, 0, 1000, ref hIco);
                        }

                        if (hIco == IntPtr.Zero && _icon == null && ShellHelper.Exists(WinFileName))
                        {
                            // last resort: find icon by executable. if we already have an icon from a previous fetch, then just skip this
                            IconSize size = IconSize.Small;
                            if (sizeSetting != size)
                                size = IconSize.Large;

                            hIco = IconHelper.GetIconByFilename(WinFileName, size);
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
                            {
                                NativeMethods.DestroyIcon(hIco);
                            }
                        }
                    }

                    _iconLoading = false;
                }, CancellationToken.None, TaskCreationOptions.None, IconHelper.IconScheduler);
            }
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
                    {
                        return;
                    }

                    IntPtr hShared = NativeMethods.SHLockShared(lParam, procId);

                    if (hShared == IntPtr.Zero)
                    {
                        return;
                    }

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
                {
                    _windows.Add(handle);
                    if (Handle != IntPtr.Zero)
                    {
                        _windows.Insert(0, Handle);
                        Handle = IntPtr.Zero;
                    }
                }
                if (_windows.Count > 1)
                    State = WindowState.Unknown;
                OnPropertyChanged(nameof(Launched));
                SetTitle();
                SetShowInTaskbar();
                SetIcon();
                SetClassName();
            }
        }

        public void BringToFront(IntPtr handle = default)
        {
            if (handle == default)
                handle = Handle;

            if (handle == IntPtr.Zero && _windows.Count == 1)
                handle = _windows[0];

            // call restore if window is minimized
            if (IsMinimized(handle))
            {
                Restore(handle);
            }
            else
            {
                NativeMethods.ShowWindow(handle, NativeMethods.WindowShowStyle.Show);
                NativeMethods.SetForegroundWindow(handle);

                if (State == WindowState.Flashing) State = WindowState.Active; // some stubborn windows (Outlook) start flashing while already active, this lets us stop
            }
        }

        public void Minimize()
        {
            if ((WindowStyles & (int)NativeMethods.WindowStyles.WS_MINIMIZEBOX) != 0)
            {
                IntPtr retval = IntPtr.Zero;
                NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_MINIMIZE, 0, 2, 200, ref retval);
            }
        }

        public void Restore(IntPtr handle = default)
        {
            if (handle == default)
                handle = Handle;
            if (handle == IntPtr.Zero && _windows.Count == 1)
                handle = _windows[0];
            IntPtr retval = IntPtr.Zero;
            NativeMethods.SendMessageTimeout(handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_RESTORE, 0, 2, 200, ref retval);

            NativeMethods.SetForegroundWindow(handle);
        }

        public void Maximize()
        {
            bool maximizeResult = NativeMethods.ShowWindow(Handle == IntPtr.Zero ? _windows[0] : Handle, NativeMethods.WindowShowStyle.Maximize);
            if (!maximizeResult)
            {
                // we don't have a fallback for elevated windows here since our only hope, SC_MAXIMIZE, doesn't seem to work for them. fall back to restore.
                IntPtr retval = IntPtr.Zero;
                NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_RESTORE, 0, 2, 200, ref retval);
            }
            NativeMethods.SetForegroundWindow(Handle == IntPtr.Zero ? _windows[0] : Handle);
        }

        internal IntPtr DoClose()
        {
            IntPtr retval = IntPtr.Zero;
            NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_CLOSE, 0, 2, 200, ref retval);

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
            NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_MOVE, 0, 2, 200, ref retval);
        }

        public void Size()
        {
            // size window via arrow keys; must be active window to control
            BringToFront();
            IntPtr retval = IntPtr.Zero;
            NativeMethods.SendMessageTimeout(Handle == IntPtr.Zero ? _windows[0] : Handle, (int)NativeMethods.WM.SYSCOMMAND, NativeMethods.SC_SIZE, 0, 2, 200, ref retval);
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
                        {
                            _dateStart = process.StartTime;
                        }
                    }
                }

                return _dateStart.Value;
            }
        }

        /// <summary>
        /// Returns whether a window is normal (1), minimized (2), or maximized (3).
        /// </summary>
        /// <param name="hWnd">The handle of the window.</param>
        private NativeMethods.WindowShowStyle GetWindowShowStyle(IntPtr hWnd)
        {
            NativeMethods.WindowPlacement placement = new();
            NativeMethods.GetWindowPlacement(hWnd, ref placement);
            return placement.showCmd;
        }

        public bool Launched
        {
            get { return Handle != IntPtr.Zero || _windows.Count > 0; }
        }

        public List<IntPtr> ListWindows
        {
            get { return _windows; }
        }

        #region IEquatable<Window> Members

        public bool Equals(ApplicationWindow other)
        {
            return Handle.Equals(other.Handle);
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
            Unknown = 999
        }

        public bool IsPinnedApp { get; set; }
    }
}
