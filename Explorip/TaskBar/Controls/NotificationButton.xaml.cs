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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    private void Notification_OnClick(object sender, RoutedEventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
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
