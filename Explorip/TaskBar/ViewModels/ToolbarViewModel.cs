﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

using Securify.ShellLink;

namespace Explorip.TaskBar.ViewModels;

public partial class ToolbarViewModel : BaseToolbarViewModel
{
    private Task _taskRefresh;
    private readonly object _lockRefresh;
    private Popup _moreItems;

    public ToolbarViewModel()
    {
        _lockRefresh = new object();
    }

    public override string Id => Path;

    private enum MenuItemId : uint
    {
        OpenParentFolder = CommonContextMenuItem.Paste + 1,
    }

    #region Events

    partial void OnPathChanged(string value)
    {
        SetupFolder(value);
    }

    partial void OnFolderChanged(ShellFolder value)
    {
        SetItemsSource();
    }

    #endregion

    #region Properties

    [ObservableProperty()]
    private string _path;
    [ObservableProperty()]
    private ShellFolder _folder;
    [ObservableProperty()]
    private bool _currentShowLargeIcon;
    [ObservableProperty()]
    private Visibility _moreItemsVisibility = Visibility.Collapsed;
    [ObservableProperty()]
    private bool _showTitle;
    [ObservableProperty()]
    private string _title;
    [ObservableProperty()]
    private ICollectionView _toolbarItems;
    [ObservableProperty()]
    private double _titleWidth;

    #endregion

    public override void Init(BaseToolbar parentControl)
    {
        base.Init(parentControl);
        SetupFolder(Path);
    }

    private void SetupFolder(string path)
    {
        Folder?.Dispose();
        if (Directory.Exists(Environment.ExpandEnvironmentVariables(path)) && ParentTaskbar != null)
        {
            Folder = new ShellFolder(Environment.ExpandEnvironmentVariables(path), IntPtr.Zero, true);
            Title = Folder.DisplayName;
            if (!ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarSmallSizeIcon(Path) && !CurrentShowLargeIcon)
                ShowLargeIcon();
            ShowTitle = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarShowTitle(Path);
            DefaultSavedPosition();
            Folder.Files.CollectionChanged += Files_CollectionChanged;
        }
    }

    [RelayCommand()]
    public void RefreshFolder()
    {
        Files_CollectionChanged(null, null);
    }

