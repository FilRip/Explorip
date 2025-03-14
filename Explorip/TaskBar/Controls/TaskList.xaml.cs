using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

using WindowsDesktop;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskList.xaml
/// </summary>
public partial class TaskList : UserControl
{
    private bool isLoaded;
    private double DefaultButtonWidth;
    private double TaskButtonLeftMargin;
    private double TaskButtonRightMargin;
    private readonly object _lockChangeDesktop;

    public readonly static DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(TaskList), new PropertyMetadata(new double()));
    public readonly static DependencyProperty TaskbarParentProperty = DependencyProperty.Register(nameof(TaskbarParent), typeof(Taskbar), typeof(TaskList));

    public TaskList()
    {
        InitializeComponent();
        _lockChangeDesktop = new object();
    }

    public TaskListViewModel MyDataContext
    {
        get { return (TaskListViewModel)DataContext; }
    }

    public double ButtonWidth
    {
        get { return (double)GetValue(ButtonWidthProperty); }
        set { SetValue(ButtonWidthProperty, value); }
    }

    public Taskbar TaskbarParent { get; set; }

    private void SetStyles()
    {
        DefaultButtonWidth = Application.Current.FindResource("TaskButtonWidth") as double? ?? 0;
        Thickness buttonMargin;

        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Left || ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Right)
        {
            buttonMargin = Application.Current.FindResource("TaskButtonVerticalMargin") as Thickness? ?? new Thickness();
        }
        else
        {
            buttonMargin = Application.Current.FindResource("TaskButtonMargin") as Thickness? ?? new Thickness();
        }

        TaskButtonLeftMargin = buttonMargin.Left;
        TaskButtonRightMargin = buttonMargin.Right;
    }

    private void TaskList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if (!isLoaded && MyTaskbarApp.MyShellManager.Tasks != null)
        {
            MyDataContext.ChangeEdge(this.FindControlParent<Taskbar>().AppBarEdge);

            MyTaskbarApp.MyShellManager.Tasks.GroupedWindows.CollectionChanged += GroupedWindows_CollectionChanged;
            MyTaskbarApp.MyShellManager.TasksService.RemoveAppWindow += TasksService_RemoveAppWindow;

            if (this.FindControlParent<Taskbar>().MainScreen)
            {
                if (VirtualDesktopProvider.Default.Initialized)
                    VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Refresh(true);
                });
            }

            isLoaded = true;
        }

        SetStyles();
    }

    private void TasksService_RemoveAppWindow(object sender, EventArgs e)
    {
        MyTaskbarApp.MyShellManager.Tasks.GroupedWindows.Refresh();
    }

    private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
    {
        lock (_lockChangeDesktop)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
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

                System.ComponentModel.ICollectionView newGroupedListAppWindow = System.Windows.Data.CollectionViewSource.GetDefaultView(MyTaskbarApp.MyShellManager.TasksService.Windows);
                newGroupedListAppWindow.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(ApplicationWindow.Position), System.ComponentModel.ListSortDirection.Ascending));
                newGroupedListAppWindow.Filter = FilterAppWindow;
                MyTaskbarApp.MyShellManager.Tasks.GroupedWindows = newGroupedListAppWindow;
                foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
                {
                    tb.MyTaskList.Refresh();
                }
            }));
        }
    }

    private static bool FilterAppWindow(object item)
    {
        if (item is ApplicationWindow window && window.ShowInTaskbar && !window.IsDisposed)
            return true;

        return false;
    }

    public void Refresh(bool forceRebuild = false)
    {
        if (forceRebuild)
        {
            VirtualDesktop_CurrentChanged(null, null);
        }
        TasksList.ItemsSource = MyTaskbarApp.MyShellManager.Tasks.GroupedWindows;
    }

    private static void InsertPinnedApp()
    {
        string path = Path.Combine(Environment.SpecialFolder.ApplicationData.FullPath(), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
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
                    Console.WriteLine($"Unable to add {file} as pinned app");
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
                if (MyTaskbarApp.MyShellManager.TasksService.Windows.Any(win => win.WinFileName == appWin.WinFileName))
                {
                    List<ApplicationWindow> toDispose = [];
                    foreach (ApplicationWindow win in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => aw.WinFileName == appWin.WinFileName).ToList())
                    {
                        if (win != appWin)
                        {
                            toDispose.Add(win);
                            MyTaskbarApp.MyShellManager.TasksService.Windows.Remove(win);
                            appWin.ListWindows.AddRange(win.ListWindows);
                            if (appWin.ListWindows.Count > 1)
                                appWin.State = ApplicationWindow.WindowState.Unknown;
                            else
                                appWin.State = win.State;
                        }
                    }
                    foreach (ApplicationWindow win in toDispose)
                        win.Dispose();
                }
            }
        }
    }

    private void TaskList_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        MyTaskbarApp.MyShellManager.Tasks.GroupedWindows.CollectionChanged -= GroupedWindows_CollectionChanged;
        MyTaskbarApp.MyShellManager.TasksService.RemoveAppWindow -= TasksService_RemoveAppWindow;
        if (VirtualDesktopProvider.Default.Initialized)
            VirtualDesktop.CurrentChanged -= VirtualDesktop_CurrentChanged;
        isLoaded = false;
    }

    private void GroupedWindows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        SetTaskButtonWidth();
    }

    private void TaskList_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetTaskButtonWidth();
    }

    private void SetTaskButtonWidth()
    {
        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Left || ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Right)
        {
            ButtonWidth = ActualWidth;
            return;
        }

        double margin = TaskButtonLeftMargin + TaskButtonRightMargin;
        double maxWidth = TasksList.ActualWidth / TasksList.Items.Count;
        double defaultWidth = DefaultButtonWidth + margin;

        if (maxWidth > defaultWidth)
        {
            ButtonWidth = DefaultButtonWidth;
        }
        else
        {
            ButtonWidth = Math.Floor(maxWidth);
        }
    }
}
