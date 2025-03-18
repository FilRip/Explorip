using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskManButtonViewModel : ObservableObject
{
    [RelayCommand()]
    private void ShowTaskMan()
    {
        ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.TAB);
    }
}
