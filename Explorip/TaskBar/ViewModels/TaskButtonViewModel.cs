using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.WindowsTasks;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskButtonViewModel : ObservableObject
{
    [ObservableProperty()]
    private ApplicationWindow _window;

}
