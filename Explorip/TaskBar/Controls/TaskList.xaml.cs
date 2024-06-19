using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.Utilities;

using ExploripConfig.Helpers;

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
    public readonly static DependencyProperty MainScreenProperty = DependencyProperty.Register(nameof(MainScreen), typeof(bool), typeof(TaskList), new PropertyMetadata(new bool()));
    public readonly static DependencyProperty TaskbarParentProperty = DependencyProperty.Register(nameof(TaskbarParent), typeof(bool), typeof(TaskList), new PropertyMetadata(new bool()));

    public TaskList()
    {
        InitializeComponent();
        _lockChangeDesktop = new object();
    }

    public double ButtonWidth
    {
        get { return (double)GetValue(ButtonWidthProperty); }
        set { SetValue(ButtonWidthProperty, value); }
    }

    public bool MainScreen { get; set; }

    public Taskbar TaskbarParent { get; set; }

    private void SetStyles()
    {
        DefaultButtonWidth = Application.Current.FindResource("TaskButtonWidth") as double? ?? 0;
        Thickness buttonMargin;

        if (Settings.Instance.Edge == (int)AppBarEdge.Left || Settings.Instance.Edge == (int)AppBarEdge.Right)
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
            InsertPinnedApp();

            TasksList.ItemsSource = MyTaskbarApp.MyShellManager.Tasks.GroupedWindows;
            if (MyTaskbarApp.MyShellManager.Tasks.GroupedWindows != null)
                MyTaskbarApp.MyShellManager.Tasks.GroupedWindows.CollectionChanged += GroupedWindows_CollectionChanged;

            if (VirtualDesktopProvider.Default.Initialized)
                VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;

            isLoaded = true;
        }

        SetStyles();
    }

    private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
    {
        lock (_lockChangeDesktop)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MyTaskbarApp.MyShellManager.TasksService.Windows != null)
                {
                    MyTaskbarApp.MyShellManager.TasksService.Windows.Clear();
                }
                else
                {
                    MyTaskbarApp.MyShellManager.TasksService.Windows = [];
                }

                NativeMethods.EnumWindows((hwnd, lParam) =>
                {
                    if (!VirtualDesktopProvider.Default.Initialized || VirtualDesktopHelper.IsCurrentVirtualDesktop(hwnd))
                    {
                        ApplicationWindow win = new(MyTaskbarApp.MyShellManager.TasksService, hwnd);

                        if (win.CanAddToTaskbar && win.ShowInTaskbar && !MyTaskbarApp.MyShellManager.TasksService.Windows.Contains(win))
                        {
                            MyTaskbarApp.MyShellManager.TasksService.Windows.Add(win);
                            MyTaskbarApp.MyShellManager.TasksService.SendTaskbarButtonCreatedMessage(win.Handle);
                        }
                    }
                    return true;
                }, 0);

                InsertPinnedApp();

                IntPtr hWndForeground = NativeMethods.GetForegroundWindow();
                if (MyTaskbarApp.MyShellManager.TasksService.Windows.Any(i => (i.Handle == hWndForeground || i.ListWindows.Contains(hWndForeground)) && i.ShowInTaskbar))
                {
                    ApplicationWindow win = MyTaskbarApp.MyShellManager.TasksService.Windows.First(wnd => (wnd.Handle == hWndForeground || wnd.ListWindows.Contains(hWndForeground)));
                    win.State = ApplicationWindow.WindowState.Active;
                    win.SetShowInTaskbar();
                }

                System.ComponentModel.ICollectionView nouvelleListeGroupedWindows = System.Windows.Data.CollectionViewSource.GetDefaultView(MyTaskbarApp.MyShellManager.TasksService.Windows);
                MyTaskbarApp.MyShellManager.Tasks.GroupedWindows = nouvelleListeGroupedWindows;
                TasksList.ItemsSource = MyTaskbarApp.MyShellManager.Tasks.GroupedWindows;
            }));
        }
    }

    private void InsertPinnedApp()
    {
        string path = Path.Combine(Environment.SpecialFolder.ApplicationData.FullPath(), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
        if (Directory.Exists(path))
        {
            int numPinnedApp = 0;
            Shortcut pinnedApp;
            ApplicationWindow appWin;
            foreach (string file in Directory.GetFiles(path, "*.lnk").Where(file => !MyTaskbarApp.MyShellManager.TasksService.Windows.Any(win => win.Title == Path.GetFileNameWithoutExtension(file))))
            {
                pinnedApp = Shortcut.ReadFromFile(file);
                appWin = new ApplicationWindow(MyTaskbarApp.MyShellManager.TasksService, IntPtr.Zero);
                appWin.SetTitle(Path.GetFileNameWithoutExtension(file));
                appWin.IsPinnedApp = true;
                appWin.WinFileName = pinnedApp.Target;
                if (string.IsNullOrWhiteSpace(appWin.WinFileName))
                {
                    Console.WriteLine($"Unable to add {file} as pinned app");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(pinnedApp.StringData.IconLocation))
                    appWin.Icon = IconManager.Convert(IconManager.Extract(appWin.WinFileName, 0, true));
                else
                    appWin.Icon = IconManager.Convert(IconManager.Extract(pinnedApp.StringData.IconLocation, pinnedApp.IconIndex, true));
                appWin.Arguments = pinnedApp.StringData.CommandLineArguments;
                MyTaskbarApp.MyShellManager.TasksService.Windows.Insert(numPinnedApp++, appWin);
                if (MyTaskbarApp.MyShellManager.TasksService.Windows.Any(win => win.WinFileName == appWin.WinFileName))
                {
                    foreach (ApplicationWindow win in MyTaskbarApp.MyShellManager.TasksService.Windows.Where(aw => aw.WinFileName == appWin.WinFileName).ToList())
                    {
                        if (win != appWin)
                        {
                            MyTaskbarApp.MyShellManager.TasksService.Windows.Remove(win);
                            appWin.ListWindows.Add(win.Handle);
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

    private void TaskList_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        MyTaskbarApp.MyShellManager.Tasks.GroupedWindows.CollectionChanged -= GroupedWindows_CollectionChanged;
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
        if (Settings.Instance.Edge == (int)AppBarEdge.Left || Settings.Instance.Edge == (int)AppBarEdge.Right)
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
