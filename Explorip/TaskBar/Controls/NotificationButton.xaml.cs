using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Logging;

using Microsoft.Toolkit.Uwp.Notifications;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class NotificationButton : UserControl
{
    public NotificationButton()
    {
        InitializeComponent();
    }

    public NotificationButtonViewModel MyDataContext
    {
        get { return (NotificationButtonViewModel)DataContext; }
    }

    private void NotificationArea_NotificationBalloonShown(object sender, ManagedShell.WindowsTray.NotificationBalloonEventArgs e)
    {
        //MyDataContext.IncreaseNumberOfNotifications();
        ShellLogger.Debug($"NotificationArea Show NotificationBalloon of {e.Balloon.Title}");

        new ToastContentBuilder()
            .AddHeader(e.Balloon.Title, e.Balloon.Title, "")
            .AddText(e.Balloon.Info)
            .Show();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.FindControlParent<Taskbar>().MainScreen)
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (this.FindControlParent<Taskbar>().MainScreen)
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
    }
}
