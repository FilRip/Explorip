using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

namespace ExploripComponents;

public partial class WpfExplorerViewModel(IntPtr handle, Control control) : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _folderTreeView = [];
    [ObservableProperty()]
    private ObservableCollection<OneFileSystem> _fileListView = [];
    [ObservableProperty()]
    private OneDirectory? _selectedFolder;
    [ObservableProperty()]
    private ObservableCollection<OneFileSystem> _selectedItems = [];
    private readonly Control _control = control;

    public bool CurrentlyDraging { get; set; } = false;

    public Control CurrentControl
    {
        get { return _control; }
    }

    public IntPtr WindowHandle { get; private set; } = handle;

    [RelayCommand()]
    public void Refresh()
    {
        FolderTreeView.Clear();
        OneDirectory dir;
        bool hasSubFolder;
        OneDirectory parent = new(KnownFolders.Computer, null, true) { MainViewModel = this };
        FolderTreeView.Add(parent);
        parent.Children.Clear();

        // Add special folder
        void AddChild(IKnownFolder folder)
        {
            OneDirectory child = new(folder, null, true) { MainViewModel = this };
            parent.Children.Add(child);
        }
        AddChild(KnownFolders.Desktop);
        AddChild(KnownFolders.Documents);
        AddChild(KnownFolders.Pictures);
        AddChild(KnownFolders.Music);
        AddChild(KnownFolders.Downloads);
        AddChild(KnownFolders.Videos);
        AddChild(KnownFolders.Objects3D);

        foreach (DriveInfo di in DriveInfo.GetDrives())
        {
            try
            {
                if (di.DriveType == DriveType.Fixed)
                {
                    FastDirectoryEnumerator.EnumerateFolderContent(di.RootDirectory.FullName, out List<string> subDir, out _);
                    hasSubFolder = subDir.Count > 0;
                }
                else
                    hasSubFolder = true;
            }
            catch (Exception)
            {
                hasSubFolder = false;
            }
            dir = new OneDirectory(di, parent, hasSubFolder);
            parent.Children.Add(dir);
        }

        SelectedFolder = parent;
    }

    public void BrowseTo(OneDirectory value)
    {
        string[] folders = value.FullPath.Split(Path.DirectorySeparatorChar);
        OneDirectory actualFolder = FolderTreeView[0];
        string currentPath;
        foreach (string folder in folders)
        {
            foreach (OneDirectory subFolder in actualFolder.Children)
            {
                currentPath = subFolder.FullPath.TrimEnd(Path.DirectorySeparatorChar);
                if (subFolder.Drive == null)
                    currentPath = Path.GetFileName(currentPath);

                if (currentPath == folder)
                {
                    subFolder.IsExpanded = true;
                    actualFolder = subFolder;
                    break;
                }
            }
        }
        actualFolder.IsSelected = true;
    }

    [RelayCommand()]
    public void ContextMenuBackgroundFolder()
    {
        // When right click on empty space in list view
        if (SelectedItems.Count == 0)
            new ShellContextMenu().ShowContextMenu(new DirectoryInfo(SelectedFolder!.FullPath), Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
    }

    [RelayCommand()]
    public void MouseUp()
    {
        CurrentlyDraging = false;
    }

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
            _fsWatcher.Path = SelectedFolder!.FullPath;
            _fsWatcher.EnableRaisingEvents = true;
        }
        catch (Exception)
        {
            _fsWatcher.EnableRaisingEvents = false;
            _fsWatcher.Path = "";
        }
        Debug.WriteLine($"Create Watcher of {_fsWatcher.Path}");
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
}
