using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.Utilities;

using ManagedShell.AppBar;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

using WindowsDesktop;

namespace Explorip.TaskBar.Controls
{
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

            if (!isLoaded && MyDesktopApp.MonShellManager.Tasks != null)
            {
                InsertPinnedApp();

                TasksList.ItemsSource = MyDesktopApp.MonShellManager.Tasks.GroupedWindows;
                if (MyDesktopApp.MonShellManager.Tasks.GroupedWindows != null)
                    MyDesktopApp.MonShellManager.Tasks.GroupedWindows.CollectionChanged += GroupedWindows_CollectionChanged;

                Console.WriteLine("Abonnement changement VirtualDesktop");
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
                    Console.WriteLine("Change bureau");
                    if (MyDesktopApp.MonShellManager.TasksService.Windows != null)
                    {
                        MyDesktopApp.MonShellManager.TasksService.Windows.Clear();
                    }
                    else
                    {
                        MyDesktopApp.MonShellManager.TasksService.Windows = [];
                    }

                    WinAPI.User32.EnumWindows((hwnd, lParam) =>
                    {
                        if (VirtualDesktopHelper.IsCurrentVirtualDesktop(hwnd))
                        {
                            ApplicationWindow win = new(MyDesktopApp.MonShellManager.TasksService, hwnd);

                            if (win.CanAddToTaskbar && win.ShowInTaskbar && !MyDesktopApp.MonShellManager.TasksService.Windows.Contains(win))
                            {
                                MyDesktopApp.MonShellManager.TasksService.Windows.Add(win);
                                MyDesktopApp.MonShellManager.TasksService.SendTaskbarButtonCreatedMessage(win.Handle);
                            }
                        }
                        return true;
                    }, 0);

                    InsertPinnedApp();

                    IntPtr hWndForeground = WinAPI.User32.GetForegroundWindow();
                    if (MyDesktopApp.MonShellManager.TasksService.Windows.Any(i => (i.Handle == hWndForeground || i.ListWindows.Contains(hWndForeground)) && i.ShowInTaskbar))
                    {
                        ApplicationWindow win = MyDesktopApp.MonShellManager.TasksService.Windows.First(wnd => (wnd.Handle == hWndForeground || wnd.ListWindows.Contains(hWndForeground)));
                        win.State = ApplicationWindow.WindowState.Active;
                        win.SetShowInTaskbar();
                    }

                    System.ComponentModel.ICollectionView nouvelleListeGroupedWindows = System.Windows.Data.CollectionViewSource.GetDefaultView(MyDesktopApp.MonShellManager.TasksService.Windows);
                    MyDesktopApp.MonShellManager.Tasks.GroupedWindows = nouvelleListeGroupedWindows;
                    TasksList.ItemsSource = MyDesktopApp.MonShellManager.Tasks.GroupedWindows;
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
                foreach (string file in Directory.GetFiles(path, "*.lnk").Where(file => !MyDesktopApp.MonShellManager.TasksService.Windows.Any(win => win.Title == Path.GetFileNameWithoutExtension(file))))
                {
                    pinnedApp = Shortcut.ReadFromFile(file);
                    appWin = new ApplicationWindow(MyDesktopApp.MonShellManager.TasksService, IntPtr.Zero);
                    appWin.SetTitle(Path.GetFileNameWithoutExtension(file));
                    appWin.IsPinnedApp = true;
                    if (pinnedApp.LinkTargetIDList?.Path != null)
                        appWin.WinFileName = Path.GetFullPath(pinnedApp.LinkTargetIDList.Path);
                    else if (pinnedApp.ExtraData?.EnvironmentVariableDataBlock?.TargetUnicode != null)
                        appWin.WinFileName = Path.GetFullPath(pinnedApp.ExtraData?.EnvironmentVariableDataBlock?.TargetUnicode);
                    else
                    {
                        Console.WriteLine($"Unable to add {file} as pinned app");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(pinnedApp.StringData.IconLocation))
                        appWin.Icon = IconManager.Convert(IconManager.Extract(appWin.WinFileName, 0, true));
                    else
                        appWin.Icon = IconManager.Convert(IconManager.Extract(pinnedApp.StringData.IconLocation, pinnedApp.IconIndex, true));
                    appWin.Arguments = pinnedApp.StringData.CommandLineArguments;
                    MyDesktopApp.MonShellManager.TasksService.Windows.Insert(numPinnedApp++, appWin);
                    if (MyDesktopApp.MonShellManager.TasksService.Windows.Any(win => win.WinFileName == appWin.WinFileName))
                    {
                        foreach (ApplicationWindow win in MyDesktopApp.MonShellManager.TasksService.Windows.Where(aw => aw.WinFileName == appWin.WinFileName).ToList())
                        {
                            if (win != appWin)
                            {
                                MyDesktopApp.MonShellManager.TasksService.Windows.Remove(win);
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
            MyDesktopApp.MonShellManager.Tasks.GroupedWindows.CollectionChanged -= GroupedWindows_CollectionChanged;
            Console.WriteLine("Désabonnement changement VirtualDesktop");
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
}
