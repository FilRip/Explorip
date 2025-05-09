﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Explorip.StartMenu.Window;
using Explorip.TaskBar.Utilities;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class StartButton : System.Windows.Controls.UserControl
{
    private bool allowOpenStart;
    private readonly DispatcherTimer pendingOpenTimer;
    private readonly bool _replaceStartMenu;

    public readonly static DependencyProperty StartMenuMonitorProperty = DependencyProperty.Register("StartMenuMonitor", typeof(StartMenuMonitor), typeof(StartButton));

    public StartMenuMonitor StartMenuMonitor
    {
        get { return (StartMenuMonitor)GetValue(StartMenuMonitorProperty); }
        set { SetValue(StartMenuMonitorProperty, value); }
    }

    public StartButton()
    {
        InitializeComponent();

        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        pendingOpenTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = new TimeSpan(0, 0, 0, 1)
        };
        pendingOpenTimer.Tick += (sender, args) =>
        {
            try
            {
                // if the start menu didn't open, flip the button back to unchecked
                Start.IsChecked = false;
                pendingOpenTimer.Stop();
            }
            catch (Exception) { /* Ignore errors */ }
        };
        _replaceStartMenu = ConfigManager.TaskbarReplaceStartMenu;
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
        if (_replaceStartMenu)
        {
            if (StartMenuWindow.MyStartMenu.IsVisible)
                StartMenuWindow.MyStartMenu.Hide();
            else
            {
                StartMenuWindow.MyStartMenu.Show();
                StartMenuWindow.MyStartMenu.Activate();
            }
        }
        else
        {
            if (allowOpenStart)
            {
                pendingOpenTimer.Start();
                try
                {
                    IntPtr pointeurMenuDemarrer = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.START);
                    if (pointeurMenuDemarrer != IntPtr.Zero)
                    {
                        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
                        NativeMethods.SetWindowPos(pointeurMenuDemarrer, IntPtr.Zero, (int)screen.WorkingArea.X, (int)screen.WorkingArea.Y, (int)screen.WorkingArea.Width, (int)screen.WorkingArea.Height, NativeMethods.SWP.SWP_NOACTIVATE);
                    }
                    ShellHelper.ShowStartMenu();
                }
                catch (Exception) { /* Ignore errors */ }

                return;
            }

            Start.IsChecked = false;
        }
    }

    private void Start_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        allowOpenStart = (Start.IsChecked == false);
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
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        StartMenuMonitor.StartMenuVisibilityChanged += AppVisibilityHelper_StartMenuVisibilityChanged;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        StartMenuMonitor.StartMenuVisibilityChanged -= AppVisibilityHelper_StartMenuVisibilityChanged;
    }

    private void AppVisibilityHelper_StartMenuVisibilityChanged(object sender, ManagedShell.Common.SupportingClasses.LauncherVisibilityEventArgs e)
    {
        SetStartMenuState(e.Visible);
    }
}
