using ManagedShell.AppBar;
using Explorip.TaskBar.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using WindowsDesktop;
using ManagedShell.WindowsTasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
        private readonly object _lockChangeDesktop = new object();
        private bool _mainScreen;

        public static DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(TaskList), new PropertyMetadata(new double()));
        public static DependencyProperty EcranPrincipalProperty = DependencyProperty.Register(nameof(EcranPrincipal), typeof(bool), typeof(TaskList), new PropertyMetadata(new bool()));

        public TaskList()
        {
            InitializeComponent();
        }

        public double ButtonWidth
        {
            get { return (double)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }

        public bool EcranPrincipal
        {
            get { return _mainScreen; }
            set { _mainScreen = value; }
        }

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
            if (!isLoaded && MyApp.MonShellManager.Tasks != null)
            {
                TasksList.ItemsSource = MyApp.MonShellManager.Tasks.GroupedWindows;
                if (MyApp.MonShellManager.Tasks.GroupedWindows != null)
                    MyApp.MonShellManager.Tasks.GroupedWindows.CollectionChanged += GroupedWindows_CollectionChanged;

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
                    if (MyApp.MonShellManager.TasksService.Windows != null)
                    {
                        MyApp.MonShellManager.TasksService.Windows.Clear();
                    }
                    else
                    {
                        MyApp.MonShellManager.TasksService.Windows = new ObservableCollection<ApplicationWindow>();
                    }

                    WinAPI.User32.EnumWindows((hwnd, lParam) =>
                    {
                        if (VirtualDesktopHelper.IsCurrentVirtualDesktop(hwnd))
                        {
                            ApplicationWindow win = new ApplicationWindow(MyApp.MonShellManager.TasksService, hwnd);

                            if (win.CanAddToTaskbar && win.ShowInTaskbar && !MyApp.MonShellManager.TasksService.Windows.Contains(win))
                            {
                                MyApp.MonShellManager.TasksService.Windows.Add(win);
                                MyApp.MonShellManager.TasksService.SendTaskbarButtonCreatedMessage(win.Handle);
                            }
                        }
                        return true;
                    }, 0);

                    IntPtr hWndForeground = WinAPI.User32.GetForegroundWindow();
                    if (MyApp.MonShellManager.TasksService.Windows.Any(i => i.Handle == hWndForeground && i.ShowInTaskbar))
                    {
                        ApplicationWindow win = MyApp.MonShellManager.TasksService.Windows.First(wnd => wnd.Handle == hWndForeground);
                        win.State = ApplicationWindow.WindowState.Active;
                        win.SetShowInTaskbar();
                    }

                    System.ComponentModel.ICollectionView nouvelleListeGroupedWindows = System.Windows.Data.CollectionViewSource.GetDefaultView(MyApp.MonShellManager.TasksService.Windows);
                    MyApp.MonShellManager.Tasks.groupedWindows = nouvelleListeGroupedWindows;
                    TasksList.ItemsSource = MyApp.MonShellManager.Tasks.GroupedWindows;
                }));
            }
        }

        private void TaskList_OnUnloaded(object sender, RoutedEventArgs e)
        {
            MyApp.MonShellManager.Tasks.GroupedWindows.CollectionChanged -= GroupedWindows_CollectionChanged;
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
