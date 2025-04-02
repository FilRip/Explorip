using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskbarViewModel(Taskbar parentControl) : ObservableObject()
{
    private AppBarEdge _currentEdge;

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(PanelOrientation));
        OnPropertyChanged(nameof(LeadingDockOrientation));
        OnPropertyChanged(nameof(TrailingDockOrientation));
        RefreshSearch();
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
    private Taskbar _parentTaskbar = parentControl;
    [ObservableProperty()]
    private bool _resizeOn;
    [ObservableProperty()]
    private bool _tabTipVisible;
    [ObservableProperty()]
    private bool _keyboardLayoutVisible;
    [ObservableProperty()]
    private bool _searchZoneVisible;
    [ObservableProperty()]
    private bool _searchButtonVisible;

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

    [RelayCommand()]
    private void CustomColor()
    {
        CustomColor win = Application.Current.Windows.OfType<CustomColor>().FirstOrDefault();
        win ??= new CustomColor();
        win.Show();
        win.Activate();
        win.MyDataContext.ParentTaskbar = ParentTaskbar;
    }

    [RelayCommand()]
    private void SmallIconTaskbar()
    {
        if (ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskButtonSize == 16)
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskButtonSize = 32;
        else
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskButtonSize = 16;
        ParentTaskbar.MyTaskList.SetStyles();
        ParentTaskbar.MyTaskList.SetTaskButtonWidth();
        ParentTaskbar.MyTaskList.MyDataContext.ForceRefresh();
    }

    [RelayCommand()]
    private void ShowSearchZone()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowSearchZone = true;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowSearchButton = false;
        RefreshSearch();
    }

    private void RefreshSearch()
    {
        SearchZoneVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowSearchZone;
        SearchButtonVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowSearchButton;
    }

    [RelayCommand()]
    private void ShowSearchButton()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowSearchZone = false;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowSearchButton = true;
        RefreshSearch();
    }

    [RelayCommand()]
    private void ShowTaskMgr()
    {
        ShellHelper.StartTaskManager();
    }
}
