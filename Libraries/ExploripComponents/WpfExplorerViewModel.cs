﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

namespace ExploripComponents;

public partial class WpfExplorerViewModel(IntPtr handle) : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _folderTreeView = [];
    [ObservableProperty()]
    private ObservableCollection<OneFileSystem> _fileListView = [];
    [ObservableProperty()]
    private OneDirectory? _selectedFolder;
    [ObservableProperty()]
    private ObservableCollection<OneFileSystem> _selectedItems = [];

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

        foreach (DriveInfo di in DriveInfo.GetDrives())
        {
            try
            {
                FastDirectoryEnumerator.EnumerateFolderContent(di.RootDirectory.FullName, out List<string> subDir, out _);
                hasSubFolder = subDir.Count > 0;
            }
            catch (Exception)
            {
                hasSubFolder = false;
            }
            dir = new OneDirectory(di.RootDirectory.FullName, parent, hasSubFolder)
            {
                DriveInfo = di,
            };
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
                if (subFolder.DriveInfo is null)
                    Path.GetFileName(currentPath);

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
}
