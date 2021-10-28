using ManagedShell.AppBar;
using Explorip.TaskBar.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using WindowsDesktop;
using ManagedShell.WindowsTasks;
using System.Linq;
using System.Collections.ObjectModel;

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
        private ObservableCollection<ApplicationWindow> _listeFenetresBureauCourant;

        public static DependencyProperty ButtonWidthProperty = DependencyProperty.Register("ButtonWidth", typeof(double), typeof(TaskList), new PropertyMetadata(new double()));

        public TaskList()
        {
            InitializeComponent();
            Console.WriteLine("Abonnement changement VirtualDesktop");
            VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
        }

        public double ButtonWidth
        {
            get { return (double)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
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
                
                isLoaded = true;
            }

            SetStyles();
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            lock (_lockChangeDesktop)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    Console.WriteLine("Change bureau");
                    if (_listeFenetresBureauCourant != null)
                    {
                        _listeFenetresBureauCourant.Clear();
                    }
                    else
                    {
                        _listeFenetresBureauCourant = new ObservableCollection<ApplicationWindow>();
                    }

                    WinAPI.User32.EnumWindows((hwnd, lParam) => {
                        if (VirtualDesktopHelper.IsCurrentVirtualDesktop(hwnd))
                        {
                            ApplicationWindow win = new ApplicationWindow(MyApp.MonShellManager.TasksService, hwnd);

                            if (win.CanAddToTaskbar && win.ShowInTaskbar && !_listeFenetresBureauCourant.Contains(win))
                            {
                                _listeFenetresBureauCourant.Add(win);
                                typeof(TasksService).GetMethod("sendTaskbarButtonCreatedMessage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(MyApp.MonShellManager.TasksService, new object[] { win.Handle });
                            }
                        }
                        return true;
                    }, 0);

                    IntPtr hWndForeground = WinAPI.User32.GetForegroundWindow();
                    if (_listeFenetresBureauCourant.Any(i => i.Handle == hWndForeground && i.ShowInTaskbar))
                    {
                        ApplicationWindow win = _listeFenetresBureauCourant.First(wnd => wnd.Handle == hWndForeground);
                        win.State = ApplicationWindow.WindowState.Active;
                        win.SetShowInTaskbar();
                    }

                    typeof(TasksService).GetProperty("Windows", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(MyApp.MonShellManager.TasksService, _listeFenetresBureauCourant);
                    System.ComponentModel.ICollectionView nouvelleListeGroupedWindows = System.Windows.Data.CollectionViewSource.GetDefaultView(_listeFenetresBureauCourant);
                    typeof(Tasks).GetField("groupedWindows", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(MyApp.MonShellManager.Tasks, nouvelleListeGroupedWindows);
                    TasksList.ItemsSource = MyApp.MonShellManager.Tasks.GroupedWindows;
                }));
            }
        }

        private void TaskList_OnUnloaded(object sender, RoutedEventArgs e)
        {
            MyApp.MonShellManager.Tasks.GroupedWindows.CollectionChanged -= GroupedWindows_CollectionChanged;
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
