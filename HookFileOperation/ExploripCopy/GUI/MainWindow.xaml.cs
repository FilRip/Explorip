using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.HookFileOperations.Models;

using ExploripCopy.Helpers;
using ExploripCopy.ViewModels;

using ExploripSharedCopy.WinAPI;

using ManagedShell.Interop;

namespace ExploripCopy.GUI;

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

        if (ExploripSharedCopy.Helpers.WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            ExploripSharedCopy.Helpers.WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }

        Instance = this;
        base.DataContext = MainViewModels.Instance;
        _forceClose = false;

        IpcServer.CreateIpcServer();

        Icon = Constants.Icons.MainIconSource;
    }

    public void IconInSystray_Exit()
    {
        if (DataContext.ListWaiting?.Count > 0 || DataContext.OperationInProgress)
        {
            ShowWindow();
        }
        else
        {
            MainViewModels.Instance.Dispose();
            _forceClose = true;
            Close();
            Environment.Exit(0);
        }
    }

    public new MainViewModels DataContext
    {
        get { return (MainViewModels)base.DataContext; }
    }

    #region Window manager

    public void ShowWindow()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
            Activate();
        });
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

    private void TitleBar_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        // Show System Context Menu when right click on 'title' bar
        IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
        IntPtr hMenu = NativeMethods.GetSystemMenu(hWnd, false);
        Point posMouse = PointToScreen(Mouse.GetPosition(this));
        int cmd = NativeMethods.TrackPopupMenu(hMenu, NativeMethods.TrackPopUpMenuActions.RETURNCMD, (int)posMouse.X, (int)posMouse.Y, 0, hWnd, IntPtr.Zero);
        if (cmd > 0)
            NativeMethods.SendMessage(hWnd, NativeMethods.WM.SYSCOMMAND, (uint)cmd, 0);
    }

    #endregion

    #region Drag Window

    private bool _startDrag;
    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (_startDrag && WindowState != WindowState.Minimized && IsVisible && IsActive)
        {
            _startDrag = false;
            if (WindowState == WindowState.Maximized)
                Top = Mouse.GetPosition(Application.Current.MainWindow).Y;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WindowState = WindowState.Normal;
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

    private void Window_StateChanged(object sender, EventArgs e)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            DataContext.WindowMaximized = WindowState == WindowState.Maximized;
        }, System.Windows.Threading.DispatcherPriority.Background);
    }

    private void DgListWaiting_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.OriginalSource is ListView lv)
        {
            DataContext.SelectedLines = [.. lv.SelectedItems.OfType<OneFileOperation>()];
        }
    }

    private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.OriginalSource is ListView lv)
        {
            ExtensionsWpf.AdaptSize(lv, [new GridLength(0, GridUnitType.Auto), new GridLength(0.5, GridUnitType.Star), new GridLength(0, GridUnitType.Auto), new GridLength(0.5, GridUnitType.Star)]);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        StateChanged -= Window_StateChanged;
        StateChanged += Window_StateChanged;
    }

    private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;
    }

    private void RestoreWindow_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Normal;
    }
}
