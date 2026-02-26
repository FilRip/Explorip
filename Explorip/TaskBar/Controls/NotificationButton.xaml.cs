using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Helpers;
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

    public new NotificationButtonViewModel DataContext
    {
        get { return (NotificationButtonViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void NotificationArea_NotificationBalloonShown(object sender, ManagedShell.WindowsTray.NotificationBalloonEventArgs e)
    {
        //DataContext.IncreaseNumberOfNotifications();
        ShellLogger.Debug($"NotificationArea Show NotificationBalloon for {e.Balloon.Title}");

        try
        {
            string appUserModelId = ShellHelper.GetAppUserModelIdForHandle(e.Balloon.HandleWindow);
            int procId = (int)ShellHelper.GetProcIdForHandle(e.Balloon.HandleWindow);
            if (string.IsNullOrWhiteSpace(appUserModelId))
            {
                Process process = Process.GetProcessById(procId);
                appUserModelId = process.ProcessName;
            }

            e.Handled = ToastHelper.Show(appUserModelId, e.Balloon.Title, e.Balloon.Info, procId);
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
        {
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
        }
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
