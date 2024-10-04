using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploripComponents;

public partial class OneDirectory : OneFileSystem
{
    private static readonly OneDirectory _dummyDir = new("", null, false);
    private readonly OneDirectory _parent;
    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _children = [];
    [ObservableProperty()]
    private bool _isExpanded;
    private readonly ObservableCollection<OneFile> _files = [];
    private Task _taskCalculateSize;
    private CancellationTokenSource _cancellationToken;

    public ExplorerViewModel MainViewModel { get; set; }

    public OneDirectory(string fullPath, OneDirectory parent, bool hasSubFolder) : base(fullPath)
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
        FastDirectoryEnumerator.EnumerateFolderContent(FullPath, out List<string> listDirectories, out _);
        string newFullPath;
        foreach (string directory in listDirectories)
        {
            newFullPath = Path.Combine(FullPath, directory);
            try
            {
                FastDirectoryEnumerator.EnumerateFolderContent(newFullPath, out List<string> subDir, out _);
                hasSubFolder = subDir.Count > 0;
            }
            catch (Exception)
            {
                hasSubFolder = false;
            }
            dir = new OneDirectory(newFullPath, this, hasSubFolder);
            Children.Add(dir);
        }
    }

    protected override void RefreshListView()
    {
        _files.Clear();
        FastDirectoryEnumerator.EnumerateFolderContent(FullPath, out _, out List<string> listFiles);
        foreach (string file in listFiles)
        {
            _files.Add(new OneFile(Path.Combine(FullPath, file)));
        }
        _cancellationToken?.Cancel();
        GetRootParent().MainViewModel.FileListView = _files;
        GetRootParent().MainViewModel.SelectedFolder = FullPath;
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

    public override long Size
    {
        get
        {
            if (_lastSize == 0 && (_taskCalculateSize == null || _cancellationToken?.IsCancellationRequested == true))
            {
                _cancellationToken = new CancellationTokenSource();
                _taskCalculateSize = new Task(() => CalculateFolderSize(), _cancellationToken.Token).ContinueWith(EndCalculationSize);
                _taskCalculateSize.Start();
            }
            return _lastSize;
        }
    }

    private void EndCalculationSize(Task task)
    {
        _cancellationToken.Dispose();
    }

    public Task CalculateFolderSize()
    {
        _lastSize = RecursiveCalculateFolderSize(FullPath);
        return Task.CompletedTask;
    }

    private static long RecursiveCalculateFolderSize(string path)
    {
        long result = 0;
        foreach (string file in Directory.GetFiles(path))
            try
            {
                result += new FileInfo(file).Length;
            }
            catch (Exception) { /* Ignore access to file */ }
        foreach (string folder in Directory.GetDirectories(path))
            try
            {
                result += RecursiveCalculateFolderSize(folder);
            }
            catch (Exception) { /* Ignore access to file */ }
        return result;
    }
}
