﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Interop;
using ManagedShell.ShellFolders;

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

    #endregion

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
        OneDirectory parent = new(Environment.SpecialFolder.MyComputer, null, true) { MainViewModel = this };
        FolderTreeView.Add(parent);
        parent.Children.Clear();

        // Add special folder
        void AddChild(Environment.SpecialFolder folder)
        {
            OneDirectory child = new(folder, null, true) { MainViewModel = this, IsItemVisible = true };
            parent.Children.Add(child);
        }
        AddChild(Environment.SpecialFolder.Desktop);
        AddChild(Environment.SpecialFolder.MyDocuments);
        AddChild(Environment.SpecialFolder.MyPictures);
        AddChild(Environment.SpecialFolder.MyMusic);
        AddChild(Environment.SpecialFolder.MyVideos);
        // Add special folder "Downloads"
        Guid guid = new("374DE290-123F-4565-9164-39C4925E467B");
        NativeMethods.SHGetKnownFolderPath(ref guid, NativeMethods.KnownFolder.None, IntPtr.Zero, out string pathDownload);
        NativeMethods.ShFileInfo fi = new();
        NativeMethods.SHGetFileInfo(pathDownload, NativeMethods.FILE_ATTRIBUTE.NORMAL | NativeMethods.FILE_ATTRIBUTE.DIRECTORY, ref fi, (uint)Marshal.SizeOf(fi), NativeMethods.SHGFI.DisplayName);
        parent.Children.Add(new OneDirectory(pathDownload, parent, true, fi.szDisplayName) { IsItemVisible = true });

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
            dir = new OneDirectory(di, parent, hasSubFolder, new ShellItem(di.RootDirectory.FullName).DisplayName) { IsItemVisible = true };
            parent.Children.Add(dir);
        }

        SelectedFolder = parent;

        // Add Network root
        string specialPath = "::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}";
        ShellItem si = new(specialPath);
        parent = new OneDirectory(specialPath, null, true, si.DisplayName) { MainViewModel = this, NetworkRoot = true, IsItemVisible = true };
        FolderTreeView.Add(parent);
    }

    public void BrowseTo(string fullPath)
    {
        string[] folders = fullPath.Split(Path.DirectorySeparatorChar);
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

    public void ScrollToTop()
    {
        ((MainWindow)_control).FileLV.FindVisualChild<ScrollViewer>()!.ScrollToTop();
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
            _fsWatcher.Path = SelectedFolder!.FullPath;
            _fsWatcher.EnableRaisingEvents = true;
        }
        catch (Exception)
        {
            _fsWatcher.EnableRaisingEvents = false;
            _fsWatcher.Path = "";
        }
        ScrollToTop();
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
