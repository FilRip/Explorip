using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;

using ManagedShell.Common.Enums;
using ManagedShell.Interop;
using ManagedShell.ShellFolders;

using WpfToolkit.Controls;

namespace ExploripComponents;

public partial class WpfExplorerViewModel : ObservableObject
{
    #region Binding fields

    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _folderTreeView = [];
    [ObservableProperty()]
    private ObservableCollection<OneFileSystem> _fileListView = [];
    [ObservableProperty()]
    private OneDirectory? _selectedFolder;
    [ObservableProperty()]
    private ObservableCollection<OneFileSystem> _selectedItems = [];
    [ObservableProperty()]
    private IconSize _currentIconSize = IconSize.Small;
    [ObservableProperty()]
    private bool _viewDetails = false;
    [ObservableProperty()]
    private ICollectionView? _currentGroupBy;

    #endregion

    private readonly MainWindow _control;
    private OneFileSystem? _currentlyRenaming;
    private ItemsPanelTemplate? _itemTemplateDetails;
    private ItemsPanelTemplate? _itemTemplateWrap;

    #region Constructors

    public WpfExplorerViewModel(MainWindow control)
    {
        _control = control;
        ChangeDisplay();
        CurrentGroup = GroupBy.NONE;
    }

    [RelayCommand()]
    public void Refresh()
    {
        ExploripSharedCopy.Constants.Colors.LoadTheme();
        Explorip.Constants.Localization.LoadTranslation();
        FolderTreeView.Clear();
        OneDirectory myComputer = new(Environment.SpecialFolder.MyComputer, null, true) { MainViewModel = this, FullPath = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}" };
        FolderTreeView.Add(myComputer);
        myComputer.IsExpanded = true;

        // Add Network root
        string specialPath = "::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}";
        ShellItem si = new(specialPath);
        OneDirectory networkNeiborhood = new(specialPath, null, true, si.DisplayName) { MainViewModel = this, NetworkRoot = true, IsItemVisible = true };
        FolderTreeView.Add(networkNeiborhood);

        if (Environment.GetCommandLineArgs().Length > 1)
            BrowseTo(Environment.GetCommandLineArgs()[1]);
        else
            BrowseTo(null);

        ChangeIconSize(ViewDetails, CurrentIconSize);
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
    public void MouseUp()
    {
        CurrentlyDraging = false;
        CurrentControl.FileLV.DrawSelection = false;
    }

    #endregion

    #region Properties

    public MainWindow CurrentControl
    {
        get { return _control; }
    }

    #endregion

    public void BrowseTo(string? fullPath)
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
                string[] folders = fullPath!.Split(Path.DirectorySeparatorChar);
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

                        if (currentPath.ToLower() == folder.ToLower())
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
        if (SelectedItems.Count == 0 && !string.IsNullOrWhiteSpace(SelectedFolder?.FullPath))
        {
            if (SelectedFolder!.FullPath == Environment.SpecialFolder.Desktop.FullPath())
                new ShellContextMenu(this).ShowContextMenu("::{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
            else
                new ShellContextMenu(this).ShowContextMenu(SelectedFolder!.FullPath, Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
        }
    }

    #region Show Display/Group

    private void ChangeDisplay()
    {
        _itemTemplateDetails ??= new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
        _itemTemplateWrap ??= new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingWrapPanel)));
        CurrentControl.FileLV.ItemsPanel = (ViewDetails ? _itemTemplateDetails : _itemTemplateWrap);
    }

    public void ChangeIconSize(bool details, IconSize value)
    {
        ViewDetails = details;
        CurrentIconSize = value;
        SelectedFolder?.Refresh();
        ChangeDisplay();
        ChangeGroupBy(CurrentGroup);
    }

    public GroupBy CurrentGroup { get; set; }

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
        OnPropertyChanged(nameof(CurrentGroupBy));
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

    #endregion

    #region Rename file/folder

    public void SetCurrentlyRenaming(OneFileSystem? fs)
    {
        _currentlyRenaming = fs;
    }

    public void RenameMode()
    {
        IInputElement o = FocusManager.GetFocusedElement(_control);
        if (o is ListViewItem && SelectedItems.Count > 0)
            SelectedItems[0].EditMode(true);
        else if (o is TreeViewItem)
            SelectedFolder?.EditMode(true);
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
        await Task.Run(async () =>
        {
            await Task.Delay(500);
            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CurrentControl.ForceRefreshVisibleItems();
            });
        });
    }

    [RelayCommand()]
    public async Task KeyUp(KeyEventArgs e)
    {
        IInputElement o = FocusManager.GetFocusedElement(_control);
        if (e.Key == Key.Escape && _currentlyRenaming != null)
        {
            _currentlyRenaming.EditMode(false);
            return;
        }
        if ((e.Key == Key.Enter || e.Key == Key.Return) && _currentlyRenaming != null)
        {
            _currentlyRenaming.Rename();
            return;
        }
        if (SelectedItems.Count > 0 && SelectedItems[0].RenameMode)
            return;
        if (SelectedFolder?.RenameMode == true)
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
    }

    public void ScrollToFirstItem()
    {
        _control.FileLV.FindVisualChild<ScrollViewer>()!.ScrollToHome();
        _control.FileLV.InvalidateVisual();
    }

    #region Auto refresh by folder watcher

    private FileSystemWatcher? _fsWatcher;
    partial void OnSelectedFolderChanged(OneDirectory? value)
    {
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
            if (!string.IsNullOrWhiteSpace(SelectedFolder?.FullPath))
            {
                _fsWatcher.Path = SelectedFolder!.FullPath;
                _fsWatcher.EnableRaisingEvents = true;
            }
        }
        catch (Exception)
        {
            _fsWatcher.EnableRaisingEvents = false;
        }
        ScrollToFirstItem();

        CurrentGroupBy = (CollectionView)CollectionViewSource.GetDefaultView(FileListView);
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
