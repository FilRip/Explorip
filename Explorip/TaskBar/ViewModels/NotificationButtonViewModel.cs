using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;

namespace Explorip.TaskBar.ViewModels;

public partial class NotificationButtonViewModel : ObservableObject
{
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(NumberOfNotificationsVisible))]
    private int _numberOfNotifications;

    [RelayCommand()]
    private void ShowNotification()
    {
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
