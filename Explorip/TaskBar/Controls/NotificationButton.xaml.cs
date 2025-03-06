using System.Windows;
using System.Windows.Controls;

using ManagedShell.Common.Helpers;

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
        MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            ShellHelper.ShowNotificationCenter();
        }
        else
        {
            ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_A);
        }
        NumberOfNotifications.Text = "";
    }

    private void NotificationArea_NotificationBalloonShown(object sender, ManagedShell.WindowsTray.NotificationBalloonEventArgs e)
    {
        _nbNotif++;
        NumberOfNotifications.Text = _nbNotif.ToString();
    }
}
