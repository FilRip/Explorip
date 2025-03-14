using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar.ViewModels;

public partial class SearchButtonViewModel : ObservableObject
{
    [RelayCommand()]
    private void ShowSearch()
    {
        ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_S);
    }
}
