using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Plugins;
using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

using ExploripPlugins;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;

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
        SetShowTaskMan();
        SetShowWidget();
        SetShowDesktopPreview();
        SetSmallIcon();
        SetGroupApplicationWindows();
        SetShowTitleWindow();
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
    private bool _resizeOn, _tabTipVisible, _keyboardLayoutVisible, _searchZoneVisible, _searchButtonVisible, _taskManVisible, _widgetsVisible, _desktopPreviewVisible, _showApplicationWindowTitle, _isGroupedApplicationWindow, _isShowSmallIcon;

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
        {
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskButtonSize = 32;
            IsShowSmallIcon = false;
        }
        else
        {
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskButtonSize = 16;
            IsShowSmallIcon = true;
        }
        ParentTaskbar.MyTaskList.MyDataContext.ChangeButtonSize();
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

    private void SetShowTaskMan()
    {
        TaskManVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowTaskManButton;
    }

    [RelayCommand()]
    private void ShowTaskMan()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowTaskManButton = !TaskManVisible;
        SetShowTaskMan();
    }

    private void SetShowWidget()
    {
        WidgetsVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowWidgetButton;
    }

    [RelayCommand()]
    private void ShowWidgets()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowWidgetButton = !WidgetsVisible;
        SetShowWidget();
    }

    private void SetShowDesktopPreview()
    {
        DesktopPreviewVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowDesktopPreview;
    }

    [RelayCommand()]
    private void ShowDesktopPreview()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ShowDesktopPreview = !DesktopPreviewVisible;
        SetShowDesktopPreview();
    }

    private void SetSmallIcon()
    {
        IsShowSmallIcon = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).TaskButtonSize == 16;
    }

    [RelayCommand()]
    private void Exit()
    {
        ((MyTaskbarApp)Application.Current).ExitGracefully();
    }

    [RelayCommand()]
    private void TaskbarAllScreen()
    {
        ((MyTaskbarApp)Program.MyCurrentApp).ShowTaskbarOnAllOthersScreen();
    }

    [RelayCommand()]
    private void ShowTitleWindow()
    {
        ConfigManager.ShowTitleApplicationWindow = !ShowApplicationWindowTitle;
        SetShowTitleWindow();
        RefreshTaskList();
    }

    private void SetShowTitleWindow()
    {
        ShowApplicationWindowTitle = ConfigManager.ShowTitleApplicationWindow;
    }

    [RelayCommand()]
    private void GroupApplicationWindows()
    {
        ConfigManager.GroupedApplicationWindow = !IsGroupedApplicationWindow;
        SetGroupApplicationWindows();
        RefreshTaskList();
    }

    private void SetGroupApplicationWindows()
    {
        IsGroupedApplicationWindow = ConfigManager.GroupedApplicationWindow;
        MyTaskbarApp.MyShellManager.TasksService.GroupApplicationsWindows = IsGroupedApplicationWindow;
    }

    private void RefreshTaskList()
    {
        TaskListViewModel.RefreshAllCollectionView(null, EventArgs.Empty);
        ParentTaskbar.MyTaskList.MyDataContext.ChangeButtonSize();
        ParentTaskbar.MyTaskList.MyDataContext.ForceRefresh();
    }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public IEnumerable<string> ListPlugins
    {
        get
        {
            if (PluginsManager.ListName().Any())
                return PluginsManager.ListName();
            else
                return ["No plugins loaded"];
        }
    }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    public void BuildToolbarsMenu()
    {
        ParentTaskbar.MenuToolbars.Items.Clear();
        foreach (string path in ConfigManager.ToolbarsPath)
        {
            IExploripToolbar plugin = null;
            string title = null;
            if (Guid.TryParse(path, out Guid guid))
            {
                plugin = PluginsManager.GetPlugin(guid);
                if (plugin != null)
                    title = plugin.Name;
            }
            else
            {
                ShellFolder sf = new(Environment.ExpandEnvironmentVariables(path), IntPtr.Zero);
                title = sf.DisplayName;
                sf.Dispose();
            }
            if (title != null)
            {
                MenuItem item = new()
                {
                    Header = title,
                    Tag = path,
                    Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
                    Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                    Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle"),
                };
                item.Items.Add(new MenuItem()
                {
                    Header = Constants.Localization.VISIBLE,
                    Tag = path,
                    IsChecked = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarVisible(title),
                });
                ((MenuItem)item.Items[0]).Click += ChangeToolbarVisible;
                item.Items.Add(new MenuItem()
                {
                    Header = Constants.Localization.SHOW_TITLE,
                    Tag = path,
                    IsChecked = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarShowTitle(title),
                });
                ((MenuItem)item.Items[1]).Click += ChangeShowTitile;
                item.Items.Add(new MenuItem()
                {
                    Header = Constants.Localization.LARGE_ICON,
                    Tag = path,
                    IsChecked = !ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarSmallSizeIcon(title),
                });
                ((MenuItem)item.Items[2]).Click += ChangeShowLargeIcon;
                if (plugin == null)
                {
                    item.Items.Add(new MenuItem()
                    {
                        Header = Constants.Localization.OPEN_FOLDER,
                        Tag = path,
                    });
                    ((MenuItem)item.Items[3]).Click += OpenToolbarFolder;
                }

                ParentTaskbar.MenuToolbars.Items.Add(item);
            }
        }
    }

    private static void OpenToolbarFolder(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is string path)
        {
            ShellHelper.StartProcess(Environment.ExpandEnvironmentVariables(path), useShellExecute: true, verb: "open");
        }
    }

    private void ChangeToolbarVisible(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is string path)
        {
            bool visible = !mi.IsChecked;
            mi.IsChecked = visible;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarVisible(path, visible);
            if (visible)
            {
                if (Guid.TryParse(path, out Guid guid))
                {
                    IExploripToolbar plugin = PluginsManager.GetPlugin(guid);
                    if (plugin != null)
                        ParentTaskbar.AddToolbar(plugin);
                }
                else
                    ParentTaskbar.AddToolbar(path);
            }
            else
            {
                Toolbar tb = ParentTaskbar.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.MyDataContext.Id == path);
                if (tb != null)
                    ParentTaskbar.ListToolbars.Children.Remove(tb);
            }
        }
    }

    private void ChangeShowTitile(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is string path)
        {
            mi.IsChecked = !mi.IsChecked;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarShowTitle(path, mi.IsChecked);
            Toolbar tb = ParentTaskbar.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.MyDataContext.Id == path);
            tb.MyDataContext.ShowHideTitle();
        }
    }

    private void ChangeShowLargeIcon(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is string path)
        {
            mi.IsChecked = !mi.IsChecked;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarSmallSizeIcon(path, !mi.IsChecked);
            Toolbar tb = ParentTaskbar.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.MyDataContext.Id == path);
            tb.MyDataContext.ShowLargeIcon();
        }
    }
}
