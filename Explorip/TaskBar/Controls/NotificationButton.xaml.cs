using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

using Hardcodet.Wpf.TaskbarNotification.Interop;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

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
        MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
    }

    private void Notification_OnClick(object sender, RoutedEventArgs e)
    {
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            ShellHelper.ShowNotificationCenter();
        }
        else
        {
            ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_A);
        }
        NumberOfNotifications.Text = "";
        _nbNotif = 0;
    }

    private void NotificationArea_NotificationBalloonShown(object sender, ManagedShell.WindowsTray.NotificationBalloonEventArgs e)
    {
        _nbNotif++;
        NumberOfNotifications.Text = _nbNotif.ToString();
        NativeMethods.NotifyIconData nid = new()
        {
            cbSize = (uint)Marshal.SizeOf<NotifyIconData>(),
            hWnd = (uint)e.Balloon.HandleWindow,
            uID = 1,
            uFlags = NativeMethods.NIF.MESSAGE | NativeMethods.NIF.TIP | NativeMethods.NIF.INFO | NativeMethods.NIF.ICON,
            hIcon = (uint)SystemIcons.Information.Handle,
            szTip = e.Balloon.Title,
            szInfo = e.Balloon.Info,
            szInfoTitle = e.Balloon.Title,
            dwInfoFlags = NativeMethods.NIIF.INFO,
        };
        NativeMethods.Shell_NotifyIcon(NativeMethods.NIM.NIM_ADD, ref nid);
    }
}
