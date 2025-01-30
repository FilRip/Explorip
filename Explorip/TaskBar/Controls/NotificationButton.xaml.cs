using System.Windows;
using System.Windows.Controls;

using ManagedShell.Common.Helpers;

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

    private void Notification_OnClick(object sender, RoutedEventArgs e)
    {
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            ShellHelper.ShowNotificationCenter();
        }
        else
        {
            ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_A);
        }
    }
}
