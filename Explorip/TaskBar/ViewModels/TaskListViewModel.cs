using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.Helpers;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskListViewModel : ObservableObject
{
    private AppBarEdge _currentEdge;

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(PanelOrientation));
    }

    public Orientation PanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }
}
