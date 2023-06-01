using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using ExploripCopy.Helpers;
using ExploripCopy.ViewModels;

using ManagedShell.Interop;

namespace ExploripCopy.GUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _forceClose;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Constants.Localization.LoadTranslation();

            Instance = this;
            DataContext = MainViewModels.Instance;
            _forceClose = false;

            IpcServer.CreateIpcServer();
        }

        public void IconInSystray_Exit()
        {
            MainViewModels.Instance.Dispose();
            _forceClose = true;
            Close();
        }

        private MainViewModels MyDataContext
        {
            get { return (MainViewModels)DataContext; }
        }

        #region Window manager

        public void IconInSystray_DoubleClick()
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Visibility = Visibility.Hidden;
            e.Cancel = !_forceClose;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            MyDataContext.WindowMaximized = true;
        }

        private void RestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            MyDataContext.WindowMaximized = false;
        }

        private void TitleBar_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = NativeMethods.GetSystemMenu(hWnd, false);
            Point posMouse = PointToScreen(Mouse.GetPosition(this));
            int cmd = NativeMethods.TrackPopupMenu(hMenu, 0x100, (int)posMouse.X, (int)posMouse.Y, 0, hWnd, IntPtr.Zero);
            if (cmd > 0)
                NativeMethods.SendMessage(hWnd, 0x112, (uint)cmd, 0);
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Drag Window

        private bool _startDrag;
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_startDrag && WindowState != WindowState.Minimized && IsVisible && IsActive)
            {
                _startDrag = false;
                if (WindowState == WindowState.Maximized)
                    Top = Mouse.GetPosition(System.Windows.Application.Current.MainWindow).Y;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    RestoreWindow_Click(sender, null);
                    DragMove();
                }
            }
        }

        private void TitleBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _startDrag = false;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startDrag = true;
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            IpcServer.ShutdownIpcServer();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DgListWaiting.Items.Refresh();
        }
    }
}
