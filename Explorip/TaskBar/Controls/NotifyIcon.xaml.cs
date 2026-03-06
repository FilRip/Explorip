using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;
using ManagedShell.WindowsTray;

using Microsoft.WindowsAPICodePack.Shell.Constants;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for NotifyIcon.xaml
/// </summary>
public partial class NotifyIcon : UserControl
{
    private bool _isLoaded, _ignoreReload;

    public NotifyIcon()
    {
        InitializeComponent();
    }

    public new ManagedShell.WindowsTray.NotifyIcon DataContext
    {
        get { return base.DataContext as ManagedShell.WindowsTray.NotifyIcon; }
        set { base.DataContext = value; }
    }

    private void NotifyIcon_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded || !_ignoreReload)
        {
            _isLoaded = true;
            if (DataContext == null)
                return;

            ShellLogger.Debug($"Create Systray Icon for {DataContext.Title}");

            if (Window.GetWindow(this) is Taskbar tb && tb.MainScreen && DataContext != null)
            {
                WeakEventManager<ManagedShell.WindowsTray.NotifyIcon, NotificationBalloonEventArgs>.RemoveHandler(DataContext, nameof(DataContext.NotificationBalloonShown), TrayIcon_NotificationBalloonShown);
                WeakEventManager<ManagedShell.WindowsTray.NotifyIcon, NotificationBalloonEventArgs>.AddHandler(DataContext, nameof(DataContext.NotificationBalloonShown), TrayIcon_NotificationBalloonShown);
            }

            // If a notification was received before we started listening, it will be here. Show the first one that is not expired.
            NotificationBalloon firstUnexpiredNotification = DataContext.MissedNotifications.FirstOrDefault(balloon => balloon.Received.AddMilliseconds(balloon.Timeout) > DateTime.Now);

            if (firstUnexpiredNotification != null)
            {
                BalloonControl.Show(firstUnexpiredNotification, NotifyIconBorder);
                DataContext.MissedNotifications.Remove(firstUnexpiredNotification);
            }
        }
    }

    private void NotifyIcon_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_ignoreReload)
            return;

        if (DataContext != null && Window.GetWindow(this) is Taskbar tb && tb.MainScreen)
        {
            WeakEventManager<ManagedShell.WindowsTray.NotifyIcon, NotificationBalloonEventArgs>.RemoveHandler(DataContext, nameof(DataContext.NotificationBalloonShown), TrayIcon_NotificationBalloonShown);
        }
        _isLoaded = false;
    }

    private void TrayIcon_NotificationBalloonShown(object sender, NotificationBalloonEventArgs e)
    {
        e.Handled = true;
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
            return;
        BalloonControl.Show(e.Balloon, NotifyIconBorder);
        e.Handled = true;
    }

    private void NotifyIcon_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
            return;
        DataContext?.IconMouseDown(e.ChangedButton, MouseHelper.GetCursorPositionParam(), SystemInformations.DoubleClickTime);
    }

    private void NotifyIcon_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
            return;
        DataContext?.IconMouseUp(e.ChangedButton, MouseHelper.GetCursorPositionParam(), SystemInformations.DoubleClickTime);
    }

    private void NotifyIcon_OnMouseEnter(object sender, MouseEventArgs e)
    {
        e.Handled = true;
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
            return;

        if (DataContext != null)
        {
            // update icon position for Shell_NotifyIconGetRect
            Decorator sendingDecorator = sender as Decorator;
            Point location = sendingDecorator.PointToScreen(new Point(0, 0));
            double dpiScale = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;

            DataContext.Placement = new NativeMethods.Rect { Top = (int)location.Y, Left = (int)location.X, Bottom = (int)(sendingDecorator.ActualHeight * dpiScale), Right = (int)(sendingDecorator.ActualWidth * dpiScale) };
            DataContext.IconMouseEnter(MouseHelper.GetCursorPositionParam());
        }
    }

    private void NotifyIcon_OnMouseLeave(object sender, MouseEventArgs e)
    {
        e.Handled = true;
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
            return;
        DataContext?.IconMouseLeave(MouseHelper.GetCursorPositionParam());
    }

    private void NotifyIcon_OnMouseMove(object sender, MouseEventArgs e)
    {
        e.Handled = true;
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
            return;
        DataContext?.IconMouseMove(MouseHelper.GetCursorPositionParam());
    }

    private void NotifyIconBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
        {
            e.Handled = true;
            return;
        }
        if ((e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && (Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down) || Keyboard.GetKeyStates(Key.RightCtrl).HasFlag(KeyStates.Down)))) && DataContext != null)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ViewModels.NotifyIconListViewModel.ChangePinItem(DataContext);
            });
            e.Handled = true;
        }
    }

    private void NotifyIconBorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (MyTaskbarApp.MyShellManager.TrayService.Disable)
        {
            e.Handled = true;
            return;
        }
        if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && (Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down) || Keyboard.GetKeyStates(Key.RightCtrl).HasFlag(KeyStates.Down))))
            e.Handled = true;
    }

    private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ignoreReload = true;
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            await Task.Delay(1000);
            _ignoreReload = false;
        });
    }
}
