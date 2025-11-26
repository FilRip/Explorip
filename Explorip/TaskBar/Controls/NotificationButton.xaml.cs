using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Logging;

using Microsoft.Toolkit.Uwp.Notifications;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class NotificationButton : UserControl
{
    private bool _isLoaded, _ignoreReload = false;

    public NotificationButton()
    {
        InitializeComponent();
        this.IsVisibleChanged += NotificationButton_IsVisibleChanged;
    }

    private void NotificationButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ignoreReload = true;
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            await Task.Delay(1000);
            _ignoreReload = false;
        });
    }

    public NotificationButtonViewModel MyDataContext
    {
        get { return (NotificationButtonViewModel)DataContext; }
    }

    private void NotificationArea_NotificationBalloonShown(object sender, ManagedShell.WindowsTray.NotificationBalloonEventArgs e)
    {
        //MyDataContext.IncreaseNumberOfNotifications();
#pragma warning disable CS0618
        DesktopNotificationManagerCompat.RegisterAumidAndComServer<MyNotificationActivator>("CoolBytes.Explorip");
        DesktopNotificationManagerCompat.RegisterActivator<MyNotificationActivator>();
#pragma warning restore CS0618

        ShellLogger.Debug($"NotificationArea Show NotificationBalloon of {e.Balloon.Title}");

        string title = e.Balloon.Title;
        string message = e.Balloon.Info;

        new ToastContentBuilder()
            .AddHeader(title, title, "")
            .AddText(message)
            .Show();

        e.Handled = true;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if ((!_isLoaded || !_ignoreReload) && ((Taskbar)Window.GetWindow(this)).MainScreen && MyTaskbarApp.MyShellManager != null)
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
        _isLoaded = true;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) || _ignoreReload)
            return;

        if (Window.GetWindow(this) is Taskbar parentTaskbar && parentTaskbar.MainScreen && MyTaskbarApp.MyShellManager != null)
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
        _isLoaded = false;
    }
}
