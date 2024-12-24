using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.FilesOperations;
using Explorip.FilesOperations.Interfaces;
using Explorip.Helpers;

using ExploripComponents.Controls;
using ExploripComponents.Helpers;
using ExploripComponents.Models;

using ManagedShell.Common.Enums;
using ManagedShell.Interop;

using WpfToolkit.Controls;

namespace ExploripComponents.ViewModels;

public partial class WpfExplorerViewModel : ObservableObject
{
    #region Binding fields

    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _folderTreeView = [];
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(NumberOfFiles))]
    private ObservableCollection<OneFileSystem> _fileListView = [];
    [ObservableProperty()]
    private OneDirectory? _selectedFolder;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(NumberOfSelectedItems))]
    private ObservableCollection<OneFileSystem> _selectedItems = [];
    [ObservableProperty()]
    private IconSize _currentIconSize = IconSize.Small;
    [ObservableProperty()]
    private bool _viewDetails = false;
    [ObservableProperty()]
    private ICollectionView? _currentGroupBy;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ListViewVisible))]
    private bool _errorVisible;
    [ObservableProperty()]
    private string? _errorMessage;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ListViewVisible))]
    private bool _pleaseWait;

    #endregion

    private readonly MainWindow _control;
    private OneFileSystem? _currentlyRenaming;
    private ItemsPanelTemplate? _itemTemplateDetails;
    private ItemsPanelTemplate? _itemTemplateWrap;
    private readonly Stopwatch _detectRename;
    private OneFileSystem? _lastSelected;
    private string? _recycledFullPath;
    private string? _localizedNameRecycleBin;

    #region Constructors

    public WpfExplorerViewModel(MainWindow control)
    {
        _control = control;
        ChangeItemTemplate();
        CurrentGroup = GroupBy.NONE;
        CurrentOrderBy = OrderBy.NONE;
        _detectRename = Stopwatch.StartNew();
    }

    [RelayCommand()]
    public void Refresh()
    {
        FolderTreeView.Clear();
        OneDirectory myComputer = new(Environment.SpecialFolder.MyComputer, null, true) { MainViewModel = this, FullPath = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}" };
        FolderTreeView.Add(myComputer);
        myComputer.IsExpanded = true;

        // Add Network root
        string specialPath = "::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}";
        ManagedShell.ShellFolders.ShellItem si = new(specialPath);
        OneDirectory networkNeiborhood = new(specialPath, null, true, si.DisplayName) { MainViewModel = this, NetworkRoot = true, IsItemVisible = true };
        FolderTreeView.Add(networkNeiborhood);

        // Add recycled bin
        specialPath = "::{645FF040-5081-101B-9F08-00AA002F954E}";
        si = new(specialPath);
        OneDirectory recycledBin = new(specialPath, null, true, si.DisplayName) { MainViewModel = this, IsItemVisible = true, RecycledBin = true };
        FolderTreeView.Add(recycledBin);
        string drive = Environment.SpecialFolder.Windows.FullPath().Substring(0, 3);
        _localizedNameRecycleBin = si.DisplayName;
        _recycledFullPath = Helpers.ExtensionsDirectory.SearchRecycledBinPath(drive, si.DisplayName);

        // Add blank line for the horizontal scrollbar
        FolderTreeView.Add(new OneDirectory("", null, false, "") { Children = [] });

        if (Environment.GetCommandLineArgs().Length > 1)
            Application.Current.Dispatcher.BeginInvoke(() => BrowseTo(Environment.GetCommandLineArgs()[1]));
        else
        {
            BrowseTo(null);
            SelectedFolder!.Refresh();
        }

        OnPropertyChanged(nameof(ListViewVisible));
        SetDisplay();
    }

    #endregion

    #region Drag'n drop

    private readonly Stopwatch _stopWatchDrop = new();
    private bool _currentlyDragging;
    public bool CurrentlyDraging
    {
        get { return _currentlyDragging; }
        set
        {
            _currentlyDragging = value;
            if (value)
                _stopWatchDrop.Restart();
            else
                _stopWatchDrop.Stop();
        }
    }
    public long NbMillisecondsStartDragging
    {
        get
        {
            if (_stopWatchDrop.IsRunning)
                return _stopWatchDrop.ElapsedMilliseconds;
            else
                return 0;
        }
    }

    public DragDropKeyStates DragDropKeyStates { get; set; }

    [RelayCommand()]
    public void DoubleClick(MouseButtonEventArgs e)
    {
        _lastSelected = null;
        _detectRename.Restart();
    }

    [RelayCommand()]
    public void MouseUp(MouseButtonEventArgs e)
    {
        if (CurrentControl.FileLV.DrawSelection)
            return;
        CurrentlyDraging = false;
        if (_currentlyRenaming == null &&
            SelectedItems.Count == 1 && e.ChangedButton == MouseButton.Left)
        {
            if (_lastSelected == SelectedItems[0] && _detectRename.ElapsedMilliseconds > Constants.DoubleClickDelay)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(Constants.DelayIgnoreRename);
                    if (_lastSelected == SelectedItems[0])
                        RenameMode();
                });
            }
            else
            {
                _detectRename.Restart();
                _lastSelected = SelectedItems[0];
            }
            return;
        }
        _lastSelected = null;
    }

    #endregion

    #region Properties

    public MainWindow CurrentControl
    {
        get { return _control; }
    }

    public string? FullPathRecycledBin
    {
        get { return _recycledFullPath; }
    }

    public string? LocalizedNameRecycleBin
    {
        get { return _localizedNameRecycleBin; }
    }

    public bool ListViewVisible
    {
        get { return !ErrorVisible && !PleaseWait; }
    }

    #endregion

    public void BrowseTo(string? fullPath, bool addNavigation = true)
    {
        if (fullPath?.StartsWith("\\\\") == true)
        {
            // TODO : Network search
        }
        else
        {
            if (!FolderTreeView[0].IsExpanded)
                FolderTreeView[0].IsExpanded = true;
            OneDirectory actualFolder = FolderTreeView[0];
            if (!string.IsNullOrWhiteSpace(fullPath))
            {
                if (fullPath!.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    fullPath = fullPath.Substring(0, fullPath.Length - 1);
                string[] folders = fullPath.Split(Path.DirectorySeparatorChar);
                string currentPath;
                foreach (string folder in folders)
                {
                    if (actualFolder.Children.Count == 0)
                        actualFolder.Refresh();
                    foreach (OneDirectory subFolder in actualFolder.Children)
                    {
                        currentPath = subFolder.FullPath.TrimEnd(Path.DirectorySeparatorChar);
                        if (subFolder.Drive == null)
                            currentPath = Path.GetFileName(currentPath);

                        if (currentPath.ToLower() == folder.ToLower() ||
                            (fullPath.StartsWith(((char)255).ToString()) && subFolder.DisplayText == fullPath.Replace(((char)255).ToString(), string.Empty)))
                        {
                            subFolder.IsExpanded = true;
                            actualFolder = subFolder;
                            break;
                        }
                    }
                }
            }
            actualFolder.IsSelected = true;
        }

        if (addNavigation)
            AddNavigation(fullPath);
        else
            RefreshNavigation();
    }

    public void AddNavigation(string? fullPath, string? specialFolder = null)
    {
        if (_currentPosition >= 0 && _navigationItems[_currentPosition] == fullPath)
            return;
        if (_navigationItems.Count > 0 && _navigationItems.Count - 1 > _currentPosition)
            _navigationItems.RemoveRange(_currentPosition + 1, _navigationItems.Count - _currentPosition - 1);
        _currentPosition++;
        if (string.IsNullOrWhiteSpace(specialFolder))
            _navigationItems.Add(fullPath ?? "");
        else
            _navigationItems.Add(((char)255) + specialFolder);
        RefreshNavigation();
    }

    [RelayCommand()]
    public void SetFocusToListView()
    {
        CurrentControl.FileLV.Focus();
    }

    [RelayCommand()]
    public void ContextMenuBackgroundFolder(MouseButtonEventArgs e)
    {
        // When right click on empty space in list view
        if (!string.IsNullOrWhiteSpace(SelectedFolder?.FullPath))
        {
            CurrentControl.FileLV.UnselectAll();
            if (SelectedFolder!.FullPath == Environment.SpecialFolder.Desktop.FullPath())
                new ShellContextMenu(this).ShowContextMenu("::{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
            else
                new ShellContextMenu(this).ShowContextMenu(SelectedFolder!.FullPath, Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
        }
    }

    #region Show Display/Group/OrderBy

    private void ChangeItemTemplate()
    {
        _itemTemplateDetails ??= new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
        if (_itemTemplateWrap == null)
        {
            FrameworkElementFactory fef = new(typeof(VirtualizingWrapPanel));
            fef.SetValue(VirtualizingWrapPanel.OrientationProperty, Orientation.Horizontal);
            fef.SetValue(VirtualizingWrapPanel.SpacingModeProperty, SpacingMode.None);
            fef.SetValue(VirtualizingWrapPanel.IsGridLayoutEnabledProperty, true);
            fef.SetValue(VirtualizingWrapPanel.AllowDifferentSizedItemsProperty, true);
            _itemTemplateWrap = new ItemsPanelTemplate(fef);
        }
        CurrentControl.FileLV.ItemsPanel = (ViewDetails ? _itemTemplateDetails : _itemTemplateWrap);
    }

    public void SetDisplay(/* TODO : save display style per folder*/)
    {
        ChangeGroupBy(CurrentGroup);
    }

    public void ChangeIconSize(bool details, IconSize value)
    {
        ViewDetails = details;
        CurrentIconSize = value;
        OnPropertyChanged(nameof(IconSizePx));
        OnPropertyChanged(nameof(NameSizePx));
        SelectedFolder?.Refresh();
        ChangeItemTemplate();
        ChangeGroupBy(CurrentGroup);
    }

    public GroupBy CurrentGroup { get; set; }

    public OrderBy CurrentOrderBy { get; set; }

    public OrderDirection CurrentOrderDirection { get; set; }

    public void ChangeGroupBy(GroupBy newGroup)
    {
        CurrentGroupBy ??= (CollectionView)CollectionViewSource.GetDefaultView(FileListView);
        CurrentGroupBy.GroupDescriptions.Clear();
        switch (newGroup)
        {
            case GroupBy.NAME:
                CurrentGroupBy.GroupDescriptions.Add(new PropertyGroupDescription(nameof(OneFileSystem.NameFirstLetter)));
                break;
            case GroupBy.SIZE:
                CurrentGroupBy.GroupDescriptions.Add(new PropertyGroupDescription(nameof(OneFileSystem.Size)));
                break;
            case GroupBy.TYPE:
                CurrentGroupBy.GroupDescriptions.Add(new PropertyGroupDescription(nameof(OneFileSystem.TypeName)));
                break;
            case GroupBy.LAST_MODIFIED:
                CurrentGroupBy.GroupDescriptions.Add(new PropertyGroupDescription(nameof(OneFileSystem.LastModified)));
                break;
        }
        CurrentGroup = newGroup;
        CurrentGroupBy.Refresh();
    }

    public void ChangeOrderBy(OrderBy newOrderBy)
    {
        CurrentOrderBy = newOrderBy;
        CurrentGroupBy ??= (CollectionView)CollectionViewSource.GetDefaultView(FileListView);
        CurrentGroupBy.SortDescriptions.Clear();
        switch (newOrderBy)
        {
            case OrderBy.NAME:
                CurrentGroupBy.SortDescriptions.Add(new SortDescription(nameof(OneFileSystem.NameFirstLetter), CurrentOrderDirection == OrderDirection.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending));
                break;
            case OrderBy.SIZE:
                CurrentGroupBy.SortDescriptions.Add(new SortDescription(nameof(OneFileSystem.Size), CurrentOrderDirection == OrderDirection.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending));
                break;
            case OrderBy.TYPE:
                CurrentGroupBy.SortDescriptions.Add(new SortDescription(nameof(OneFileSystem.TypeName), CurrentOrderDirection == OrderDirection.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending));
                break;
            case OrderBy.LAST_MODIFIED:
                CurrentGroupBy.SortDescriptions.Add(new SortDescription(nameof(OneFileSystem.LastModified), CurrentOrderDirection == OrderDirection.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending));
                break;
        }
        CurrentOrderBy = newOrderBy;
        CurrentGroupBy.Refresh();
    }

    public void ChangeOrderByDirection(OrderDirection newOrderDirection)
    {
        CurrentOrderDirection = newOrderDirection;
        ChangeOrderBy(CurrentOrderBy);
    }

    public GridLength IconSizePx
    {
        get
        {
            if (ViewDetails)
                return new GridLength(16, GridUnitType.Pixel);
            return CurrentIconSize switch
            {
                IconSize.Large => new GridLength(32, GridUnitType.Pixel),
                IconSize.Medium => new GridLength(24, GridUnitType.Pixel),
                IconSize.ExtraLarge => new GridLength(48, GridUnitType.Pixel),
                IconSize.Jumbo => new GridLength(96, GridUnitType.Pixel),
                _ => new GridLength(16, GridUnitType.Pixel),
            };
        }
    }

    public double NameSizePx
    {
        get { return IconSizePx.Value * 4; }
    }

    #endregion

    #region Rename file/folder

    public void SetCurrentlyRenaming(OneFileSystem? fs)
    {
        _currentlyRenaming = fs;
    }

    public OneFileSystem? CurrentlyRenaming
    {
        get { return _currentlyRenaming; }
    }

    public void RenameMode()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IInputElement o = FocusManager.GetFocusedElement(_control);
            if ((o is ListViewItem || o is ListView) && SelectedItems.Count > 0)
                SelectedItems[0].EditMode(true);
            else if (o is TreeViewItem || o is TreeView)
                SelectedFolder?.EditMode(true);
        });
    }

    #endregion

    public OneFileSystem[]? GetCurrentSelection()
    {
        IInputElement o = FocusManager.GetFocusedElement(_control);
        if (o is ListViewItem)
            return [.. SelectedItems];
        else if (o is TreeViewItem && SelectedFolder != null)
            return [SelectedFolder];
        return null;
    }

    public async Task ForceRefresh()
    {
        SelectedFolder?.Refresh();
        CurrentGroupBy = (CollectionView)CollectionViewSource.GetDefaultView(FileListView);
        ChangeGroupBy(CurrentGroup);
        ChangeOrderBy(CurrentOrderBy);
        await Task.Run(async () =>
        {
            await Task.Delay(Constants.DelayBeforeForceRefreshItems);
            CurrentControl.ForceRefreshVisibleItems();
        });
    }

    public string NumberOfFiles
    {
        get { return Explorip.Constants.Localization.NUMBER_OF_ELEMENT.Replace("%s", (FileListView?.Count ?? 0).ToString()) ?? ""; }
    }

    public string NumberOfSelectedItems
    {
        get { return ", " + Explorip.Constants.Localization.NUMBER_OF_SELECTED_ELEMENT.Replace("%s", SelectedItems.Count.ToString()); }
    }

    [RelayCommand()]
    public async Task KeyUp(KeyEventArgs e)
    {
        IInputElement o = FocusManager.GetFocusedElement(_control);
        if (e.Key == Key.Apps)
        {
            if (o is TreeView || o is TreeViewItem)
                SelectedFolder!.ContextMenuFolder();
            else if (o is ListView || o is ListViewItem || o is OneFileSystem)
                SelectedItems[0].ContextMenuFilesOrFolder();
            return;
        }
        if (e.Key == Key.Escape)
        {
            _currentlyDragging = false;
            if (CurrentControl.FileLV.DrawSelection)
                CurrentControl.FileLV.ResetCurrentSelection();
            _currentlyRenaming?.EditMode(false);
            ModeSearch = false;
            ModeEditPath = false;
            return;
        }
        if ((e.Key == Key.Enter || e.Key == Key.Return) && _currentlyRenaming != null)
        {
            _currentlyRenaming.Rename();
            return;
        }
        if (SelectedItems.Count > 0 && SelectedItems[0].RenameMode)
            return;
        if (SelectedFolder?.RenameMode == true || ModeEditPath || ModeSearch)
            return;
        if (e.Key == Key.F5)
        {
            await ForceRefresh();
            return;
        }
        if (e.Key == Key.F2)
        {
            RenameMode();
            return;
        }
        else if (e.Key == Key.Delete)
        {
            if (o is ListViewItem && SelectedItems.Count > 0)
            {
                Explorip.HookFileOperations.FilesOperations.FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
                foreach (OneFileSystem fs in SelectedItems)
                    fileOp.DeleteItem(fs.FullPath);
                fileOp.PerformOperations();
            }
            else if (o is TreeViewItem && SelectedFolder != null)
            {
                if (SelectedFolder.Drive != null || SelectedFolder.IsSpecialFolder)
                    return;
                Explorip.HookFileOperations.FilesOperations.FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
                fileOp.DeleteItem(SelectedFolder.FullPath);
                fileOp.PerformOperations();
                SelectedFolder = SelectedFolder.ParentDirectory;
                SelectedFolder!.Children.Clear();
                SelectedFolder!.LoadChildren();
            }
            return;
        }
        if (e.Key == Key.Back)
        {
            OneDirectory? parent = _control.FolderTV.SelectedItem as OneDirectory;
            if (parent?.ParentDirectory != null)
                BrowseTo(parent.ParentDirectory.FullPath);
        }
        else if ((e.Key == Key.C || e.Key == Key.X) && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
        {
            string[]? listItems = null;
            if (o is TreeView || o is TreeViewItem && SelectedFolder != null)
                listItems = [SelectedFolder!.FullPath];
            else if (SelectedItems.Count > 0)
                listItems = [.. SelectedItems.Select(i => i.FullPath)];
            if (listItems != null && listItems.Length > 0)
            {
                DataObject data = new();
                data.SetFileDropList([.. listItems]);
                if (e.Key == Key.X)
                    data.SetData("Preferred DropEffect", new byte[] { 2, 0, 0, 0 });
                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
            }
        }
        else if (e.Key == Key.V && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)) &&
            Clipboard.GetDataObject() is DataObject data && Clipboard.GetFileDropList()?.Count > 0 && SelectedFolder != null)
        {
            string[] listItems = (string[])data.GetData(DataFormats.FileDrop);
            FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
            fileOp.ChangeOperationFlags(EFileOperation.FOF_RENAMEONCOLLISION |
                EFileOperation.FOF_NOCONFIRMMKDIR |
                EFileOperation.FOFX_ADDUNDORECORD);
            bool move = false;
            if (Clipboard.GetData("Preferred DropEffect") != null)
            {
                try
                {
                    byte[] buffer = new byte[4];
                    if (await ((MemoryStream)Clipboard.GetData("Preferred DropEffect")).ReadAsync(buffer, 0, 4) > 0)
                    {
                        int dropEffect = BitConverter.ToInt32(buffer, 0);
                        move = dropEffect == 2;
                    }
                }
                catch (Exception) { /* Ignore errors */ }
            }
            foreach (string item in listItems)
                if (move)
                    fileOp.MoveItem(item, SelectedFolder!.FullPath, Path.GetFileName(item));
                else
                    fileOp.CopyItem(item, SelectedFolder!.FullPath, Path.GetFileName(item));
            fileOp.PerformOperations();
            fileOp.Dispose();
        }
    }

    public void ScrollToFirstItem()
    {
        _control.FileLV.FindVisualChild<ScrollViewer>()!.ScrollToHome();
        _control.FileLV.InvalidateVisual();
        _control.FileLV.InvalidateMeasure();
        _control.FileLV.InvalidateArrange();
    }

    #region Auto refresh by folder watcher

    private FileSystemWatcher? _fsWatcher;
    partial void OnSelectedFolderChanged(OneDirectory? value)
    {
        DisposeSearch();
        ChangePath?.Invoke(this, EventArgs.Empty);
        ErrorVisible = false;
        PleaseWait = false;

        if (_fsWatcher == null)
        {
            _fsWatcher = new FileSystemWatcher();
            _fsWatcher.Changed += FsWatcher_Changed;
            _fsWatcher.Created += FsWatcher_Created;
            _fsWatcher.Deleted += FsWatcher_Deleted;
            _fsWatcher.Renamed += FsWatcher_Renamed;
        }
        try
        {
            if (!string.IsNullOrWhiteSpace(SelectedFolder?.FullPath) && SelectedFolder?.FullPath.StartsWith("::") == false)
            {
                Directory.GetDirectories(SelectedFolder.FullPath, ".");
                _fsWatcher.Path = SelectedFolder.FullPath;
                _fsWatcher.EnableRaisingEvents = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorVisible = true;
            _fsWatcher.EnableRaisingEvents = false;
        }
        ScrollToFirstItem();
        CurrentGroupBy = (CollectionView)CollectionViewSource.GetDefaultView(FileListView);
    }

    public void ForceSelectedFolder()
    {
        OnSelectedFolderChanged(SelectedFolder);
    }

    private void FsWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        SelectedFolder!.RefreshListView();
    }

    private void FsWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        SelectedFolder!.RefreshListView();
    }

    private void FsWatcher_Created(object sender, FileSystemEventArgs e)
    {
        SelectedFolder!.RefreshListView();
    }

    private void FsWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        SelectedFolder!.RefreshListView();
    }

    #endregion
}
