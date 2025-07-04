﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTray;

using Microsoft.WindowsAPICodePack.Shell.Constants;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for NotifyIcon.xaml
/// </summary>
public partial class NotifyIcon : UserControl
{
    private bool isLoaded;
    private ManagedShell.WindowsTray.NotifyIcon TrayIcon;

    public NotifyIcon()
    {
        InitializeComponent();
    }

    private void NotifyIcon_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!isLoaded)
        {
            TrayIcon = DataContext as ManagedShell.WindowsTray.NotifyIcon;

            if (TrayIcon == null)
            {
                return;
            }

            TrayIcon.NotificationBalloonShown += TrayIcon_NotificationBalloonShown;

            // If a notification was received before we started listening, it will be here. Show the first one that is not expired.
            NotificationBalloon firstUnexpiredNotification = TrayIcon.MissedNotifications.FirstOrDefault(balloon => balloon.Received.AddMilliseconds(balloon.Timeout) > DateTime.Now);

            if (firstUnexpiredNotification != null)
            {
                BalloonControl.Show(firstUnexpiredNotification, NotifyIconBorder);
                TrayIcon.MissedNotifications.Remove(firstUnexpiredNotification);
            }

            isLoaded = true;
        }
    }

    private void NotifyIcon_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (TrayIcon != null)
        {
            TrayIcon.NotificationBalloonShown -= TrayIcon_NotificationBalloonShown;
        }
        isLoaded = false;
    }

    private void TrayIcon_NotificationBalloonShown(object sender, NotificationBalloonEventArgs e)
    {
        BalloonControl.Show(e.Balloon, NotifyIconBorder);
        e.Handled = true;
    }

    private void NotifyIcon_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        TrayIcon?.IconMouseDown(e.ChangedButton, MouseHelper.GetCursorPositionParam(), SystemInformations.DoubleClickTime);
    }

    private void NotifyIcon_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        TrayIcon?.IconMouseUp(e.ChangedButton, MouseHelper.GetCursorPositionParam(), SystemInformations.DoubleClickTime);
    }

    private void NotifyIcon_OnMouseEnter(object sender, MouseEventArgs e)
    {
        e.Handled = true;

        if (TrayIcon != null)
        {
            // update icon position for Shell_NotifyIconGetRect
            Decorator sendingDecorator = sender as Decorator;
            Point location = sendingDecorator.PointToScreen(new Point(0, 0));
            double dpiScale = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;

            TrayIcon.Placement = new NativeMethods.Rect { Top = (int)location.Y, Left = (int)location.X, Bottom = (int)(sendingDecorator.ActualHeight * dpiScale), Right = (int)(sendingDecorator.ActualWidth * dpiScale) };
            TrayIcon.IconMouseEnter(MouseHelper.GetCursorPositionParam());
        }
    }

    private void NotifyIcon_OnMouseLeave(object sender, MouseEventArgs e)
    {
        e.Handled = true;
        TrayIcon?.IconMouseLeave(MouseHelper.GetCursorPositionParam());
    }

    private void NotifyIcon_OnMouseMove(object sender, MouseEventArgs e)
    {
        e.Handled = true;
        TrayIcon?.IconMouseMove(MouseHelper.GetCursorPositionParam());
    }

    private void NotifyIconBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if ((e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && (Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down) || Keyboard.GetKeyStates(Key.RightCtrl).HasFlag(KeyStates.Down)))) && TrayIcon != null)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
                    tb.MySystray.ChangePinItem(TrayIcon);
            });
            e.Handled = true;
        }
    }

    private void NotifyIconBorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && (Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down) || Keyboard.GetKeyStates(Key.RightCtrl).HasFlag(KeyStates.Down))))
            e.Handled = true;
    }
}
