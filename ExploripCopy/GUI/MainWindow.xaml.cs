using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

using ExploripCopy.ViewModels;

using ManagedShell.Interop;

namespace ExploripCopy.GUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon _iconInSystray;

        public MainWindow()
        {
            InitializeComponent();
            _iconInSystray = new NotifyIcon
            {
                Icon = Properties.Resources.icone,
            };
            _iconInSystray.DoubleClick += IconInSystray_DoubleClick;
        }

        private MainViewModels MyDataContext
        {
            get { return (MainViewModels)DataContext; }
        }

        private void IconInSystray_DoubleClick(object sender, System.EventArgs e)
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            MyDataContext.WindowMaximized = true;
        }

        private void RestoreWindow_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
