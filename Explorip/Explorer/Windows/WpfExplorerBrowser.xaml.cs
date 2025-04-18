﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Explorip.Explorer.Controls;
using Explorip.Explorer.ViewModels;
using Explorip.Helpers;

using ExploripConfig.Configuration;
using ExploripConfig.Helpers;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.Windows;

/// <summary>
/// Logique d'interaction pour WpfExplorerBrowser.xaml
/// </summary>
public partial class WpfExplorerBrowser : Window
{
    private readonly IntPtr _windowHandle;
    private readonly bool _mainSession;
    private readonly DispatcherTimer _dispatcherTimer;
    private bool _snapVisible;
    private readonly double _firstLeftTabWidth;

    public WpfExplorerBrowser() : this(Environment.GetCommandLineArgs().RemoveAt(0)) { }

    public WpfExplorerBrowser(string[] args, bool mainSession = true)
    {
        InitializeComponent();

        _mainSession = mainSession;
        _windowHandle = new WindowInteropHelper(this).EnsureHandle();
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            _dispatcherTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        string dir = null;
        bool newInstance = Array.Exists(args, arg => arg.Equals("newinstance", StringComparison.InvariantCultureIgnoreCase));
        args = MyDebug.RemoveDebugArguments(args);

        if (args.Length > 0)
        {
            try
            {
                dir = args[0];
                LeftTab.AddNewTab(dir);
            }
            catch (Exception)
            {
                dir = null;
            }
        }

        if (_mainSession && string.IsNullOrWhiteSpace(dir))
        {
            foreach (string path in ConfigManager.LeftTabs.Where(p => Directory.Exists(p)))
                LeftTab.AddNewTab(ShellObject.FromParsingName(path), Path.GetFileName(path));

            if (ConfigManager.StartTwoExplorer)
                foreach (string path in ConfigManager.RightTabs.Where(p => Directory.Exists(p)))
                    RightTab.AddNewTab(ShellObject.FromParsingName(path), Path.GetFileName(path));
        }
        if (string.IsNullOrWhiteSpace(dir))
        {
            if (LeftTab.Items.Count == 1)
                LeftTab.AddNewTab((ShellObject)Microsoft.WindowsAPICodePack.Shell.KnownFolders.KnownFolders.Desktop);
            if (RightTab.Items.Count == 1 && ConfigManager.StartTwoExplorer)
                RightTab.AddNewTab((ShellObject)Microsoft.WindowsAPICodePack.Shell.KnownFolders.KnownFolders.Desktop);
        }
        if (!ConfigManager.StartTwoExplorer && !newInstance)
            HideRightTab(newInstance);

        Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.IconeExplorateur.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(_windowHandle, true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }

        Left = ConfigManager.ExplorerPosX;
        Top = ConfigManager.ExplorerPosY;
        if (_mainSession)
            WindowState = ConfigManager.ExplorerWindowState;
        if (WindowState == WindowState.Minimized && newInstance)
            WindowState = WindowState.Normal;

        if (RightTab.Visibility == Visibility.Visible)
            _firstLeftTabWidth = ConfigManager.MyRegistryKey.OpenSubKey("LeftTab")?.ReadInteger("Width") ?? 0;
    }

    public WpfExplorerBrowserViewModel MyDataContext
    {
        get { return (WpfExplorerBrowserViewModel)DataContext; }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        MyDataContext.SelectionLeft = false;
        MyDataContext.SelectionRight = false;
        StateChanged += Window_StateChanged;
        LocationChanged += Window_LocationChanged;
        SizeChanged += Window_SizeChanged;
        if (WindowState == WindowState.Normal)
        {
            Width = ConfigManager.ExplorerSizeX;
            Height = ConfigManager.ExplorerSizeY;
        }
    }

