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
        foreach (string directory in Directory.GetDirectories($"c:\\"))
        {
            dir = new OneDirectory()
            {
                Text = Path.GetFileName(directory),
            };
            try
            {
                dir.HasChildren = Directory.GetDirectories(directory).Length > 0;
            }
            catch (Exception) { }
            list.Add(dir);
        }
        FolderTreeView = new ObservableCollection<OneDirectory>(list);
    }
}
