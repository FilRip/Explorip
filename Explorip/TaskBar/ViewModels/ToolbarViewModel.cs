using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;
using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

using Securify.ShellLink;

namespace Explorip.TaskBar.ViewModels;

public partial class ToolbarViewModel : ObservableObject
{
    private readonly ContextMenu moreItems;
    private Toolbar _parentControl;
    private double _startX, _startY;
    private Task _taskRefresh;
    private readonly object _lockRefresh;

    public ToolbarViewModel()
    {
        _lockRefresh = new object();
        moreItems = new ContextMenu()
        {
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0, 0, 0, 0),
        };
    }

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

    public Taskbar ParentTaskbar { get; set; }

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
    private Thickness _margin;
    [ObservableProperty()]
    private double _titleWidth;

    #endregion

    public void Init(Toolbar parentControl)
    {
        _parentControl = parentControl;
        ParentTaskbar = parentControl.FindVisualParent<Taskbar>();
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
            Point point = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarPosition(Path);
            Margin = new Thickness(point.X, point.Y, Margin.Right, Margin.Bottom);
            Folder.Files.CollectionChanged += Files_CollectionChanged;
        }
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

    #region Expand toolbar

    private void UpdateInvisibleIcons()
    {
        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            if (Folder == null)
                return;
            double maxWidth = _parentControl.ToolbarItems.ActualWidth - (CurrentShowLargeIcon ? 32 : 16);
            if (ShowTitle)
                maxWidth -= TitleWidth;
            double currentWidth = 0;
            moreItems.Items.Clear();
            ShellFile lastVisible = null;
            foreach (ShellFile item in Folder.Files.OrderBy(sf => sf.Position))
            {
                currentWidth += (CurrentShowLargeIcon ? 32 : 16);
                if (currentWidth > maxWidth)
                    AddMenuItem(item);
                else
                    lastVisible = item;
            }
            if (moreItems.Items.Count > 0 && lastVisible != null)
                AddMenuItem(lastVisible, true);
            MoreItemsVisibility = (moreItems.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed);
        });
    }

    private void AddMenuItem(ShellFile item, bool atStart = false)
    {
        if (item == null)
            return;
        MenuItem mi = CreateMenuItem(item);
        if (atStart)
            moreItems.Items.Insert(0, mi);
        else
            moreItems.Items.Add(mi);
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
            Margin = new Thickness(0, 0, 0, 0),
            Tag = item,
            IsCheckable = false,
        };
        mi.PreviewMouseLeftButtonUp += Mi_PreviewMouseLeftButtonUp;
        mi.PreviewMouseRightButtonUp += Mi_PreviewMouseRightButtonUp;
        return mi;
    }

    private const int MaxRecursive = 5;
    private void ExpandFolder(MenuItem mi, ShellFolder folder, int nbRecursive = 1)
    {
        if (nbRecursive > MaxRecursive || folder == null)
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
            ManagedShell.Common.Helpers.ShellHelper.StartProcess(Folder.Path);
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
        moreItems.IsOpen = true;
    }

    #endregion

    private void UnloadFolder()
    {
        try
        {
            if (Folder.Files != null)
                Folder.Files.CollectionChanged -= Files_CollectionChanged;
            Folder?.Dispose();
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
        DataTemplateSelector dts = _parentControl.ToolbarItems.ItemTemplateSelector;
        _parentControl.ToolbarItems.ItemTemplateSelector = null;
        _parentControl.ToolbarItems.ItemTemplateSelector = dts;
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

    #region Drag'n drop toolbar in taskbar

    [RelayCommand()]
    private void MouseDown()
    {
        if (!ParentTaskbar.MyDataContext.ResizeOn)
            return;

        Grid myGrid = _parentControl.FindVisualParent<Grid>();
        _startX = Mouse.GetPosition(myGrid).X - Margin.Left;
        _startY = Mouse.GetPosition(myGrid).Y - Margin.Top;

        Mouse.OverrideCursor = Cursors.ScrollAll;
        _parentControl.CaptureMouse();
    }

    [RelayCommand()]
    private void MouseUp()
    {
        if (_parentControl.IsMouseCaptured)
        {
            _parentControl.ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarPosition(Path, new Point(Margin.Left, Margin.Top));
        }
    }

    [RelayCommand()]
    private void MouseMove(MouseEventArgs e)
    {
        if (!_parentControl.IsMouseCaptured)
            return;

        Grid myGrid = _parentControl.FindVisualParent<Grid>();
        Margin = new Thickness(Math.Max(0, Mouse.GetPosition(myGrid).X - _startX), Margin.Top, Margin.Right, Margin.Bottom);
        HitTestResult result = VisualTreeHelper.HitTest(myGrid, e.GetPosition(myGrid));
        if (result?.VisualHit != null)
        {
            if (Margin.Left == 0)
            {
                Toolbar parent = result.VisualHit.FindVisualParent<Toolbar>();
                if (parent != null)
                {
                    int previousRow = Grid.GetRow(_parentControl);
                    int newRow = Grid.GetRow(parent);
                    if (previousRow != newRow)
                    {
                        Grid.SetRow(parent, previousRow);
                        Grid.SetRow(_parentControl, newRow);
                    }
                }
            }
            else
            {
                Margin = new Thickness(Margin.Left, Mouse.GetPosition(myGrid).Y - _startY, Margin.Right, Margin.Bottom);
                Toolbar parent = result.VisualHit.FindVisualParent<Toolbar>();
                if (parent != null)
                {
                    int previousRow = Grid.GetRow(_parentControl);
                    int newRow = Grid.GetRow(parent);
                    if (previousRow != newRow)
                    {
                        int previousColumn = Grid.GetColumn(_parentControl);
                        int newColumn = Grid.GetColumn(parent);
                        if (previousColumn == newColumn)
                        {
                            Grid.SetRow(_parentControl, newRow);
                            myGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            Grid.SetColumn(_parentControl, myGrid.ColumnDefinitions.Count - 1);
                            Margin = new Thickness(0, Margin.Top, Margin.Right, Margin.Bottom);
                            _startX = 0;
                        }
                        else
                        {
                            Grid.SetColumn(_parentControl, newColumn);
                            Grid.SetColumn(parent, previousColumn);
                        }
                    }
                }
            }
        }
        UpdateInvisibleIcons();
    }

    #endregion

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
}