    private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (_taskRefresh == null || _taskRefresh.Status != TaskStatus.Running)
        {
            _taskRefresh = new Task(async () =>
            {
                await Task.Delay(500);
                SetItemsSource();
            });
            _taskRefresh.Start();
        }
    }

    private void SetItemsSource()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            lock (_lockRefresh)
            {
                if (Folder != null)
                {
                    int lastPosition = -1;
                    Dictionary<string, int> orders = [];
                    string config = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Config", "exploripToolbar" + ConfigManager.ToolbarNumber(Path) + ".ini");
                    if (File.Exists(config))
                    {
                        string[] lines = File.ReadAllLines(config);
                        string[] splitter;
                        foreach (string line in lines)
                        {
                            splitter = line.Split('|');
                            if (splitter.Length == 2 && int.TryParse(splitter[0], out int position))
                                orders.Add(splitter[1], position);
                        }
                        if (Folder?.Files?.Count > 0)
                            foreach (ShellItem si in Folder.Files)
                                if (orders.TryGetValue(System.IO.Path.GetFileName(si.Path), out int position))
                                {
                                    si.Position = position;
                                    lastPosition = Math.Max(lastPosition, position);
                                }
                    }
                    foreach (ShellItem si in Folder.Files.Where(sf => sf.Position == 0))
                        si.Position = ++lastPosition;
                    ToolbarItems = CollectionViewSource.GetDefaultView(Folder.Files);
                    ToolbarItems.SortDescriptions.Add(new SortDescription(nameof(ShellItem.Position), ListSortDirection.Ascending));
                    UpdateInvisibleIcons();
                    RefreshMyCollectionView();
                }
            }
        });
    }

    public void RefreshMyCollectionView()
    {
        ToolbarItems?.Refresh();
    }

    protected override void UpdateAfterMove()
    {
        UpdateInvisibleIcons();
    }

    public Toolbar MyToolbar
    {
        get { return (Toolbar)_parentControl; }
    }

    #region Expand toolbar

    public void UpdateInvisibleIcons()
    {
        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            if (Folder == null)
                return;
            double maxWidth = MyToolbar.ToolbarItems.ActualWidth - (CurrentShowLargeIcon ? 32 : 16);
            if (ShowTitle)
                maxWidth -= TitleWidth;
            double currentWidth = 0;
            if (_moreItems != null)
            {
                ((ItemsControl)_moreItems.Child).Items.Clear();
                if (_moreItems.IsOpen)
                    _moreItems.IsOpen = false;
                MyTaskbarApp.MyShellManager.TasksService.WindowActivated -= ClosePopup;
            }
            _moreItems = new Popup()
            {
                Margin = new Thickness(0),
                Child = new ItemsControl()
                {
                    Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                    Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
                },
                StaysOpen = false,
            };
            MyTaskbarApp.MyShellManager.TasksService.WindowActivated += ClosePopup;
            _moreItems.PlacementTarget = MyToolbar;
            ShellFile lastVisible = null;
            foreach (ShellFile item in Folder.Files.OrderBy(sf => sf.Position))
            {
                currentWidth += (CurrentShowLargeIcon ? 32 : 16);
                if (currentWidth > maxWidth)
                    AddMenuItem(item);
                else
                    lastVisible = item;
            }
            if (((ItemsControl)_moreItems.Child).Items.Count > 0 && lastVisible != null)
                AddMenuItem(lastVisible, true);
            MoreItemsVisibility = (((ItemsControl)_moreItems.Child).Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed);
        });
    }

    private void ClosePopup(IntPtr activatedWindow)
    {
        _moreItems.IsOpen = false;
    }

    private void AddMenuItem(ShellFile item, bool atStart = false)
    {
        if (item == null)
            return;
        MenuItem mi = CreateMenuItem(item);
        if (atStart)
            ((ItemsControl)_moreItems.Child).Items.Insert(0, mi);
        else
            ((ItemsControl)_moreItems.Child).Items.Add(mi);
        if (System.IO.Path.GetExtension(item.FileName) == ".lnk")
        {
            try
            {
                Shortcut sc = Shortcut.ReadFromFile(item.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    ShellFolder sf = new(newPath, IntPtr.Zero);
                    mi.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
                    ExpandFolder(mi, sf);
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }
        else if (item.IsFolder)
        {
            mi.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
            ExpandFolder(mi, new ShellFolder(item.Path, IntPtr.Zero));
        }
    }

    private MenuItem CreateMenuItem(ShellFile item)
    {
        item.AllowAsync = false;
        MenuItem mi = new()
        {
            Header = item.DisplayName,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Icon = new Image()
            {
                Source = item.SmallIcon,
            },
            BorderBrush = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
            Tag = item,
            IsCheckable = false,
        };
        if (mi.Icon == null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (mi.Icon == null && sw.ElapsedMilliseconds < 3000)
                mi.Icon = item.SmallIcon;
            sw.Stop();
        }
        mi.PreviewMouseLeftButtonUp += Mi_PreviewMouseLeftButtonUp;
        mi.PreviewMouseRightButtonUp += Mi_PreviewMouseRightButtonUp;
        return mi;
    }

    private void ExpandFolder(MenuItem mi, ShellFolder folder, int nbRecursive = 1)
    {
        if (nbRecursive > ConfigManager.MaxRecursiveSubFolderInToolbar || folder == null)
            return;
        foreach (ShellFile sf in folder.Files)
        {
            MenuItem subMenu = CreateMenuItem(sf);
            mi.Items.Add(subMenu);
            if (sf.IsFolder)
            {
                subMenu.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
                ExpandFolder(subMenu, new ShellFolder(sf.Path, IntPtr.Zero), nbRecursive++);
            }
            else if (System.IO.Path.GetExtension(sf.FileName) == ".lnk")
            {
                Shortcut sc = Shortcut.ReadFromFile(sf.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    mi.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
                    ExpandFolder(subMenu, new ShellFolder(newPath, IntPtr.Zero), nbRecursive++);
                }
            }
        }
    }

    private void Mi_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is MenuItem mi && mi.Tag is ShellFile sf && InvokeContextMenu(sf, true, new ShellFolder(System.IO.Path.GetDirectoryName(sf.Path), IntPtr.Zero)))
            e.Handled = true;
    }

    private void Mi_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is MenuItem mi && mi.Tag is ShellFile sf && InvokeContextMenu(sf, false, new ShellFolder(System.IO.Path.GetDirectoryName(sf.Path), IntPtr.Zero)))
            e.Handled = true;
    }

    public bool InvokeContextMenu(ShellFile file, bool isInteractive, ShellFolder parentFolder = null)
    {
        if (file == null)
        {
            return false;
        }

        _ = new ShellItemContextMenu([file], (parentFolder ?? Folder), IntPtr.Zero, HandleFileAction, isInteractive, false, new ShellMenuCommandBuilder(), GetFileCommandBuilder(file));
        return true;
    }

    private bool HandleFileAction(string action, ShellItem[] items, bool allFolders)
    {
        if (action == ((uint)MenuItemId.OpenParentFolder).ToString())
        {
            ShellHelper.StartProcess(Folder.Path);
            return true;
        }

        return false;
    }

    private ShellMenuCommandBuilder GetFileCommandBuilder(ShellFile file)
    {
        if (file == null)
        {
            return new ShellMenuCommandBuilder();
        }

        ShellMenuCommandBuilder builder = new();

        builder.AddSeparator();
        builder.AddCommand(new ShellMenuCommand()
        {
            Flags = MFT.BYCOMMAND,
            Label = Constants.Localization.OPEN_FOLDER,
            UID = (uint)MenuItemId.OpenParentFolder,
        });

        return builder;
    }

    [RelayCommand()]
    private void ShowMoreItems()
    {
        _moreItems.IsOpen = true;
    }

    #endregion

    private void UnloadFolder()
    {
        try
        {
            if (Folder.Files != null)
                Folder.Files.CollectionChanged -= Files_CollectionChanged;
            Folder?.Dispose();
            if (_moreItems != null)
            {
                MyTaskbarApp.MyShellManager.TasksService.WindowActivated -= ClosePopup;
                ((ItemsControl)_moreItems.Child).Items.Clear();
                _moreItems.IsOpen = false;
            }
        }
        catch (Exception) { /* Ignore errors */ }
        Folder = null;
    }

    [RelayCommand()]
    public void ShowHideTitle()
    {
        ShowTitle = !ShowTitle;
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarShowTitle(Path, ShowTitle);
    }

    [RelayCommand()]
    public void ShowLargeIcon()
    {
        CurrentShowLargeIcon = !CurrentShowLargeIcon;
        DataTemplateSelector dts = MyToolbar.ToolbarItems.ItemTemplateSelector;
        MyToolbar.ToolbarItems.ItemTemplateSelector = null;
        MyToolbar.ToolbarItems.ItemTemplateSelector = dts;
        Taskbar parentTaskbar = ParentTaskbar;
        if (parentTaskbar != null)
        {
            double newHeight = parentTaskbar.Height;
            if (CurrentShowLargeIcon)
                newHeight += 16;
            else
                newHeight -= 16;
            parentTaskbar.ChangeDesiredSize(newHeight, parentTaskbar.Width);
        }
        ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarSmallSizeIcon(Path, !CurrentShowLargeIcon);
    }

    [RelayCommand()]
    private void CloseToolbar()
    {
        ParentTaskbar?.ToolsBars.Children.Remove(_parentControl);
    }

    [RelayCommand()]
    private void IsVisibleChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool visible && e.NewValue != e.OldValue)
        {
            if (visible)
            {
                if (Folder != null)
                    return;

                SetupFolder(Path);
            }
            else
                UnloadFolder();
        }
    }

    [RelayCommand()]
    private void OpenFolder()
    {
        if (!string.IsNullOrWhiteSpace(Folder?.Path))
            ShellHelper.StartProcess(Folder.Path, useShellExecute: true, verb: "open");
    }
}
