﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Interfaces;

using NativeMethods = ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public partial class OneDirectory : OneFileSystem
{
    #region Fields

    private static readonly OneDirectory _dummyDir = new(null, "", false, "");

    #region Binding properties

    [ObservableProperty()]
    private ObservableCollection<OneDirectory> _children;
    [ObservableProperty()]
    private bool _isExpanded;

    #endregion

    private readonly ObservableCollection<OneFileSystem> _items;
    private Task? _taskCalculateSize;
    private CancellationTokenSource? _cancellationToken;
    private readonly Environment.SpecialFolder? _specialFolder;
    private readonly object _lockSize = new();
    private ImageSource? _treeViewIcon;

    #endregion

    #region Constructor

    private OneDirectory(OneDirectory? parent, string fullPath, bool hasSubFolder, string displayText) : base(fullPath, displayText, parent)
    {
        _children = [];
        _items = [];
        _specialFolder = null;
        if (hasSubFolder)
            _children.Add(_dummyDir);
    }

    public OneDirectory(string fullPath, OneDirectory? parent, bool hasSubFolder, string displayText) : this(parent, fullPath, hasSubFolder, displayText)
    {
    }

    public OneDirectory(Environment.SpecialFolder knownFolder, OneDirectory? parent, bool hasSubFolder) : this(parent, Environment.GetFolderPath(knownFolder), hasSubFolder, knownFolder.RealName())
    {
        _specialFolder = knownFolder;
    }

    public OneDirectory(DriveInfo drive, OneDirectory parent, bool hasSubFolder, string displayText) : this(parent, drive.Name, hasSubFolder, displayText)
    {
        Drive = drive;
    }

    #endregion

    public void LoadChildren()
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
            dir = new OneDirectory(newFullPath, this, hasSubFolder, directory) { IsItemVisible = true };
            Children.Add(dir);
        }
    }

    public void RefreshListView()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _items.Clear();
            if (_specialFolder == Environment.SpecialFolder.MyComputer)
            {
                foreach (OneFileSystem fs in Children.OfType<OneFileSystem>())
                    _items.Add(fs);
            }
            else if (NetworkRoot)
            {
                IShellFolder? networkShellFolder = null;
                Guid guidSf = typeof(IShellFolder).GUID;

                // Get IShellFolder from WinApi
                NativeMethods.SHGetDesktopFolder(out IntPtr pidl);
                IShellFolder desktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidl, typeof(IShellFolder));
                NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_NETWORK, ref pidl);
                desktopFolder.BindToObject(pidl, IntPtr.Zero, ref guidSf, out IntPtr ptrsh);
                networkShellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrsh, typeof(IShellFolder));

                // Get IShellFolder from ManagedShell
                /*ShellFolder networkFolder = new(FullPath, IntPtr.Zero);
                networkShellFolder = networkFolder.ShellFolderInterface;*/

                if (networkShellFolder != null)
                {
                    networkShellFolder.EnumObjects(IntPtr.Zero, ManagedShell.ShellFolders.Enums.SHCONTF.FOLDERS, out IntPtr data);
                    IEnumIDList itemsEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(data, typeof(IEnumIDList));
                    while (itemsEnum.Next(1, out IntPtr subItemPtr, out uint fetch) == 0 && fetch == 1)
                    {
                        if (networkShellFolder.BindToObject(subItemPtr, IntPtr.Zero, ref guidSf, out IntPtr siPtr) == 0)
                        {
                            IShellFolder subSf = (IShellFolder)Marshal.GetTypedObjectForIUnknown(siPtr, typeof(IShellFolder));
                            IntPtr namePtr = IntPtr.Zero;
                            subSf.GetDisplayNameOf(subItemPtr, ManagedShell.ShellFolders.Enums.SHGDN.NORMAL, namePtr);
                            string displayName = Marshal.PtrToStringAuto(namePtr);
                            Marshal.FreeCoTaskMem(namePtr);
                            _items.Add(new OneFile(displayName, this));
                        }
                    }
                }
            }
            else
            {
                FastDirectoryEnumerator.EnumerateFolderContent(FullPath, out List<string> listSubFolder, out List<string> listFiles);
                foreach (string subFolder in listSubFolder)
                    _items.Add(new OneDirectory(Path.Combine(FullPath, subFolder), this, true, subFolder));
                foreach (string file in listFiles)
                    _items.Add(new OneFile(Path.Combine(FullPath, file), this));
            }
            CancelCalculateSize();
            GetRootParent().MainViewModel!.FileListView = _items;
            GetRootParent().MainViewModel!.SelectedFolder = this;
        });
    }

    public OneDirectory GetRootParent()
    {
        OneDirectory curDir = this;
        while (curDir.ParentDirectory != null)
        {
            curDir = curDir.ParentDirectory;
        }
        return curDir;
    }

    public void CancelCalculateSize()
    {
        if (_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            _cancellationToken.Cancel();
    }

    #region Events

    partial void OnIsExpandedChanged(bool value)
    {
        if (IsExpanded && ParentDirectory != null)
            ParentDirectory.IsExpanded = true;

        if (HasDummyChild)
        {
            Children.Remove(_dummyDir);
            LoadChildren();
        }
    }

    protected override void OnSelectIt()
    {
        RefreshListView();
    }

    protected override void DeSelectIt()
    {
        if (_items?.Count > 0)
            foreach (OneDirectory dir in _items.OfType<OneDirectory>())
                dir.CancelCalculateSize();
    }

    #endregion

    public void Refresh()
    {
        DeSelectIt();
        if (IsExpanded)
        {
            Children.Clear();
            if (_specialFolder == Environment.SpecialFolder.MyComputer)
            {
                OneDirectory dir;
                bool hasSubFolder;

                void AddChild(Environment.SpecialFolder folder)
                {
                    OneDirectory child = new(folder, this, true) { IsItemVisible = true };
                    Children.Add(child);
                }
                // Add special folder
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
                Children.Add(new OneDirectory(pathDownload, this, true, fi.szDisplayName) { IsItemVisible = true });

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
                    dir = new OneDirectory(di, this, hasSubFolder, new ShellItem(di.RootDirectory.FullName).DisplayName) { IsItemVisible = true };
                    Children.Add(dir);
                }
            }
            else
            {
                LoadChildren();
            }
        }
        RefreshListView();
        GetRootParent().MainViewModel!.ScrollToTop();
    }

    public override void EditMode(bool activate)
    {
        if (Drive != null || _specialFolder != null)
            return;
        base.EditMode(activate);
    }

    public override void Rename()
    {
        Directory.Move(FullPath, Path.Combine(Path.GetDirectoryName(FullPath), NewName));
        DisplayText = NewName;
        base.Rename();
        if (ParentDirectory?.IsExpanded == true)
        {
            ParentDirectory.Children.Clear();
            ParentDirectory.LoadChildren();
        }
    }

    #region Size folder

    public override ulong Size
    {
        get
        {
            lock (_lockSize)
            {
                if (_lastSize == null && (_taskCalculateSize == null || _cancellationToken?.IsCancellationRequested == true))
                {
                    if (Drive == null)
                    {
                        _cancellationToken = new CancellationTokenSource();
                        _taskCalculateSize = new Task(() => CalculateFolderSize(), _cancellationToken.Token);
                        _taskCalculateSize.ContinueWith(EndCalculationSize);
                        _taskCalculateSize.Start();
                    }
                    else
                    {
                        _lastSize = (ulong)Drive.TotalSize - (ulong)Drive.TotalFreeSpace;
                    }
                }
            }
            return _lastSize ?? 0;
        }
    }

    private void EndCalculationSize(Task task)
    {
        _cancellationToken?.Dispose();
        _cancellationToken = null;
        OnPropertyChanged(nameof(Size));
    }

    public Task CalculateFolderSize()
    {
        ulong calculateSize = 0;
        FastDirectoryEnumerator.FolderSize(FullPath, ref calculateSize, _cancellationToken!.Token);
        _lastSize = calculateSize;
        return Task.CompletedTask;
    }

    #endregion

    #region Relay commands

    [RelayCommand()]
    public void ContextMenuFolder()
    {
        // When right click on an item tree view
        ShellContextMenu scm = new(GetRootParent().MainViewModel!)
        {
            Root = _specialFolder == Environment.SpecialFolder.MyComputer || _specialFolder == Environment.SpecialFolder.Desktop || FullPath?.StartsWith("::") == true,
        };
        DirectoryInfo dir;
        if (string.IsNullOrWhiteSpace(FullPath) || FullPath?.StartsWith("::") == true)
            dir = new DirectoryInfo(DisplayText);
        else
            dir = new DirectoryInfo(FullPath);
        scm.ShowContextMenu(dir, Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
    }

    public override void DoubleClickFile()
    {
        GetRootParent().MainViewModel!.BrowseTo(FullPath);
    }

    #endregion

    #region Properties

    public bool IsSpecialFolder
    {
        get { return _specialFolder != null; }
    }

    private bool HasDummyChild
    {
        get { return Children.Count == 1 && Children[0] == _dummyDir; }
    }

    public DriveInfo? Drive { get; set; }

    public bool NetworkRoot { get; set; }

    public WpfExplorerViewModel? MainViewModel { get; set; }

    public override ImageSource? Icon
    {
        get
        {
            if (_icon == null && IsItemVisible && (_specialFolder != null || FullPath.StartsWith("::")))
            {
                WpfExplorerViewModel vm = GetRootParent().MainViewModel!;
                if (!string.IsNullOrWhiteSpace(FullPath))
                {
                    IntPtr hIcon = IntPtr.Zero, hOverlay = IntPtr.Zero;
                    if (FullPath.StartsWith("::"))
                    {
                        IntPtr pidl = NativeMethods.ILCreateFromPath(FullPath);
                        if (pidl != IntPtr.Zero)
                        {
                            hIcon = IconHelper.GetIconByPidl(pidl, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out hOverlay);
                            NativeMethods.ILFree(pidl);
                        }
                        else
                        {
                            _icon = IconImageConverter.GetDefaultIcon();
                        }
                    }
                    else
                    {
                        hIcon = IconHelper.GetIconByFilename(FullPath, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out hOverlay);
                    }
                    if (hIcon != IntPtr.Zero)
                    {
                        _icon = IconImageConverter.GetImageFromHIcon(hIcon);
                        if (hOverlay != IntPtr.Zero)
                            IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
                    }
                    if (_icon == null)
                    {
                        System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(FullPath);
                        if (icon != null)
                        {
                            _icon = IconManager.Convert(icon);
                            icon.Dispose();
                        }
                    }
                }
                else if (_specialFolder == Environment.SpecialFolder.MyComputer)
                {
                    IntPtr pidl = IntPtr.Zero;
                    NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_DRIVES, ref pidl);
                    if (pidl != IntPtr.Zero)
                    {
                        IntPtr hIcon = IconHelper.GetIconByPidl(pidl, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out IntPtr hOverlay);
                        NativeMethods.ILFree(pidl);
                        if (hIcon != IntPtr.Zero)
                        {
                            _icon = IconImageConverter.GetImageFromHIcon(hIcon);
                            if (hOverlay != IntPtr.Zero)
                                IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
                        }
                    }
                }
            }
            return base.Icon;
        }
    }

    public ImageSource? TreeViewIcon
    {
        get
        {
            if (_treeViewIcon == null && (_specialFolder != null || FullPath.StartsWith("::")))
            {
                if (!string.IsNullOrWhiteSpace(FullPath))
                {
                    IntPtr hIcon = IntPtr.Zero, hOverlay = IntPtr.Zero;
                    if (FullPath.StartsWith("::"))
                    {
                        IntPtr pidl = NativeMethods.ILCreateFromPath(FullPath);
                        if (pidl != IntPtr.Zero)
                        {
                            hIcon = IconHelper.GetIconByPidl(pidl, ManagedShell.Common.Enums.IconSize.Small, out hOverlay);
                            NativeMethods.ILFree(pidl);
                        }
                        else
                        {
                            _treeViewIcon = IconImageConverter.GetDefaultIcon();
                        }
                    }
                    else
                    {
                        hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.Small, out hOverlay);
                    }
                    if (hIcon != IntPtr.Zero)
                    {
                        _treeViewIcon = IconImageConverter.GetImageFromHIcon(hIcon);
                        if (hOverlay != IntPtr.Zero)
                            IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
                    }
                    if (_treeViewIcon == null)
                    {
                        System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(FullPath);
                        if (icon != null)
                        {
                            _treeViewIcon = IconManager.Convert(icon);
                            icon.Dispose();
                        }
                    }
                }
                else if (_specialFolder == Environment.SpecialFolder.MyComputer)
                {
                    IntPtr pidl = IntPtr.Zero;
                    NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_DRIVES, ref pidl);
                    if (pidl != IntPtr.Zero)
                    {
                        IntPtr hIcon = IconHelper.GetIconByPidl(pidl, ManagedShell.Common.Enums.IconSize.Small, out IntPtr hOverlay);
                        NativeMethods.ILFree(pidl);
                        if (hIcon != IntPtr.Zero)
                        {
                            _treeViewIcon = IconImageConverter.GetImageFromHIcon(hIcon);
                            if (hOverlay != IntPtr.Zero)
                                IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
                        }
                    }
                }
            }
            if (_treeViewIcon == null && IsItemVisible && !string.IsNullOrWhiteSpace(FullPath))
            {
                IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.Small, out IntPtr hOverlay);
                _treeViewIcon = IconImageConverter.GetImageFromHIcon(hIcon);
                if (hOverlay != IntPtr.Zero)
                    IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
            }
            return _treeViewIcon;
        }
    }

    public override bool Hidden
    {
        get
        {
            if (Drive != null || NetworkRoot)
                return false;
            return base.Hidden;
        }
    }

    #endregion

    #region Drag'n Drop

    public override void Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && GetRootParent().MainViewModel!.NbMillisecondsStartDragging > Constants.DelayIgnoreDrag)
        {
            List<string> filesAndFolders = ((DataObject)e.Data).GetFileDropList().OfType<string>().ToList();
            if (GetRootParent().MainViewModel!.DragDropKeyStates.HasFlag(DragDropKeyStates.LeftMouseButton))
            {
                if (filesAndFolders[0] != FullPath)
                    foreach (string fs in filesAndFolders)
                        File.Copy(fs, Path.Combine(FullPath, Path.GetFileName(fs)));
            }
            else if (GetRootParent().MainViewModel!.DragDropKeyStates.HasFlag(DragDropKeyStates.RightMouseButton))
            {
                DragDropHelper.GetInstance().DragDrop(GetRootParent().MainViewModel!.DragDropKeyStates,
                                                      Application.Current.MainWindow.PointToScreen(e.GetPosition(Application.Current.MainWindow)),
                                                      FullPath,
                                                      filesAndFolders);
            }
        }
        base.Drop(sender, e);
    }

    [RelayCommand()]
    public void MouseMoveTreeView()
    {
        if (ParentDirectory == null)
            return;

        if ((Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed) &&
            !ParentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging)
        {
            ParentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging = true;
            DataObject data = new();
            data.SetFileDropList([FullPath]);
            DragDrop.DoDragDrop(ParentDirectory.GetRootParent().MainViewModel!.CurrentControl, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }
    }

    #endregion
}