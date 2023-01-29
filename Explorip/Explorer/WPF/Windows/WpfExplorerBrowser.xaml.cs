﻿using System.Windows;
using System.Windows.Interop;

using Explorip.Helpers;
using Explorip.WinAPI;

using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Media.Imaging;
using Explorip.Explorer.WPF.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Explorip.Explorer.WPF.Controls;
using System.Linq;
using System.IO;
using System.Windows.Threading;

namespace Explorip.Explorer.WPF.Windows
{
    /// <summary>
    /// Logique d'interaction pour WpfExplorerBrowser.xaml
    /// </summary>
    public partial class WpfExplorerBrowser : Window
    {
        private readonly FilesOperations.FileOperation _fileOperation;

        public WpfExplorerBrowser()
        {
            InitializeComponent();

            _fileOperation = new FilesOperations.FileOperation();

            LeftTab.FirstTab.ExplorerBrowser.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
            RightTab.FirstTab.ExplorerBrowser.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);

            Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.IconeExplorateur.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // TODO : https://stackoverflow.com/questions/59366391/is-there-any-way-to-make-a-wpf-app-respect-the-system-choice-of-dark-light-theme
            //        https://stackoverflow.com/questions/69097246/how-to-support-for-windows-11-snap-layout-to-the-custom-maximize-restore-butto
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).Handle, true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }
        }

        public WpfExplorerBrowserViewModel MyDataContext
        {
            get { return (WpfExplorerBrowserViewModel)DataContext; }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.WindowState = WindowState.Minimized;
            }));
        }

        private void RestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            SetWindowNormal();
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.WindowState = WindowState.Maximized;
                this.Activate();
            }));
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
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.WindowState = WindowState.Normal;
                    this.Activate();
                }));
                MyDataContext.WindowMaximized = false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Minimized)
                    return;

                if (e.ChangedButton == MouseButton.Left && e.GetPosition(this).Y <= 32)
                {
                    if (e.ClickCount == 2 && WindowState == WindowState.Normal)
                    {
                        MaximizeWindow_Click(this, new RoutedEventArgs());
                    }
                    else
                    {
                        HitTestResult result = VisualTreeHelper.HitTest((WpfExplorerBrowser)sender, e.GetPosition((WpfExplorerBrowser)sender));
                        if (result.VisualHit is System.Windows.Controls.Primitives.TabPanel)
                        {
                            SetWindowNormal();
                            DragMove();
                        }
                    }
                }
                else if (e.ChangedButton == MouseButton.Right && e.GetPosition(this).Y <= 32)
                {
                    HitTestResult result = VisualTreeHelper.HitTest((WpfExplorerBrowser)sender, e.GetPosition((WpfExplorerBrowser)sender));
                    if (result.VisualHit is System.Windows.Controls.Primitives.TabPanel)
                    {
                        IntPtr hWnd = new WindowInteropHelper(this).Handle;
                        User32.GetWindowRect(hWnd, out WinAPI.Modeles.RECT pos);
                        IntPtr hMenu = User32.GetSystemMenu(hWnd, false);
                        Point posMouse = PointToScreen(Mouse.GetPosition(this));
                        int cmd = User32.TrackPopupMenu(hMenu, 0x100, (int)posMouse.X, (int)posMouse.Y, 0, hWnd, IntPtr.Zero);
                        if (cmd > 0)
                            User32.SendMessage(hWnd, 0x112, (uint)cmd, 0);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((TabItemExplorerBrowser)LeftTab.SelectedItem).ExplorerBrowser.Focus();
            MyDataContext.SelectionLeft = false;
            MyDataContext.SelectionRight = false;
        }

        private void CopyBetweenTab(TabExplorerBrowser tabSource, TabExplorerBrowser tabDestination, bool move = false)
        {
            ShellObject[] listeItems = tabSource.CurrentTab.ExplorerBrowser.SelectedItems.ToArray();
            if (listeItems?.Length > 0)
            {
                string fichier, destination;
                destination = tabDestination.CurrentTab.ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);
                foreach (ShellObject item in listeItems)
                {
                    fichier = item.GetDisplayName(DisplayNameType.FileSystemPath);
                    if (move)
                        _fileOperation.MoveItem(fichier, destination, Path.GetFileName(fichier));
                    else
                        _fileOperation.CopyItem(fichier, destination, Path.GetFileName(fichier));
                }
                _fileOperation.PerformOperations();
            }
        }

        private void CopyLeft_Click(object sender, RoutedEventArgs e)
        {
            CopyBetweenTab(LeftTab, RightTab);
        }

        private void CopyRight_Click(object sender, RoutedEventArgs e)
        {
            CopyBetweenTab(RightTab, LeftTab);
        }

        private void MoveLeft_Click(object sender, RoutedEventArgs e)
        {
            CopyBetweenTab(LeftTab, RightTab, true);
        }

        private void MoveRight_Click(object sender, RoutedEventArgs e)
        {
            CopyBetweenTab(RightTab, LeftTab, true);
        }
    }
}
