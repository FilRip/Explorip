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
        SetTaskListAlignment(ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment,
                             ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment);
        RefreshSearch();
        SetShowTaskMan();
        SetShowWidget();
        SetShowDesktopPreview();
        SetShowCopilot();
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

    public int ToolbarColumnPosition
    {
        get { return ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarColumn; }
    }

    public int ToolbarRowPosition
    {
        get { return ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarRow; }
    }

    public int TasklistColumnPosition
    {
        get { return ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TasklistColumn; }
    }

    public int TasklistRowPosition
    {
        get { return ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TasklistRow; }
    }

    [ObservableProperty()]
    private Taskbar _parentTaskbar = parentControl;

    [ObservableProperty()]
    private bool _resizeOn, _tabTipVisible, _keyboardLayoutVisible, _searchZoneVisible, _searchButtonVisible, _taskManVisible, _widgetsVisible, _desktopPreviewVisible, _showApplicationWindowTitle, _isGroupedApplicationWindow, _isShowSmallIcon, _copilotVisible, _taskbarVisible;

    [ObservableProperty()]
    private bool _isTaskListToLeft, _isTaskListToRight, _isTaskListToCenter, _isTaskListToTop, _isTaskListToBottom, _isTaskListToVCenter;

    private void SetTaskListAlignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
        IsTaskListToLeft = horizontal == HorizontalAlignment.Left;
        IsTaskListToRight = horizontal == HorizontalAlignment.Right;
        IsTaskListToCenter = horizontal == HorizontalAlignment.Center;
        IsTaskListToTop = vertical == VerticalAlignment.Top;
        IsTaskListToBottom = vertical == VerticalAlignment.Bottom;
        IsTaskListToVCenter = vertical == VerticalAlignment.Center;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment = vertical;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment = horizontal;
        OnPropertyChanged(nameof(HorizontalTaskListAlignment));
        OnPropertyChanged(nameof(VerticalTaskListAlignment));
    }

    [RelayCommand()]
    private void AlignTaskListToLeft()
    {
        SetTaskListAlignment(HorizontalAlignment.Left, ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment);
    }

    [RelayCommand()]
    private void AlignTaskListToRight()
    {
        SetTaskListAlignment(HorizontalAlignment.Right, ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment);
    }

    [RelayCommand()]
    private void AlignTaskListToCenter()
    {
        SetTaskListAlignment(HorizontalAlignment.Center, ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListVerticalAlignment);
    }

    [RelayCommand()]
    private void AlignTaskListToCenterV()
    {
        SetTaskListAlignment(ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment, VerticalAlignment.Center);
    }

    [RelayCommand()]
    private void AlignTaskListToTop()
    {
        SetTaskListAlignment(ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment, VerticalAlignment.Top);
    }

    [RelayCommand()]
    private void AlignTaskListToBottom()
    {
        SetTaskListAlignment(ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskListHorizontalAlignment, VerticalAlignment.Bottom);
    }

    [RelayCommand()]
    private void ToolbarToTop()
    {
        SetBarRow(0, 1);
    }

    [RelayCommand()]
    private void ToolbarToBottom()
    {
        SetBarRow(1, 0);
    }

    [RelayCommand()]
    private void ToolbarToLeft()
    {
        SetBarColumn(0, 1);
    }

    [RelayCommand()]
    private void ToolbarToRight()
    {
        SetBarColumn(1, 0);
    }

    public bool IsToolbarTop
    {
        get { return ToolbarRowPosition == 0; }
    }

    public bool IsToolbarBottom
    {
        get { return ToolbarRowPosition == 1; }
    }

    public bool IsToolbarLeft
    {
        get { return ToolbarColumnPosition == 0; }
    }

    public bool IsToolbarRight
    {
        get { return ToolbarColumnPosition == 1; }
    }

    private void SetBarColumn(int toolbarColumn, int tasklistColumn)
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarColumn = toolbarColumn;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TasklistColumn = tasklistColumn;
        OnPropertyChanged(nameof(ToolbarColumnPosition));
        OnPropertyChanged(nameof(TasklistColumnPosition));
        OnPropertyChanged(nameof(IsToolbarLeft));
        OnPropertyChanged(nameof(IsToolbarRight));
    }

    private void SetBarRow(int toolbarRow, int tasklistRow)
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarRow = toolbarRow;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TasklistRow = tasklistRow;
        OnPropertyChanged(nameof(ToolbarRowPosition));
        OnPropertyChanged(nameof(TasklistRowPosition));
        OnPropertyChanged(nameof(IsToolbarTop));
        OnPropertyChanged(nameof(IsToolbarBottom));
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
        if (ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton)
        {
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone = true;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton = false;
        }
        else
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone = false;
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
        if (ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone)
        {
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchZone = false;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton = true;
        }
        else
            ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowSearchButton = false;
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
    private void ShowCopilot()
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowCopilotButton = !CopilotVisible;
        SetShowCopilot();
    }

    private void SetShowCopilot()
    {
        CopilotVisible = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowCopilotButton;
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
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowTitleApplicationWindow = !ShowApplicationWindowTitle;
        SetShowTitleWindow();
        RefreshTaskList();
    }

    private void SetShowTitleWindow()
    {
        ShowApplicationWindowTitle = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ShowTitleApplicationWindow;
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

    public IEnumerable<Control> MenuItemsPlugins
    {
        get
        {
            List<Control> result = [];
            if (PluginsManager.ListPlugins().Any())
            {
                foreach (IExploripToolbar plugin in PluginsManager.ListPlugins())
                {
                    result.Add(new MenuItem()
                    {
                        Header = plugin.Name,
                        Tag = plugin.GuidKey,
                    });
                }
            }
            else
                result.Add(new MenuItem() { Header = Constants.Localization.NO_PLUGINS, IsEnabled = false });

            result.Add(new Separator() { Style = (Style)Application.Current.FindResource("MySeparatorStyle") });
            result.Add(new MenuItem() { Header = Constants.Localization.RELOAD_PLUGINS, Tag = Constants.Localization.RELOAD_PLUGINS });
            return result;
        }
    }

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
                    Style = (Style)Application.Current.FindResource(Constants.WindowsConstants.StyleMenuItemWithSubMenu),
                };

                item.Items.Add(new MenuItem()
                {
                    Header = Constants.Localization.VISIBLE,
                    Tag = path,
                    IsChecked = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarVisible(path),
                });
                ((MenuItem)item.Items[0]).Click += ChangeToolbarVisible;

                if (plugin == null)
                {
                    item.Items.Add(new MenuItem()
                    {
                        Header = Constants.Localization.SHOW_TITLE,
                        Tag = path,
                        IsChecked = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarShowTitle(path),
                    });
                    ((MenuItem)item.Items[1]).Click += ChangeShowTitle;

                    item.Items.Add(new MenuItem()
                    {
                        Header = Constants.Localization.LARGE_ICON,
                        Tag = path,
                        IsChecked = !ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarSmallSizeIcon(path),
                    });
                    ((MenuItem)item.Items[2]).Click += ChangeShowLargeIcon;

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
                BaseToolbar tb = ParentTaskbar.ListToolbars.Children.OfType<BaseToolbar>().FirstOrDefault(t => t.BaseDataContext.Id == path);
                if (tb != null)
                    ParentTaskbar.ListToolbars.Children.Remove(tb);
            }
        }
    }

    private void ChangeShowTitle(object sender, RoutedEventArgs e)
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

    [RelayCommand()]
    public void ExpandCollapseTaskbar(object param)
    {
        if (bool.TryParse(param?.ToString(), out bool visible))
        {
            TaskbarVisible = visible;
            ParentTaskbar.FloatingTaskbar();
            ParentTaskbar.Width = (visible ? ParentTaskbar.Screen.Bounds.Width : 16);
        }
    }
}
