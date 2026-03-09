using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.ShellFolders;

using Securify.ShellLink;

namespace Explorip.TaskBar.ViewModels;

public partial class ToolbarViewModel : BaseToolbarViewModel
{
    private Task _taskRefresh;
    private readonly object _lockRefresh;
    private readonly Popup _moreItems;

    public ToolbarViewModel()
    {
        _lockRefresh = new object();

        _moreItems = new Popup()
        {
            AllowsTransparency = true,
            Child = new Border()
            {
                CornerRadius = ConfigManager.PopUpCornerRadius,
                BorderThickness = new Thickness(0),
                Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            },
            StaysOpen = false,
        };
        ((Border)_moreItems.Child).Child = new ItemsControl()
        {
            Margin = new Thickness(ConfigManager.PopUpCornerRadius.TopLeft),
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = Brushes.Transparent,
        };
    }

    public override string Id => Path;

    #region Events

    partial void OnPathChanged(string value)
    {
        SetupFolder(value);
    }

    #endregion

    #region Properties

    [ObservableProperty()]
    private string _path;
    [ObservableProperty()]
    private ShellFolder _folder;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(IconSize))]
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
        try
        {
            ((Border)_moreItems.Child).Background = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskbarBackground;
        }
        catch (Exception) { /* Ignore errors */ }
        SetupFolder(Path);
    }

    private void SetupFolder(string path)
    {
        Folder?.Files?.CollectionChanged -= Files_CollectionChanged;
        if (Directory.Exists(Environment.ExpandEnvironmentVariables(path)) && ParentTaskbar != null)
        {
            Folder = ToolbarsManager.GetToolbar(path, finishLoadAsync: SetItemsSource);
            Title = Folder.DisplayName;
            if (!ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarSmallSizeIcon(Path) && !CurrentShowLargeIcon)
                ShowLargeIcon();
            ShowTitle = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarShowTitle(Path);
            DefaultSavedPosition();
        }
    }

    [RelayCommand()]
    public void RefreshFolder()
    {
        ToolbarItems = null;
        ToolbarsManager.DeleteToolbar(Path);
        foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
        {
            Toolbar bt = tb.ListToolbars.Children.OfType<Toolbar>().FirstOrDefault(t => t.DataContext.Id == Path);
            bt?.DataContext.SetupFolder(Path);
        }
    }

    private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (_taskRefresh == null || _taskRefresh.Status != TaskStatus.Running)
        {
            _taskRefresh = new Task(UpdateInvisibleIcons);
            _taskRefresh.Start();
        }
    }

    private void SetItemsSource(ShellFolder sf)
    {
        sf.FinishLoadAsync -= SetItemsSource;
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            lock (_lockRefresh)
            {
                ShellLogger.Debug($"Call SetItemsSource for {sf.DisplayName} of taskbar {ParentTaskbar.NumScreen}");
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
                    ToolbarItems.SortDescriptions.Add(new SortDescription(nameof(ShellItem.DisplayName), ListSortDirection.Ascending));
                    RefreshMyCollectionView();
                    UpdateInvisibleIcons();
                    Folder.Files.CollectionChanged -= Files_CollectionChanged;
                    Folder.Files.CollectionChanged += Files_CollectionChanged;
                }
            }
        }, System.Windows.Threading.DispatcherPriority.Background);
    }

    public void RefreshMyCollectionView()
    {
        Application.Current.Dispatcher.Invoke(() => ToolbarItems?.Refresh());
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
                ((ItemsControl)((Border)_moreItems.Child).Child).Items.Clear();
                if (_moreItems.IsOpen)
                    _moreItems.IsOpen = false;
            }
            ((ItemsControl)((Border)_moreItems.Child).Child).Items.Clear();
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
            if (((ItemsControl)((Border)_moreItems.Child).Child).Items.Count > 0 && lastVisible != null)
                AddMenuItem(lastVisible, true);
            MoreItemsVisibility = (((ItemsControl)((Border)_moreItems.Child).Child).Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed);
        });
    }

    private void AddMenuItem(ShellFile item, bool atStart = false)
    {
        if (item == null)
            return;
        MenuItem mi = CreateMenuItem(item);
        if (atStart)
            ((ItemsControl)((Border)_moreItems.Child).Child).Items.Insert(0, mi);
        else
            ((ItemsControl)((Border)_moreItems.Child).Child).Items.Add(mi);
        if (System.IO.Path.GetExtension(item.FileName) == ".lnk")
        {
            try
            {
                Shortcut sc = Shortcut.ReadFromFile(item.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    ShellFolder sf = new(newPath, IntPtr.Zero);
                    mi.Style = (Style)Application.Current.FindResource(Constants.WindowsConstants.StyleMenuItemWithSubMenu);
                    ExpandFolder(mi, sf);
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }
        else if (item.IsFolder)
        {
            mi.Style = (Style)Application.Current.FindResource(Constants.WindowsConstants.StyleMenuItemWithSubMenu);
            ExpandFolder(mi, new ShellFolder(item.Path, IntPtr.Zero));
        }
    }

    private MenuItem CreateMenuItem(ShellFile item)
    {
        item.AllowAsync = true;
        MenuItem mi = new()
        {
            Header = item.DisplayName,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskbarBackground,
            Icon = new Image()
            {
                Source = item.SmallIcon,
            },
            BorderThickness = new Thickness(1),
            BorderBrush = ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).TaskbarBackground,
            Margin = new Thickness(0),
            Tag = item,
            IsCheckable = false,
        };
        item.IconLoaded += Item_IconLoaded;
        mi.PreviewMouseLeftButtonUp += Mi_PreviewMouseLeftButtonUp;
        mi.PreviewMouseRightButtonUp += Mi_PreviewMouseRightButtonUp;
        return mi;
    }

    private void Item_IconLoaded(ShellItem si, ManagedShell.Common.Enums.IconSize size)
    {
        si.IconLoaded -= Item_IconLoaded;
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            MenuItem mi = ReturnRecursive(si, ((ItemsControl)((Border)_moreItems.Child).Child).Items.OfType<MenuItem>());
            if (mi != null)
                switch (size)
                {
                    case ManagedShell.Common.Enums.IconSize.Large:
                        mi.Icon = new Image() { Source = si.LargeIcon };
                        return;
                    case ManagedShell.Common.Enums.IconSize.Small:
                        mi.Icon = new Image() { Source = si.SmallIcon };
                        return;
                    case ManagedShell.Common.Enums.IconSize.ExtraLarge:
                        mi.Icon = new Image() { Source = si.ExtraLargeIcon };
                        return;
                    case ManagedShell.Common.Enums.IconSize.Jumbo:
                        mi.Icon = new Image() { Source = si.JumboIcon };
                        return;
                }
        }, System.Windows.Threading.DispatcherPriority.Background);
    }

    private MenuItem ReturnRecursive(ShellItem si, IEnumerable<MenuItem> list)
    {
        MenuItem found = list.SingleOrDefault(mi => mi.Tag == si);
        if (found != null)
            return found;
        foreach (ItemCollection mi in list.Select(mi => mi.Items))
        {
            if (mi.Count > 0)
            {
                found = ReturnRecursive(si, mi.OfType<MenuItem>());
                if (found != null)
                    return found;
            }
        }
        return null;
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
                subMenu.Style = (Style)Application.Current.FindResource(Constants.WindowsConstants.StyleMenuItemWithSubMenu);
                ExpandFolder(subMenu, new ShellFolder(sf.Path, IntPtr.Zero), nbRecursive++);
            }
            else if (System.IO.Path.GetExtension(sf.FileName) == ".lnk")
            {
                Shortcut sc = Shortcut.ReadFromFile(sf.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    mi.Style = (Style)Application.Current.FindResource(Constants.WindowsConstants.StyleMenuItemWithSubMenu);
                    ExpandFolder(subMenu, new ShellFolder(newPath, IntPtr.Zero), nbRecursive++);
                }
            }
        }
    }

    private static void Mi_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is MenuItem mi && mi.Tag is ShellFile sf && InvokeContextMenuHelper.InvokeContextMenu(sf, true))
            e.Handled = true;
    }

    private static void Mi_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is MenuItem mi && mi.Tag is ShellFile sf && InvokeContextMenuHelper.InvokeContextMenu(sf, false))
            e.Handled = true;
    }

    [RelayCommand()]
    private void ShowMoreItems()
    {
        _moreItems.IsOpen = true;
    }

    #endregion

    public void UnloadFolder()
    {
        try
        {
            Folder.Files?.CollectionChanged -= Files_CollectionChanged;
            int nbStillPresent = 0;
            foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
                nbStillPresent += tb.ListToolbars.Children.OfType<Toolbar>().Count(t => t.DataContext.Id == Path);
            if (nbStillPresent <= 1)
                ToolbarsManager.DeleteToolbar(Path);
            if (_moreItems != null)
            {
                ((ItemsControl)((Border)_moreItems.Child).Child).Items.Clear();
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
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarShowTitle(Path, ShowTitle);
    }

    [RelayCommand()]
    public void ShowLargeIcon()
    {
        CurrentShowLargeIcon = !CurrentShowLargeIcon;
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
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).ToolbarSmallSizeIcon(Path, !CurrentShowLargeIcon);
    }

    public double IconSize
    {
        get { return CurrentShowLargeIcon ? 32 : 16; }
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
