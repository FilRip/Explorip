using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.Helpers;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.ViewModels;

public partial class NotifyIconListViewModel : ObservableObject
{
    private AppBarEdge _currentEdge;

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(StackPanelOrientation));
    }

    public Orientation StackPanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }
}
