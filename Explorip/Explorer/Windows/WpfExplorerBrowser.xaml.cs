using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Explorip.Explorer.Controls;
using Explorip.Explorer.ViewModels;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Explorer.Windows;

/// <summary>
/// Logique d'interaction pour WpfExplorerBrowser.xaml
/// </summary>
public partial class WpfExplorerBrowser : Window
{
    public WpfExplorerBrowser() : this(true) { }

    public WpfExplorerBrowser(bool openDefaultView)
    {
        InitializeComponent();

        if (openDefaultView)
        {
            string dir = null;
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                dir = Environment.GetCommandLineArgs()[1].Trim().ToLower() == "explorer" && Environment.GetCommandLineArgs().Length > 2
                    ? Environment.GetCommandLineArgs()[2]
                    : Environment.GetCommandLineArgs()[1];
                try
                {
                    LeftTab.FirstTab.Navigation(dir);
                }
                catch (Exception)
                {
                    dir = null;
                }
            }

            if (string.IsNullOrWhiteSpace(dir))
                LeftTab.FirstTab.ExplorerBrowser.Navigate((ShellObject)Microsoft.WindowsAPICodePack.Shell.KnownFolders.Desktop);

            RightTab.FirstTab.ExplorerBrowser.Navigate((ShellObject)Microsoft.WindowsAPICodePack.Shell.KnownFolders.Desktop);
        }

        Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.IconeExplorateur.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        // TODO : https://stackoverflow.com/questions/69097246/how-to-support-for-windows-11-snap-layout-to-the-custom-maximize-restore-butto
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    public WpfExplorerBrowserViewModel MyDataContext
    {
        get { return (WpfExplorerBrowserViewModel)DataContext; }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        MyDataContext.SelectionLeft = false;
        MyDataContext.SelectionRight = false;
    }

    #region Window icon

    private void CloseWindow_Click(object sender, RoutedEventArgs e)
    {
        LeftTab.CloseAllTabs();
        RightTab.CloseAllTabs();
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
    }

    private void SetWindowNormal()
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
    }

    #endregion

    #region Title bar

    private bool _startDrag;
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (WindowState == WindowState.Minimized || !IsVisible || !IsActive)
                return;

            if (e.GetPosition(this).Y <= 32 && e.GetPosition(this).Y > 0)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (e.ClickCount == 2 && e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (WindowState == WindowState.Normal)
                            MaximizeWindow_Click(this, new RoutedEventArgs());
                        else
                            RestoreWindow_Click(this, new RoutedEventArgs());
                    }
                    else
                    {
                        HitTestResult result = VisualTreeHelper.HitTest((WpfExplorerBrowser)sender, e.GetPosition((WpfExplorerBrowser)sender));
                        if (result.VisualHit is System.Windows.Controls.Primitives.TabPanel)
                        {
                            _startDrag = true;
                        }
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    HitTestResult result = VisualTreeHelper.HitTest((WpfExplorerBrowser)sender, e.GetPosition((WpfExplorerBrowser)sender));
                    if (result.VisualHit is System.Windows.Controls.Primitives.TabPanel)
                    {
                        IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
                        IntPtr hMenu = NativeMethods.GetSystemMenu(hWnd, false);
                        Point posMouse = PointToScreen(Mouse.GetPosition(this));
                        int cmd = NativeMethods.TrackPopupMenu(hMenu, 0x100, (int)posMouse.X, (int)posMouse.Y, 0, hWnd, IntPtr.Zero);
                        if (cmd > 0)
                            NativeMethods.SendMessage(hWnd, 0x112, (uint)cmd, 0);
                    }
                }
            }
        }
        catch (Exception) { /* No need to catch error */ }
    }

    #endregion

    #region Manage TabControl

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

    #endregion

    #region Files operations

    private void CopyBetweenTab(TabExplorerBrowser tabSource, TabExplorerBrowser tabDestination, bool move = false)
    {
        ShellObject[] listeItems = tabSource.CurrentTabExplorer.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.ToArray();
        string destination = tabDestination.CurrentTabExplorer.ExplorerBrowser.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);
        Task.Run(() =>
        {
            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
            if (listeItems?.Length > 0)
            {
                string fichier;
                foreach (ShellObject item in listeItems)
                {
                    fichier = item.GetDisplayName(DisplayNameType.FileSystemPath);
                    if (move)
                        fileOperation.MoveItem(fichier, destination, Path.GetFileName(fichier));
                    else
                        fileOperation.CopyItem(fichier, destination, Path.GetFileName(fichier));
                }
                fileOperation.PerformOperations();
                fileOperation.Dispose();
            }
        });
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

    private void DeleteSelectTab(TabExplorerBrowser tab)
    {
        ShellObject[] listeItems = tab.CurrentTabExplorer.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.ToArray();
        Task.Run(() =>
        {
            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
            if (listeItems?.Length > 0)
            {
                foreach (ShellObject file in listeItems)
                {
                    fileOperation.DeleteItem(file.GetDisplayName(DisplayNameType.FileSystemPath));
                }
                fileOperation.PerformOperations();
                fileOperation.Dispose();
            }
        });
    }

    private void DeleteLeft_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectTab(LeftTab);
    }

    private void DeleteRight_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectTab(RightTab);
    }

    #endregion

    #region Window move

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (_startDrag && WindowState != WindowState.Minimized && IsVisible && IsActive)
        {
            _startDrag = false;
            if (WindowState == WindowState.Maximized)
                Top = Mouse.GetPosition(Application.Current.MainWindow).Y;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetWindowNormal();
                DragMove();
            }
        }
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _startDrag = false;
    }

    #endregion

    private void Window_Activated(object sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                Thread.Sleep(100);
                if (IsActive && WindowState == WindowState.Minimized)
                {
                    if (MyDataContext.WindowMaximized)
                        WindowState = WindowState.Maximized;
                    else
                        WindowState = WindowState.Normal;
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }, System.Windows.Threading.DispatcherPriority.Background);
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        MyDataContext.WindowMaximized = (WindowState == WindowState.Maximized);
    }
}