    #region Window icon

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
                if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
                {
                    HitTestResult result = VisualTreeHelper.HitTest((WpfExplorerBrowser)sender, e.GetPosition((WpfExplorerBrowser)sender));
                    if (result.VisualHit is System.Windows.Controls.Primitives.TabPanel)
                    {
                        _startDrag = true;
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    HitTestResult result = VisualTreeHelper.HitTest((WpfExplorerBrowser)sender, e.GetPosition((WpfExplorerBrowser)sender));
                    if (result.VisualHit is System.Windows.Controls.Primitives.TabPanel)
                    {
                        IntPtr hMenu = NativeMethods.GetSystemMenu(_windowHandle, false);
                        Point posMouse = PointToScreen(Mouse.GetPosition(this));
                        int cmd = NativeMethods.TrackPopupMenu(hMenu, NativeMethods.TPM.RETURNCMD, (int)posMouse.X, (int)posMouse.Y, 0, _windowHandle, IntPtr.Zero);
                        if (cmd > 0)
                            NativeMethods.SendMessage(_windowHandle, NativeMethods.WM.SYSCOMMAND, (uint)cmd, 0);
                    }
                }
            }
        }
        catch (Exception) { /* No need to catch error */ }
    }

    #endregion

    #region Manage TabControl

    public void HideRightTab(bool newInstance = false)
    {
        RightGrid.Width = new GridLength(0);
        RightTab.Visibility = Visibility.Hidden;
        LeftTab.SetValue(Grid.ColumnSpanProperty, 6);
        if (!newInstance)
            ConfigManager.StartTwoExplorer = false;
    }

    public void ShowRightTab()
    {
        RightGrid.Width = new GridLength(1, GridUnitType.Star);
        RightTab.Visibility = Visibility.Visible;
        LeftTab.SetValue(Grid.ColumnSpanProperty, 1);
        LeftGrid.Width = _firstLeftTabWidth == 0 ? new GridLength(1.5, GridUnitType.Star) : new GridLength(_firstLeftTabWidth, GridUnitType.Pixel);
        ConfigManager.StartTwoExplorer = true;
    }

    #endregion

    #region Files operations

    private static void CopyBetweenTab(TabExplorerBrowser tabSource, TabExplorerBrowser tabDestination, bool move = false)
    {
        ShellObject[] listeItems = [.. tabSource.CurrentTabExplorer.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.OfType<ShellObject>()];
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

    private static void DeleteSelectTab(TabExplorerBrowser tab)
    {
        ShellObject[] listeItems = [.. tab.CurrentTabExplorer.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.OfType<ShellObject>()];
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

    private void Window_StateChanged(object sender, EventArgs e)
    {
        MyDataContext.WindowMaximized = (WindowState == WindowState.Maximized);
        if (WindowState == WindowState.Minimized)
            return;
        ConfigManager.ExplorerWindowState = WindowState;
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
        ConfigManager.ExplorerPosX = (int)Left;
        ConfigManager.ExplorerPosY = (int)Top;
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            ConfigManager.ExplorerSizeX = (int)Width;
            ConfigManager.ExplorerSizeY = (int)Height;
        }
    }

    private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (WindowState == WindowState.Minimized || !IsVisible || !IsActive)
            return;

        if (e.GetPosition(this).Y <= 32 && e.GetPosition(this).Y > 0 && e.ChangedButton == MouseButton.Left)
        {
            if (WindowState == WindowState.Normal)
                MaximizeWindow_Click(this, new RoutedEventArgs());
            else
                RestoreWindow_Click(this, new RoutedEventArgs());
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_mainSession)
        {
            ConfigManager.LeftTabs = [.. LeftTab.Items.OfType<TabItemExplorerBrowser>().Where(tab => tab.CurrentDirectory?.ParsingName != null).Select(tab => tab.CurrentDirectory.ParsingName)];
            if (RightTab.Visibility == Visibility.Visible)
                ConfigManager.RightTabs = [.. RightTab.Items.OfType<TabItemExplorerBrowser>().Where(tab => tab.CurrentDirectory?.ParsingName != null).Select(tab => tab.CurrentDirectory.ParsingName)];
        }
        LeftTab.CloseAllTabs();
        RightTab.CloseAllTabs();
    }

    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
        _dispatcherTimer?.Stop();
        _snapVisible = !_snapVisible;
        ManagedShell.Common.Helpers.ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_Z);
    }

    private void MaximizeWindow_MouseEnter(object sender, MouseEventArgs e)
    {
        _dispatcherTimer?.Start();
    }

    private void MaximizeWindow_MouseLeave(object sender, MouseEventArgs e)
    {
        _dispatcherTimer?.Stop();
        if (_snapVisible)
            DispatcherTimer_Tick(null, null);
    }
}
