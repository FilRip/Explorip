using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;
using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

using WindowsDesktop;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskListViewModel : ObservableObject, IDisposable
{
    private const int ScrollBarWidth = 22;

    private AppBarEdge _currentEdge;
    private Taskbar _taskbarParent;
    private readonly object _lockChangeDesktop;
    private bool disposedValue;
    private HorizontalAlignment _tasklistAligment;

    [ObservableProperty()]
    private ICollectionView _taskListCollection;
    [ObservableProperty()]
    private GridLength _buttonWidth, _buttonHeight;
    [ObservableProperty()]
    private GridLength _buttonRightMargin, _buttonBottomMargin;
    [ObservableProperty()]
    private double _titleLength;

    public TaskListViewModel() : base()
    {
        _lockChangeDesktop = new object();
        RebuildCollectionView();
    }

    public static void RebuildListWindows()
    {
        ((MyTaskbarApp)Application.Current).MainTaskbar.MyTaskList.MyDataContext.FirstRefresh();
    }

    public static void RefreshAllCollectionView(object sender, EventArgs e)
    {
        bool rebuild = false;
        if (sender is bool b)
            rebuild = b;
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (TaskListViewModel tl in ((MyTaskbarApp)Application.Current).ListAllTaskbar().Select(t => t.MyTaskList.MyDataContext))
            {
                if (rebuild)
                    tl.RebuildCollectionView();
                tl.RefreshMyCollectionView();
                if (ConfigManager.ReduceTitleWidthWhenTaskbarFull)
                    tl.UpdateMaxWidth();
            }
        });
    }

    public void RefreshMyCollectionView()
    {
        TaskListCollection.Refresh();
    }

    public void RebuildCollectionView()
    {
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
            if (_taskbarParent != value && value != null)
            {
                _taskbarParent = value;
                _tasklistAligment = ConfigManager.GetTaskbarConfig(value.ScreenName).TaskListAligment;
                OnPropertyChanged(nameof(TasklistHorizontalAligment));
                if (_taskbarParent.MainScreen)
                {
                    RemoveTaskServiceEvent();
                    if (VirtualDesktopProvider.Default.Initialized)
                        VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
                    MyTaskbarApp.MyShellManager.TasksService.WindowDestroy += RefreshAllCollectionView;
                    MyTaskbarApp.MyShellManager.TasksService.WindowCreate += RefreshAllCollectionView;
                    MyTaskbarApp.MyShellManager.TasksService.WindowUncloaked += TasksService_WindowUncloaked;
                    ChangeButtonSize();
                }
            }
        }
    }

    public void ChangeButtonSize()
    {
        ButtonWidth = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).TaskButtonSize + 20, GridUnitType.Pixel);
        ButtonHeight = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).TaskButtonSize + 13, GridUnitType.Pixel);
        TitleLength = ButtonWidth.Value;
        if (ConfigManager.ShowTitleApplicationWindow)
        {
            ButtonWidth = GridLength.Auto;
            TitleLength += ConfigManager.MaxWidthTitleApplicationWindow;
        }

        ButtonRightMargin = new GridLength(0, GridUnitType.Pixel);
        ButtonBottomMargin = new GridLength(0, GridUnitType.Pixel);
        if (_currentEdge == AppBarEdge.Left || _currentEdge == AppBarEdge.Right)
            ButtonBottomMargin = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).SpaceBetweenTaskButton, GridUnitType.Pixel);
        else
            ButtonRightMargin = new GridLength(ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).SpaceBetweenTaskButton, GridUnitType.Pixel);
    }

    private static void TasksService_WindowUncloaked(IntPtr windowHandle)
    {
        RefreshAllCollectionView(null, EventArgs.Empty);
    }

    public void UpdateMaxWidth()
    {
        if (!ConfigManager.ShowTitleApplicationWindow)
            return;
        double currentWidth = TaskbarParent.MyTaskList.TasksList.Items.Count * (ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).TaskButtonSize + 20 + ConfigManager.MaxWidthTitleApplicationWindow + ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).SpaceBetweenTaskButton);
        double minWidth = ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).TaskButtonSize + 20;
        if (currentWidth > TaskbarParent.MyTaskList.ActualWidth)
        {
            double newMaxWidth = (TaskbarParent.MyTaskList.ActualWidth - ScrollBarWidth - ButtonRightMargin.Value * TaskbarParent.MyTaskList.TasksList.Items.Count) / TaskbarParent.MyTaskList.TasksList.Items.Count;
            TitleLength = Math.Max(minWidth, newMaxWidth);
        }
        else
            TitleLength = ConfigManager.GetTaskbarConfig(TaskbarParent.ScreenName).TaskButtonSize + 20 + ConfigManager.MaxWidthTitleApplicationWindow;
    }

    private static void RemoveTaskServiceEvent()
    {
        MyTaskbarApp.MyShellManager.TasksService.WindowDestroy -= RefreshAllCollectionView;
        MyTaskbarApp.MyShellManager.TasksService.WindowCreate -= RefreshAllCollectionView;
        MyTaskbarApp.MyShellManager.TasksService.WindowUncloaked -= TasksService_WindowUncloaked;
    }

    private static bool FilterAppWindow(object item)
    {
        if (item is ApplicationWindow window && window.ShowInTaskbar && !window.IsDisposed)
            return true;

        return false;
    }

    public Orientation PanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }

    public HorizontalAlignment TasklistHorizontalAligment
    {
        get { return _tasklistAligment; }
    }

    private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            lock (_lockChangeDesktop)
            {
                for (int i = MyTaskbarApp.MyShellManager.TasksService.Windows.Count - 1; i >= 0; i--)
                    MyTaskbarApp.MyShellManager.TasksService.Windows[i].Dispose();
                MyTaskbarApp.MyShellManager.TasksService.Windows?.Clear();
                MyTaskbarApp.MyShellManager.TasksService.Windows = [];

                NativeMethods.EnumWindows((hwnd, lParam) =>
                {
                    if (!VirtualDesktopProvider.Default.Initialized || VirtualDesktopHelper.IsCurrentVirtualDesktop(hwnd))
                    {
                        ApplicationWindow win = new(MyTaskbarApp.MyShellManager.TasksService, hwnd);

                        if (win.CanAddToTaskbar && win.ShowInTaskbar && !MyTaskbarApp.MyShellManager.TasksService.Windows.Contains(win))
                        {
                            MyTaskbarApp.MyShellManager.TasksService.Windows.Add(win);
                            if (ManagedShell.Common.Helpers.EnvironmentHelper.IsAppRunningAsShell)
                                TasksService.SendTaskbarButtonCreatedMessage(hwnd);
                        }
                    }
                    return true;
                }, 0);

                InsertPinnedApp();

                int lastPosition = MyTaskbarApp.MyShellManager.TasksService.Windows.Where(w => w.IsPinnedApp).Max(w => w.Position);
                foreach (ApplicationWindow appWin in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(w => !w.IsPinnedApp))
                    appWin.Position = ++lastPosition;

                IntPtr hWndForeground = NativeMethods.GetForegroundWindow();
                if (hWndForeground != IntPtr.Zero)
                {
                    ApplicationWindow win = MyTaskbarApp.MyShellManager.TasksService.Windows.FirstOrDefault(wnd => wnd.ListWindows.Contains(hWndForeground));
                    if (win != null && win.ShowInTaskbar)
                    {
                        win.State = ApplicationWindow.WindowState.Active;
                        win.SetShowInTaskbar();
                    }
                }

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

                IEnumerable<ApplicationWindow> windowsToDispose = [.. MyTaskbarApp.MyShellManager.TasksService.Windows.Where(win => !win.IsPinnedApp && win.ListWindows.Count == 0)];
                foreach (ApplicationWindow win in windowsToDispose)
                    win.Dispose();

                RefreshAllCollectionView(true, EventArgs.Empty);
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
                    WinFileName = pinnedApp.Target,
                    Arguments = pinnedApp.StringData?.CommandLineArguments,
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
                appWin.Arguments = pinnedApp.StringData?.CommandLineArguments;
                if (numPinnedApp > MyTaskbarApp.MyShellManager.TasksService.Windows.Count)
                    MyTaskbarApp.MyShellManager.TasksService.Windows.Add(appWin);
                else
                    MyTaskbarApp.MyShellManager.TasksService.Windows.Insert(numPinnedApp++, appWin);
                if (MyTaskbarApp.MyShellManager.TasksService.Windows.Any(win => string.Compare(win.WinFileName, appWin.WinFileName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    foreach (ApplicationWindow win in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => string.Compare(aw.WinFileName, appWin.WinFileName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        if (win != appWin)
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
    }

    public void FirstRefresh()
    {
        Task.Run(async () =>
        {
            await Task.Delay(100);
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
                if (VirtualDesktopProvider.Default.Initialized)
                    VirtualDesktop.CurrentChanged -= VirtualDesktop_CurrentChanged;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
