﻿using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using Microsoft.Toolkit.Uwp.Notifications;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class NotificationButton : UserControl
{
    private int _nbNotif;

    public NotificationButton()
    {
        InitializeComponent();
        _nbNotif = 0;
    }

    private void Notification_OnClick(object sender, RoutedEventArgs e)
    {
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            ShellHelper.ShowNotificationCenter();
        }
        else
        {
            ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_A);
        }
        NumberOfNotifications.Text = "";
        _nbNotif = 0;
    }

    private void NotificationArea_NotificationBalloonShown(object sender, ManagedShell.WindowsTray.NotificationBalloonEventArgs e)
    {
        _nbNotif++;
        NumberOfNotifications.Text = _nbNotif.ToString();
        new ToastContentBuilder()
            .AddText(e.Balloon.Title)
            .AddText(e.Balloon.Info)
            .Show();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.FindVisualParent<Taskbar>().MainScreen)
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (this.FindVisualParent<Taskbar>().MainScreen)
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
    }
}
