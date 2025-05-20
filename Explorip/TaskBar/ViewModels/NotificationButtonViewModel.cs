using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.ViewModels;

public partial class NotificationButtonViewModel : ObservableObject
{
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(NumberOfNotificationsVisible))]
    private int _numberOfNotifications;

    [RelayCommand()]
    private void ShowNotification()
    {
        IntPtr ptrNotifCenterWindow = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.NOTIFICATION_CENTER);
        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
        NativeMethods.SetWindowPos(ptrNotifCenterWindow, IntPtr.Zero, (int)(screen.WorkingArea.Right) - 400, (int)(screen.WorkingArea.Top * screen.ScaleFactor), (int)(400 * screen.ScaleFactor), (int)(screen.WorkingArea.Height * screen.ScaleFactor), NativeMethods.SWP.SWP_NOACTIVATE);
        ShellHelper.ShowNotificationCenter();
        NumberOfNotifications = 0;
    }

    public void IncreaseNumberOfNotifications()
    {
        NumberOfNotifications++;
    }

    public bool NumberOfNotificationsVisible
    {
        get { return NumberOfNotifications > 0; }
    }
}
