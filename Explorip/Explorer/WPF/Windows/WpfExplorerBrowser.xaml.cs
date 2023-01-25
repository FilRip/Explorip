using System.Windows;
using System.Windows.Interop;

using Explorip.Helpers;
using Explorip.WinAPI;

using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Media.Imaging;
using Explorip.Explorer.WPF.ViewModels;
using System;
using System.Windows.Controls;

namespace Explorip.Explorer.WPF.Windows
{
    /// <summary>
    /// Logique d'interaction pour WpfExplorerBrowser.xaml
    /// </summary>
    public partial class WpfExplorerBrowser : Window
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
                WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).Handle, true);
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
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.GetPosition(this).Y <= 32)
                {
                    if (e.ClickCount == 2 && WindowState == WindowState.Normal)
                    {
                        MaximizeWindow_Click(this, new RoutedEventArgs());
                    }
                    else
                    {
                        SetWindowNormal();
                        DragMove();
                    }
                }
            }
            catch (Exception) { /* No need to catch error */ }
        }

        public void HideRightTab()
        {
            RightGrid.Width = new GridLength(0);
            LeftTab.SetValue(Grid.ColumnSpanProperty, 6);
        }

        public void ShowRightTab()
        {
            RightGrid.Width = new GridLength(1, GridUnitType.Star);
            RightTab.Visibility = Visibility.Visible;
            LeftTab.SetValue(Grid.ColumnSpanProperty, 1);
        }
    }
}
