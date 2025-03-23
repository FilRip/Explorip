using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskbarViewModel : ObservableObject
{
    private AppBarEdge _currentEdge;

    public TaskbarViewModel(Taskbar parentControl) : base()
    {
        _parentTaskbar = parentControl;
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
    private bool _tabTipVisible;
    [ObservableProperty()]
    private bool _keyboardLayoutVisible;

    [RelayCommand()]
    private void ShowHideTabTip()
    {
        TabTipVisible = !TabTipVisible;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowTabTip = TabTipVisible;
    }

    [RelayCommand()]
    private void ShowKeyboardLayout()
    {
        KeyboardLayoutVisible = !KeyboardLayoutVisible;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowKeyboardLayout = KeyboardLayoutVisible;
    }
}
