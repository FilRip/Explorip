﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.HookFileOperations.Models;

using ExploripCopy.Helpers;
using ExploripCopy.ViewModels;

using ExploripSharedCopy.Helpers;
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

        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }

        Instance = this;
        DataContext = MainViewModels.Instance;
        MyDataContext.ForceRefreshList += ForceRefresh;
        _forceClose = false;

        IpcServer.CreateIpcServer();

        Icon = Constants.Icons.MainIconSource;
    }

    public void IconInSystray_Exit()
    {
        MainViewModels.Instance.Dispose();
        _forceClose = true;
        Close();
        Environment.Exit(0);
    }

    private MainViewModels MyDataContext
    {
        get { return (MainViewModels)DataContext; }
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
        IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
        IntPtr hMenu = NativeMethods.GetSystemMenu(hWnd, false);
        Point posMouse = PointToScreen(Mouse.GetPosition(this));
        int cmd = NativeMethods.TrackPopupMenu(hMenu, NativeMethods.TPM.RETURNCMD, (int)posMouse.X, (int)posMouse.Y, 0, hWnd, IntPtr.Zero);
        if (cmd > 0)
            NativeMethods.SendMessage(hWnd, NativeMethods.WM.SYSCOMMAND, (uint)cmd, 0);
    }

    private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
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
        MyDataContext.ForceRefreshList -= ForceRefresh;
    }

    private void ForceRefresh(object sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                DgListWaiting.Items.Refresh();
            }
            catch (Exception)
            {
                // Nothing to do
            }
        });
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        MyDataContext.WindowMaximized = WindowState == WindowState.Maximized;
    }

    private void DgListWaiting_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        MyDataContext.SelectedLines = [.. DgListWaiting.SelectedItems.OfType<OneFileOperation>()];
    }

    private void StartNow_Click(object sender, RoutedEventArgs e)
    {
        MyDataContext.SelectedLines = [(OneFileOperation)((FrameworkElement)e.Source).DataContext];
        MyDataContext.StartNow();
    }
}
