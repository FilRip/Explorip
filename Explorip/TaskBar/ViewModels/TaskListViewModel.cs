using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Constants;
using Explorip.Helpers;
using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

using VirtualDesktop;
using VirtualDesktop.Models;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskListViewModel : ObservableObject, IDisposable
{
    private const int ScrollBarWidth = 22;

    private AppBarEdge _currentEdge;
    private Taskbar _taskbarParent;
    private readonly object _lockChangeDesktop;
    private bool disposedValue;
    private static Guid _currentVirtualDesktopId;

    [ObservableProperty()]
    private ICollectionView _taskListCollection;
    [ObservableProperty()]
    private GridLength _buttonWidth, _buttonHeight;
    [ObservableProperty()]
    private GridLength _buttonRightMargin, _buttonBottomMargin;
    [ObservableProperty()]
    private double _taskButtonLength, _maxTitleLength;

    public TaskListViewModel() : base()
    {
        _lockChangeDesktop = new object();
        SetCurrentVirtualDesktop();
        RebuildCollectionView();
    }

    private static void SetCurrentVirtualDesktop()
    {
        _currentVirtualDesktopId = VirtualDesktopManager.IsInitialized ? VirtualDesktopManager.Current.Id : Guid.Empty;
    }

    public static void RebuildListWindows()
    {
        ((MyTaskbarApp)Application.Current).MainTaskbar.MyTaskList.MyDataContext.FirstRefresh();
    }

    public static void RefreshAllCollectionView(object sender, EventArgs e)
    {
        ERefreshList function = ERefreshList.None;
        if (sender is ERefreshList rl)
            function = rl;
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (TaskListViewModel tl in ((MyTaskbarApp)Application.Current).ListAllTaskbar().Select(t => t.MyTaskList.MyDataContext))
            {
                if (function.HasFlag(ERefreshList.Rebuild))
                    tl.RebuildCollectionView();
                if (function.HasFlag(ERefreshList.Refresh))
                    tl.RefreshMyCollectionView();
                if (ConfigManager.ReduceTitleWidthWhenTaskbarFull)
                    tl.UpdateMaxWidth();
            }
        });
    }

    public void RefreshMyCollectionView()
    {
        ShellLogger.Debug($"Refresh TaskList of screen {TaskbarParent.NumScreen}");
        TaskListCollection.Refresh();
    }

    public void RebuildCollectionView()
    {
        ShellLogger.Debug($"Rebuild TaskList of screen {TaskbarParent?.NumScreen}");
        TaskListCollection = System.Windows.Data.CollectionViewSource.GetDefaultView(MyTaskbarApp.MyShellManager.TasksService.Windows);
        TaskListCollection.SortDescriptions.Add(new SortDescription(nameof(ApplicationWindow.Position), ListSortDirection.Ascending));
        TaskListCollection.Filter = FilterAppWindow;
    }

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(PanelOrientation));
        ChangeButtonSize();
    }

    public Taskbar TaskbarParent
    {
        get { return _taskbarParent; }
        set
        {
            if (value != null)
            {
                _taskbarParent = value;
                if (_taskbarParent.MainScreen)
                {
                    RemoveTaskServiceEvent();
                    if (VirtualDesktopManager.IsInitialized)
                        VirtualDesktopEvents.CurrentChanged += VirtualDesktop_CurrentChanged;
                    MyTaskbarApp.MyShellManager.TasksService.Windows.CollectionChanged += Windows_CollectionChanged;
                    MyTaskbarApp.MyShellManager.TasksService.WindowUncloaked += RefreshView;
                    MyTaskbarApp.MyShellManager.TasksService.FullScreenChanged += TasksService_FullScreenChanged;
                }
                ChangeButtonSize();
            }
        }
    }

    private static void TasksService_FullScreenChanged(object sender, FullScreenEventArgs e)
    {
        if (e.IsExiting && e.Handle != IntPtr.Zero &&
            e.ProcessId == Process.GetProcessesByName("explorer")?[0].Id &&
            e.Title == Constants.Localization.ACTIVES_APPLICATIONS)
        {
            // In case it's task manager who have been leaved, some windows may be changed virtual desktop, we must refresh all
            foreach (ApplicationWindow appWin in MyTaskbarApp.MyShellManager.TasksService.Windows)
                foreach (ApplicationWindowsProperty appWinProp in appWin.ListWindows)
                    appWinProp.VirtualDesktopId = ReturnVirtualDesktopId(appWinProp.Handle);
            RefreshView(null, null);
        }
    }

    private static void Windows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && VirtualDesktopManager.IsInitialized)
        {
            foreach (ApplicationWindow appWin in e.NewItems)
                foreach (ApplicationWindowsProperty appWinProp in appWin.ListWindows.Where(w => w.VirtualDesktopId == Guid.Empty))
                    appWinProp.VirtualDesktopId = ReturnVirtualDesktopId(appWinProp.Handle);
        }
        RefreshView(null, null);
    }

    private static Guid ReturnVirtualDesktopId(IntPtr handle)
    {
        Guid virtualDesktopId = Guid.Empty;
        int nbTry = 0;
        // Sometimes, for unknown reasons, the get virtual desktop of window handle not working the first time
        // So we loop, until it return the virtual desktop of the window (maximum 5 try, in case of reality not working and break initiny loop)
        while (virtualDesktopId == Guid.Empty && nbTry < 5)
        {
            nbTry++;
            try
            {
                virtualDesktopId = VirtualDesktopManager.FromHwnd(handle).Id;
            }
            catch (Exception) { /* Ignore errors */ }
            Thread.Sleep(10);
        }
        return virtualDesktopId;
    }

    private static void RefreshView(object sender, WindowEventArgs e)
    {
        RefreshAllCollectionView(ERefreshList.Refresh, EventArgs.Empty);
    }

    public void ChangeButtonSize()
    {
        ButtonWidth = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskButtonSize + 20, GridUnitType.Pixel);
        ButtonHeight = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskButtonSize + 13, GridUnitType.Pixel);
        TaskButtonLength = ButtonWidth.Value;
        if (ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).ShowTitleApplicationWindow)
        {
            ButtonWidth = GridLength.Auto;
            TaskButtonLength += ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).MaxWidthTitleApplicationWindow;
        }

        ButtonRightMargin = new GridLength(0, GridUnitType.Pixel);
        ButtonBottomMargin = new GridLength(0, GridUnitType.Pixel);
        if (_currentEdge == AppBarEdge.Left || _currentEdge == AppBarEdge.Right)
            ButtonBottomMargin = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).SpaceBetweenTaskButton, GridUnitType.Pixel);
        else
            ButtonRightMargin = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).SpaceBetweenTaskButton, GridUnitType.Pixel);
    }

    public void UpdateMaxWidth()
    {
        if (!ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).ShowTitleApplicationWindow || !TaskbarParent.MyDataContext.TaskbarVisible)
            return;
        double currentWidth = TaskbarParent.MyTaskList.TasksList.Items.Count * (ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskButtonSize + 20 + ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).MaxWidthTitleApplicationWindow + ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).SpaceBetweenTaskButton);
        double minWidth = ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskButtonSize + 20;
        if (currentWidth > TaskbarParent.MyTaskList.ActualWidth)
        {
            double newMaxWidth = (TaskbarParent.MyTaskList.ActualWidth - ScrollBarWidth - (ButtonRightMargin.Value + ButtonBottomMargin.Value) * TaskbarParent.MyTaskList.TasksList.Items.Count) / TaskbarParent.MyTaskList.TasksList.Items.Count;
            TaskButtonLength = Math.Max(minWidth, newMaxWidth);
        }
        else
            TaskButtonLength = ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskButtonSize + 20 + ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).MaxWidthTitleApplicationWindow;
    }

    partial void OnTaskButtonLengthChanged(double value)
    {
        MaxTitleLength = value - (ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskButtonSize + 20 + ButtonBottomMargin.Value + ButtonRightMargin.Value);
    }

    public void RemoveTaskServiceEvent()
    {
        if (TaskbarParent.MainScreen)
        {
            if (VirtualDesktopManager.IsInitialized)
                VirtualDesktopEvents.CurrentChanged -= VirtualDesktop_CurrentChanged;
            MyTaskbarApp.MyShellManager.TasksService.Windows.CollectionChanged -= Windows_CollectionChanged;
            MyTaskbarApp.MyShellManager.TasksService.WindowUncloaked -= RefreshView;
            MyTaskbarApp.MyShellManager.TasksService.FullScreenChanged -= TasksService_FullScreenChanged;
        }
    }

    private static bool FilterAppWindow(object item)
    {
        if (item is ApplicationWindow window &&
            window.ShowInTaskbar &&
            !window.IsDisposed)
        {
            if (window.IsPinnedApp)
                return true;
            if (window.ListWindows.Count > 0)
                return window.ListWindows[0].VirtualDesktopId == _currentVirtualDesktopId;
        }

        return false;
    }

    public Orientation PanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }

    private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
    {
        ShellLogger.Debug("Rebuild TaskList" + (e != null ? " from new virtual desktop : " + e.NewDesktop.Name : ""));
        if (e?.NewDesktop != null)
            SetCurrentVirtualDesktop();
        Application.Current.Dispatcher.Invoke(() =>
        {
            lock (_lockChangeDesktop)
            {
                DisposeAllApplicationWindow();
                // Browse all Window
                NativeMethods.EnumWindows((hwnd, lParam) =>
                {
                    ApplicationWindow win = new(MyTaskbarApp.MyShellManager.TasksService, hwnd);
                    win.ListWindows[0].VirtualDesktopId = _currentVirtualDesktopId;
                    if (win.CanAddToTaskbar && win.ShowInTaskbar && !MyTaskbarApp.MyShellManager.TasksService.Windows.Contains(win))
                    {
                        MyTaskbarApp.MyShellManager.TasksService.Windows.Add(win);
                        if (ManagedShell.Common.Helpers.EnvironmentHelper.IsAppRunningAsShell)
                            TasksService.SendTaskbarButtonCreatedMessage(hwnd);
                    }
                    return true;
                }, 0);

                InsertPinnedApp();

                // Give default order
                int lastPosition = MyTaskbarApp.MyShellManager.TasksService.Windows.Where(w => w.IsPinnedApp).Max(w => w.Position);
                foreach (ApplicationWindow appWin in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(w => !w.IsPinnedApp))
                    appWin.Position = ++lastPosition;

                // Merge same app in one App if Grouped application window enabled
                if (MyTaskbarApp.MyShellManager.TasksService.GroupApplicationsWindows)
                {
                    bool changed = true;
                    while (changed)
                    {
                        changed = false;
                        foreach (ApplicationWindow appWin in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => aw.ListWindows.Count > 0))
                        {
                            List<ApplicationWindow> listSame = [.. MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => aw != appWin && string.Compare(aw.WinFileName, appWin.WinFileName, StringComparison.OrdinalIgnoreCase) == 0 && aw.ListWindows.Count > 0)];
                            if (listSame.Any())
                            {
                                foreach (ApplicationWindow win in listSame)
                                {
                                    appWin.ListWindows.AddRange(win.ListWindows);
                                    win.ListWindows.Clear();
                                    appWin.SetTitle();
                                    if (appWin.ListWindows.Count > 1)
                                        appWin.State = ApplicationWindow.WindowState.Unknown;
                                    else
                                        appWin.State = win.State;
                                }
                                changed = true;
                                break;
                            }
                        }
                        if (!changed)
                            break;
                    }
                }

                // Remove not valid application window
                IEnumerable<ApplicationWindow> windowsToDispose = [.. MyTaskbarApp.MyShellManager.TasksService.Windows.Where(win => !win.IsPinnedApp && win.ListWindows.Count == 0)];
                foreach (ApplicationWindow win in windowsToDispose)
                    win.Dispose();

                // Redraw task list
                RefreshAllCollectionView(ERefreshList.Rebuild, EventArgs.Empty);
            }
        });
    }

    private void InsertPinnedApp()
    {
        Dictionary<string, int> orders = [];
        try
        {
            string exploripConfigFile = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Config", "exploripTaskbar.ini");
            if (File.Exists(exploripConfigFile))
            {
                string[] lines = File.ReadAllLines(exploripConfigFile);
                string[] splitter;
                foreach (string line in lines)
                {
                    splitter = line.Split('|');
                    if (splitter.Length == 2 && int.TryParse(splitter[0], out int position))
                        orders.Add(splitter[1], position);
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }

        string path = Path.Combine(Environment.SpecialFolder.ApplicationData.FullPath(), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
        if (Directory.Exists(path))
        {
            int numPinnedApp = 0;
            Shortcut pinnedApp;
            ApplicationWindow appWin;
            foreach (string file in Directory.GetFiles(path, "*.lnk").Where(file => !MyTaskbarApp.MyShellManager.TasksService.Windows.Any(win => win.Title == Path.GetFileNameWithoutExtension(file))))
            {
                pinnedApp = Shortcut.ReadFromFile(file);
                appWin = new ApplicationWindow(MyTaskbarApp.MyShellManager.TasksService, IntPtr.Zero)
                {
                    IsPinnedApp = true,
                    PinnedShortcut = file,
                    WinFileName = string.IsNullOrWhiteSpace(pinnedApp.Target) ? Environment.ExpandEnvironmentVariables(@"%windir%\explorer.exe") : pinnedApp.Target,
                    Arguments = pinnedApp.StringData?.CommandLineArguments,
                    WorkingDirectory = pinnedApp.StringData?.WorkingDir,
                };
                if (string.Compare(appWin.WinFileName, Path.Combine(Environment.SpecialFolder.Windows.FullPath(), "explorer.exe"), StringComparison.OrdinalIgnoreCase) == 0 && !string.IsNullOrWhiteSpace(appWin.Arguments) && appWin.Arguments.StartsWith("shell:AppsFolder\\"))
                    appWin.SetIsUWP();
                appWin.SetTitle();
                if (orders.TryGetValue(Path.GetFileName(file), out int position))
                    appWin.Position = position;
                if (string.IsNullOrWhiteSpace(appWin.WinFileName))
                {
                    Debug.WriteLine($"Unable to add {file} as pinned app");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(pinnedApp.StringData?.IconLocation))
                    appWin.Icon = IconManager.Convert(IconManager.Extract(appWin.WinFileName, 0, true));
                else
                    appWin.Icon = IconManager.Convert(IconManager.Extract(pinnedApp.StringData.IconLocation, pinnedApp.IconIndex, true));
                if (numPinnedApp > MyTaskbarApp.MyShellManager.TasksService.Windows.Count)
                    MyTaskbarApp.MyShellManager.TasksService.Windows.Add(appWin);
                else
                    MyTaskbarApp.MyShellManager.TasksService.Windows.Insert(numPinnedApp++, appWin);
                if (MyTaskbarApp.MyShellManager.TasksService.Windows.Any(win => string.Compare(win.WinFileName, appWin.WinFileName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    foreach (ApplicationWindow win in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => aw != appWin && string.Compare(aw.WinFileName, appWin.WinFileName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        appWin.ListWindows.AddRange(win.ListWindows);
                        win.ListWindows.Clear();
                        appWin.SetTitle();
                        if (appWin.ListWindows.Count > 1)
                            appWin.State = ApplicationWindow.WindowState.Unknown;
                        else
                            appWin.State = win.State;
                        if (!MyTaskbarApp.MyShellManager.TasksService.GroupApplicationsWindows)
                            break;
                    }
                }
            }
        }
    }

    public void FirstRefresh()
    {
        Task.Run(async () =>
        {
            await Task.Delay(10);
            VirtualDesktop_CurrentChanged(null, null);
        });
    }

    public void ForceRefresh()
    {
        OnPropertyChanged(nameof(TaskListCollection));
        TaskListCollection.Refresh();
    }

    #region IDisposable

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                RemoveTaskServiceEvent();
                DisposeAllApplicationWindow();
            }

            disposedValue = true;
        }
    }

    public static void DisposeAllApplicationWindow()
    {
        for (int i = MyTaskbarApp.MyShellManager.TasksService.Windows.Count - 1; i >= 0; i--)
            MyTaskbarApp.MyShellManager.TasksService.Windows[i].Dispose();
        MyTaskbarApp.MyShellManager.TasksService.Windows?.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
