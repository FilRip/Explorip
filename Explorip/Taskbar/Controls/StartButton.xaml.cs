using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ManagedShell.Common.Helpers;
using Explorip.TaskBar.Utilities;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for StartButton.xaml
    /// </summary>
    public partial class StartButton : UserControl
    {
        private bool allowOpenStart;
        private readonly DispatcherTimer pendingOpenTimer;

        public static DependencyProperty StartMenuMonitorProperty = DependencyProperty.Register("StartMenuMonitor", typeof(StartMenuMonitor), typeof(StartButton));

        public StartMenuMonitor StartMenuMonitor
        {
            get { return (StartMenuMonitor)GetValue(StartMenuMonitorProperty); }
            set { SetValue(StartMenuMonitorProperty, value); }
        }

        public StartButton()
        {
            InitializeComponent();

            pendingOpenTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = new TimeSpan(0, 0, 0, 1)
            };
            pendingOpenTimer.Tick += (sender, args) =>
            {
                // if the start menu didn't open, flip the button back to unchecked
                Start.IsChecked = false;
                pendingOpenTimer.Stop();
            };
        }

        public void SetStartMenuState(bool opened)
        {
            Dispatcher.Invoke(() =>
            {
                Start.IsChecked = opened;
            });
            pendingOpenTimer.Stop();
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            if (allowOpenStart)
            {
                pendingOpenTimer.Start();
                try
                {
                    ShellHelper.ShowStartMenu();
                }
                catch (Exception) { }
                /*IntPtr pointeurMenuDemarrer = WinAPI.User32.FindWindow("Windows.UI.Core.CoreWindow", "Démarrer");
                if (pointeurMenuDemarrer != IntPtr.Zero)
                {
                    ManagedShell.Interop.NativeMethods.GetWindowRect(pointeurMenuDemarrer, out ManagedShell.Interop.NativeMethods.Rect rect);
                    System.Windows.Forms.Screen ecran = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
                    WinAPI.User32.SetWindowPos(pointeurMenuDemarrer, ecran.Bounds.X, ecran.Bounds.Height - rect.Height, 0, 0, WinAPI.User32.SWP.NOSIZE | WinAPI.User32.SWP.SHOWWINDOW);
                }*/
                return;
            }

            Start.IsChecked = false;
        }

        private void Start_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            allowOpenStart = Start.IsChecked == false;
        }

        private void Start_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EnvironmentHelper.IsWindows10OrBetter)
            {
                ShellHelper.ShowStartContextMenu();
                e.Handled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartMenuMonitor.StartMenuVisibilityChanged += AppVisibilityHelper_StartMenuVisibilityChanged;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StartMenuMonitor.StartMenuVisibilityChanged -= AppVisibilityHelper_StartMenuVisibilityChanged;
        }

        private void AppVisibilityHelper_StartMenuVisibilityChanged(object sender, ManagedShell.Common.SupportingClasses.LauncherVisibilityEventArgs e)
        {
            SetStartMenuState(e.Visible);
        }
    }
}
