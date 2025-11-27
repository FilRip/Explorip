using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Logging;

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
        ShellLogger.Debug($"NotificationArea Show NotificationBalloon of {e.Balloon.Title}");

        string title = e.Balloon.Title;
        string message = e.Balloon.Info;

        try
        {
            ToastHelper.Show(title, message);
            e.Handled = true;
        }
        catch (Exception ex)
        {
            ShellLogger.Error($"Unable to create windows notification : {ex.Message}");
        }
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
