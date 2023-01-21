using System.Windows;
using System.Windows.Interop;

using Explorip.Helpers;
using Explorip.WinAPI;

using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Media.Imaging;
using Explorip.Explorer.WPF.ViewModels;
using System;

namespace Explorip.Explorer.WPF.Window
{
    /// <summary>
    /// Logique d'interaction pour WpfExplorerBrowser.xaml
    /// </summary>
    public partial class WpfExplorerBrowser : System.Windows.Window
    {
        public WpfExplorerBrowser()
        {
            InitializeComponent();
            LeftTab.FirstTab.ExplorerBrowser.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
            RightTab.FirstTab.ExplorerBrowser.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);

            Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.IconeExplorateur.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // TODO : https://stackoverflow.com/questions/59366391/is-there-any-way-to-make-a-wpf-app-respect-the-system-choice-of-dark-light-theme
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                //WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).Handle, true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }
        }

        private WpfExplorerBrowserViewModel MyDataContext
        {
            get { return (WpfExplorerBrowserViewModel)DataContext; }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            SetWindowNormal();
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            MyDataContext.WindowMaximized = true;
            BorderThickness = new Thickness(6);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            SetWindowNormal();
        }

        private void SetWindowNormal()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MyDataContext.WindowMaximized = false;
                BorderThickness = new Thickness(0);
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.GetPosition(this).Y <= 16)
                {
                    SetWindowNormal();
                    DragMove();
                }
            }
            catch (Exception) { /* No need to catch error */ }
        }
    }
}
