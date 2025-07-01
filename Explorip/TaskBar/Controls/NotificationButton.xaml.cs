using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

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
