using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploripComponents;

public partial class ExplorerViewModel : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _folderTreeView;
    [ObservableProperty()]
    private ObservableCollection<OneFile> _fileListView;

    public ExplorerViewModel()
    {
    }

    public void Refresh()
    {
        List<OneDirectory> list = [];
        OneDirectory dir;
        OneDirectory parent = new(null, true) { FullPath = "C:\\", MainViewModel = this };
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
            dir = new OneDirectory(parent, hasSubFolder)
            {
                FullPath = directory,
            };
            list.Add(dir);
        }
        FolderTreeView = new ObservableCollection<OneDirectory>(list);
    }
}
