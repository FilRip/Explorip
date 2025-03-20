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

using ManagedShell.AppBar;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

using WindowsDesktop;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskListViewModel : ObservableObject, IDisposable
{
    private AppBarEdge _currentEdge;
    private Taskbar _taskbarParent;
    private readonly object _lockChangeDesktop;
    private bool disposedValue;

    [ObservableProperty()]
    private ICollectionView _taskListCollection;

    public TaskListViewModel() : base()
    {
        _lockChangeDesktop = new object();
        RebuildCollectionView();
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
    }

    public Taskbar TaskbarParent
    {
        get { return _taskbarParent; }
        set
        {
            if (_taskbarParent != value && value != null)
            {
                _taskbarParent = value;
                RemoveTaskServiceEvent();
                if (_taskbarParent.MainScreen)
                {
                    if (VirtualDesktopProvider.Default.Initialized)
                        VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
                    MyTaskbarApp.MyShellManager.TasksService.WindowDestroy += RefreshAllCollectionView;
                    MyTaskbarApp.MyShellManager.TasksService.WindowCreate += RefreshAllCollectionView;
                    MyTaskbarApp.MyShellManager.TasksService.WindowUncloaked += TasksService_WindowUncloaked;
                }
            }
        }
    }

    private static void TasksService_WindowUncloaked(IntPtr windowHandle)
    {
        RefreshAllCollectionView(null, EventArgs.Empty);
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

    private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            lock (_lockChangeDesktop)
            {
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
                };
                appWin.SetTitle(Path.GetFileNameWithoutExtension(file));
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
                    foreach (ApplicationWindow win in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => string.Compare(aw.WinFileName, appWin.WinFileName, StringComparison.OrdinalIgnoreCase) == 0).ToList())
                    {
                        if (win != appWin)
                        {
                            MyTaskbarApp.MyShellManager.TasksService.Windows.Remove(win);
                            appWin.ListWindows.AddRange(win.ListWindows);
                            if (appWin.ListWindows.Count > 1)
                                appWin.State = ApplicationWindow.WindowState.Unknown;
                            else
                                appWin.State = win.State;
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
            await Task.Delay(1000);
            VirtualDesktop_CurrentChanged(null, null);
        });
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
