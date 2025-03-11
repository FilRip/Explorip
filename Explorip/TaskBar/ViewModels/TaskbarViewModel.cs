using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ManagedShell.AppBar;
using ManagedShell.WindowsTray;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskbarViewModel : ObservableObject
{
    private AppBarEdge _currentEdge;

    public TaskbarViewModel(Taskbar parentControl) : base()
    {
        _parentTaskbar = parentControl;
        ShowTabTip = Visibility.Hidden;
    }

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(PanelOrientation));
        OnPropertyChanged(nameof(LeadingDockOrientation));
        OnPropertyChanged(nameof(TrailingDockOrientation));
    }

    public Orientation PanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }

    public Dock LeadingDockOrientation
    {
        get
        {
            if (_currentEdge == AppBarEdge.Left || _currentEdge == AppBarEdge.Right)
                return Dock.Top;
            else
                return Dock.Left;
        }
    }

    public Dock TrailingDockOrientation
    {
        get
        {
            if (_currentEdge == AppBarEdge.Left || _currentEdge == AppBarEdge.Right)
                return Dock.Bottom;
            else
                return Dock.Right;
        }
    }

    [ObservableProperty()]
    private Taskbar _parentTaskbar;

    [ObservableProperty()]
    private bool _resizeOn;

    [ObservableProperty()]
    private Visibility _showTabTip;
}
