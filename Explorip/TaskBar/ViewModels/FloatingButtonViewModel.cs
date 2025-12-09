using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

namespace Explorip.TaskBar.ViewModels;

public partial class FloatingButtonViewModel : ObservableObject
{
    public Taskbar ParentTaskbar { get; set; }

    [RelayCommand()]
    private void ExpandTaskbar()
    {
        ParentTaskbar.MyDataContext.ExpandCollapseTaskbar("True");
    }
}
