﻿using System;
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

    public VerticalAlignment VerticalTaskListAlignment
    {
        get { return ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment; }
    }

    public HorizontalAlignment HorizontalTaskListAlignment
    {
        get { return ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment; }
    }

    [ObservableProperty()]
    private Taskbar _parentTaskbar = parentControl;

    [ObservableProperty()]
    private bool _resizeOn, _tabTipVisible, _keyboardLayoutVisible, _searchZoneVisible, _searchButtonVisible, _taskManVisible, _widgetsVisible, _desktopPreviewVisible, _showApplicationWindowTitle, _isGroupedApplicationWindow, _isShowSmallIcon;

    [RelayCommand()]
    private void AlignTaskListToLeft()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment = HorizontalAlignment.Left;
        OnPropertyChanged(nameof(HorizontalTaskListAlignment));
    }

    [RelayCommand()]
    private void AlignTaskListToRight()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment = HorizontalAlignment.Right;
        OnPropertyChanged(nameof(HorizontalTaskListAlignment));
    }

    [RelayCommand()]
    private void AlignTaskListToCenter()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment = HorizontalAlignment.Center;
        OnPropertyChanged(nameof(HorizontalTaskListAlignment));
    }

    [RelayCommand()]
    private void AlignTaskListToCenterV()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment = VerticalAlignment.Center;
        OnPropertyChanged(nameof(VerticalTaskListAlignment));
    }

    [RelayCommand()]
    private void AlignTaskListToTop()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment = VerticalAlignment.Top;
        OnPropertyChanged(nameof(VerticalTaskListAlignment));
    }

    [RelayCommand()]
    private void AlignTaskListToBottom()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment = VerticalAlignment.Bottom;
        OnPropertyChanged(nameof(VerticalTaskListAlignment));
    }

    [RelayCommand()]
    private void ShowHideTabTip()
    {
        TabTipVisible = !TabTipVisible;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowTabTip = TabTipVisible;
    }

    [RelayCommand()]
    private void ShowKeyboardLayout()
    {
        KeyboardLayoutVisible = !KeyboardLayoutVisible;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowKeyboardLayout = KeyboardLayoutVisible;
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
        if (ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskButtonSize == 16)
        {
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskButtonSize = 32;
            IsShowSmallIcon = false;
        }
        else
        {
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskButtonSize = 16;
            IsShowSmallIcon = true;
        }
        ParentTaskbar.MyTaskList.MyDataContext.ChangeButtonSize();
        ParentTaskbar.MyTaskList.MyDataContext.ForceRefresh();
    }

    [RelayCommand()]
    private void ShowSearchZone()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone = true;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton = false;
        RefreshSearch();
    }

    private void RefreshSearch()
    {
        SearchZoneVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone;
        SearchButtonVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton;
    }

    [RelayCommand()]
    private void ShowSearchButton()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone = false;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton = true;
        RefreshSearch();
    }

    [RelayCommand()]
    private void ShowTaskMgr()
    {
        ShellHelper.StartTaskManager();
    }

    private void SetShowTaskMan()
    {
        TaskManVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowTaskManButton;
    }

    [RelayCommand()]
    private void ShowTaskMan()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowTaskManButton = !TaskManVisible;
        SetShowTaskMan();
    }

    private void SetShowWidget()
    {
        WidgetsVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowWidgetButton;
    }

    [RelayCommand()]
    private void ShowWidgets()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowWidgetButton = !WidgetsVisible;
        SetShowWidget();
    }

    private void SetShowDesktopPreview()
    {
        DesktopPreviewVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowDesktopPreview;
    }

    [RelayCommand()]
    private void ShowDesktopPreview()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowDesktopPreview = !DesktopPreviewVisible;
        SetShowDesktopPreview();
    }

    private void SetSmallIcon()
    {
        IsShowSmallIcon = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskButtonSize == 16;
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
        RefreshTaskList(true);
    }

    private void SetGroupApplicationWindows()
    {
        IsGroupedApplicationWindow = ConfigManager.GroupedApplicationWindow;
        MyTaskbarApp.MyShellManager.TasksService.GroupApplicationsWindows = IsGroupedApplicationWindow;
    }

    private void RefreshTaskList(bool rebuild = false)
    {
        if (rebuild)
        {
            TaskListViewModel.RebuildListWindows();
        }
        else
        {
            TaskListViewModel.RefreshAllCollectionView(null, EventArgs.Empty);
            ParentTaskbar.MyTaskList.MyDataContext.ChangeButtonSize();
            ParentTaskbar.MyTaskList.MyDataContext.ForceRefresh();
        }
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
                    IsChecked = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarVisible(title),
                });
                ((MenuItem)item.Items[0]).Click += ChangeToolbarVisible;
                item.Items.Add(new MenuItem()
                {
                    Header = Constants.Localization.SHOW_TITLE,
                    Tag = path,
                    IsChecked = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarShowTitle(title),
                });
                ((MenuItem)item.Items[1]).Click += ChangeShowTitile;
                item.Items.Add(new MenuItem()
                {
                    Header = Constants.Localization.LARGE_ICON,
                    Tag = path,
                    IsChecked = !ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarSmallSizeIcon(title),
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
                    item.Items.Add(new MenuItem()
                    {
                        Header = Constants.Localization.REFRESH,
                        Tag = path,
                    });
                    ((MenuItem)item.Items[4]).Click += RefreshToolbarFolder;
                }

                ParentTaskbar.MenuToolbars.Items.Add(item);
            }
        }
    }

    private void RefreshToolbarFolder(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is string path)
        {
            Toolbar tb = ParentTaskbar.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.MyDataContext.Id == path);
            tb.MyDataContext.RefreshFolder();
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
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarVisible(path, visible);
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
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarShowTitle(path, mi.IsChecked);
            Toolbar tb = ParentTaskbar.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.MyDataContext.Id == path);
            tb.MyDataContext.ShowHideTitle();
        }
    }

    private void ChangeShowLargeIcon(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is string path)
        {
            mi.IsChecked = !mi.IsChecked;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarSmallSizeIcon(path, !mi.IsChecked);
            Toolbar tb = ParentTaskbar.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.MyDataContext.Id == path);
            tb.MyDataContext.ShowLargeIcon();
        }
    }
}
