using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ExploripComponents;

public partial class ExplorerViewModel(IntPtr handle) : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _folderTreeView = [];
    [ObservableProperty()]
    private ObservableCollection<OneFile> _fileListView = [];

    public IntPtr WindowHandle { get; private set; } = handle;

    [RelayCommand()]
    public void Refresh()
    {
        FolderTreeView.Clear();
        OneDirectory dir;
        OneDirectory parent = new("C:\\", null, true) { MainViewModel = this };
        bool hasSubFolder;
        foreach (string directory in Directory.GetDirectories(parent.FullPath))
        {
            try
            {
                hasSubFolder = Directory.GetDirectories(directory).Length > 0;
            }
            catch (Exception)
            {
                hasSubFolder = false;
            }
            dir = new OneDirectory(directory, parent, hasSubFolder);
            FolderTreeView.Add(dir);
            SelectedFolder = parent.FullPath;
        }
    }

    public string SelectedFolder { get; set; }

    [RelayCommand()]
    public void ContextMenu()
    {
        new ShellContextMenu().ShowContextMenu(new DirectoryInfo(SelectedFolder), Mouse.GetPosition(null));
    }
}
