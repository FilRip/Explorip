using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploripComponents;

public partial class OneDirectory : OneFileSystem
{
    private static readonly OneDirectory _dummyDir = new(null, false);
    private readonly OneDirectory _parent;
    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _children = [];
    [ObservableProperty()]
    private bool _isExpanded;
    private readonly ObservableCollection<OneFile> _files = [];
    [ObservableProperty()]
    private ImageSource _icon;
    [ObservableProperty()]
    private ImageSource _iconOverlay;

    public ExplorerViewModel MainViewModel { get; set; }

    public OneDirectory(OneDirectory parent, bool hasSubFolder)
    {
        _parent = parent;

        if (hasSubFolder)
            _children.Add(_dummyDir);
    }

    public bool HasDummyChild
    {
        get { return this.Children.Count == 1 && this.Children[0] == _dummyDir; }
    }

    partial void OnIsExpandedChanged(bool value)
    {
        if (IsExpanded && _parent != null)
            _parent.IsExpanded = true;

        if (this.HasDummyChild)
        {
            this.Children.Remove(_dummyDir);
            this.LoadChildren();
        }
    }

    public OneDirectory Parent
    {
        get { return _parent; }
    }

    private void LoadChildren()
    {
        OneDirectory dir;
        bool hasSubFolder;
        foreach (string directory in Directory.GetDirectories(FullPath))
        {
            try
            {
                hasSubFolder = Directory.GetDirectories(directory).Length > 0;
            }
            catch (Exception)
            {
                hasSubFolder = false;
            }
            dir = new OneDirectory(this, hasSubFolder)
            {
                FullPath = directory,
            };
            Children.Add(dir);
        }
    }

    protected override void RefreshFiles()
    {
        _files.Clear();
        foreach (string file in Directory.GetFiles(FullPath))
        {
            _files.Add(new OneFile() { FullPath = file });
        }
        GetRootParent().MainViewModel.FileListView = _files;
    }

    private OneDirectory GetRootParent()
    {
        OneDirectory curDir = this;
        while (curDir.Parent != null)
        {
            curDir = curDir.Parent;
        }
        return curDir;
    }
}
