using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using NativeMethods = ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public partial class OneDirectory : OneFileSystem
{
    private static readonly OneDirectory _dummyDir = new(null, "", false, "");

    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _children;
    [ObservableProperty()]
    private bool _isExpanded;

    public DriveInfo? Drive { get; set; }

    private readonly ObservableCollection<OneFileSystem> _items;
    private Task? _taskCalculateSize;
    private CancellationTokenSource? _cancellationToken;
    private readonly ShellObject? _shellObject;
    private readonly IKnownFolder? _knownFolder;
    private readonly object _lockSize = new();

    public WpfExplorerViewModel? MainViewModel { get; set; }

    private OneDirectory(OneDirectory? parent, string fullPath, bool hasSubFolder, string displayText) : base(fullPath, displayText, parent)
    {
        _children = [];
        _items = [];
        _knownFolder = null;
        if (hasSubFolder)
            _children.Add(_dummyDir);
    }

    public OneDirectory(string fullPath, OneDirectory? parent, bool hasSubFolder) : this(parent, fullPath, hasSubFolder, ShellObject.FromParsingName(fullPath).Name)
    {
        _shellObject = ShellObject.FromParsingName(fullPath);
    }

    public OneDirectory(IKnownFolder knownFolder, OneDirectory? parent, bool hasSubFolder) : this(parent, ((ShellObject)knownFolder).ParsingName, hasSubFolder, ((ShellObject)knownFolder).Name)
    {
        _knownFolder = knownFolder;
        _shellObject = (ShellObject)knownFolder;
    }

    public OneDirectory(DriveInfo drive, OneDirectory parent, bool hasSubFolder) : this(parent, drive.Name, hasSubFolder, ShellObject.FromParsingName(drive.RootDirectory.FullName).Name)
    {
        Drive = drive;
        _shellObject = ShellObject.FromParsingName(drive.Name);
    }

    public bool HasDummyChild
    {
        get { return this.Children.Count == 1 && this.Children[0] == _dummyDir; }
    }

    partial void OnIsExpandedChanged(bool value)
    {
        if (IsExpanded && _parentDirectory != null)
            _parentDirectory.IsExpanded = true;

        if (this.HasDummyChild)
        {
            this.Children.Remove(_dummyDir);
            this.LoadChildren();
        }
    }

    public OneDirectory? Parent
    {
        get { return _parentDirectory; }
    }

    private void LoadChildren()
    {
        OneDirectory dir;
        bool hasSubFolder;
        if (Drive != null && !IsSelected && !Drive.IsReady && Drive.DriveType != DriveType.Fixed)
            return;
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

    public void RefreshListView()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_knownFolder?.FolderId == KnownFolders.Computer.FolderId)
            {
                GetRootParent().MainViewModel!.FileListView = new ObservableCollection<OneFileSystem>(Children.OfType<OneFileSystem>());
                GetRootParent().MainViewModel!.SelectedFolder = this;
                return;
            }
            _items.Clear();
            FastDirectoryEnumerator.EnumerateFolderContent(FullPath, out List<string> listSubFolder, out List<string> listFiles);
            foreach (string subFolder in listSubFolder)
                _items.Add(new OneDirectory(Path.Combine(FullPath, subFolder), this, true));
            foreach (string file in listFiles)
                _items.Add(new OneFile(Path.Combine(FullPath, file), this));
            _cancellationToken?.Cancel();
            GetRootParent().MainViewModel!.FileListView = _items;
            GetRootParent().MainViewModel!.SelectedFolder = this;
        });
    }

    public OneDirectory GetRootParent()
    {
        OneDirectory curDir = this;
        while (curDir.Parent != null)
        {
            curDir = curDir.Parent;
        }
        return curDir;
    }

    public override ulong Size
    {
        get
        {
            lock (_lockSize)
            {
                if (_lastSize == null && (_taskCalculateSize == null || _cancellationToken?.IsCancellationRequested == true))
                {
                    _cancellationToken = new CancellationTokenSource();
                    _taskCalculateSize = new Task(() => CalculateFolderSize(), _cancellationToken.Token).ContinueWith(EndCalculationSize);
                    _taskCalculateSize.Start();
                }
            }
            return _lastSize!.Value;
        }
    }

    protected override void OnSelectIt()
    {
        RefreshListView();
    }

    protected override void DeSelectIt()
    {
        if (_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            _cancellationToken.Cancel();
    }

    private void EndCalculationSize(Task task)
    {
        _cancellationToken?.Dispose();
        _cancellationToken = null;
        OnPropertyChanged(nameof(Size));
    }

    public Task CalculateFolderSize()
    {
        FastDirectoryEnumerator.FolderSize(FullPath, out ulong calculateSize, _cancellationToken!.Token);
        _lastSize = calculateSize;
        return Task.CompletedTask;
    }

    public IKnownFolder? KnownFolder
    {
        get { return _knownFolder; }
    }

    public ShellObject? ShellObject
    {
        get { return _shellObject; }
    }

    [RelayCommand()]
    public void ContextMenuBackgroundFolder()
    {
        // When right click on an item tree view
        ShellContextMenu scm = new()
        {
            Root = _knownFolder?.FolderId == KnownFolders.Computer.FolderId || _knownFolder?.FolderId == KnownFolders.Desktop.FolderId,
        };
        scm.ShowContextMenu(new DirectoryInfo(FullPath), Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)), false);
    }

    public override void DoubleClickFile()
    {
        GetRootParent().MainViewModel!.BrowseTo(this);
    }

    public override ImageSource? Icon
    {
        get
        {
            if (_icon == null && _knownFolder != null)
            {
                IntPtr ptr = IntPtr.Zero;
                if (_knownFolder.FolderId == KnownFolders.Computer.FolderId)
                {
                    int hResult = NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_DRIVES, ref ptr);
                    if (hResult == (int)HResult.Ok)
                    {
                        NativeMethods.ShFileInfo result = new();
                        NativeMethods.SHGetFileInfo(ptr, NativeMethods.FILE_ATTRIBUTE.NULL, ref result, (uint)Marshal.SizeOf(result), NativeMethods.SHGFI.PIDL | NativeMethods.SHGFI.Icon | NativeMethods.SHGFI.SmallIcon);
                        _icon = Imaging.CreateBitmapSourceFromHIcon(result.hIcon, new Int32Rect(0, 0, 16, 16), System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        NativeMethods.DestroyIcon(result.hIcon);
                    }
                }
            }
            return base.Icon;
        }
    }

    public override void Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            List<string> filesAndFolders = ((DataObject)e.Data).GetFileDropList().OfType<string>().ToList();
            foreach (string fs in filesAndFolders)
                File.Copy(fs, Path.Combine(FullPath, Path.GetFileName(fs)));
        }
        base.Drop(sender, e);
    }

    [RelayCommand()]
    public void MouseMoveTreeView()
    {
        if (_parentDirectory == null)
            return;

        if ((Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed) &&
            !_parentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging)
        {
            _parentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging = true;
            DataObject data = new();
            data.SetFileDropList([FullPath]);
            DragDrop.DoDragDrop(_parentDirectory.GetRootParent().MainViewModel!.CurrentControl, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }
    }
}
