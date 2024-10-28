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
using ManagedShell.ShellFolders.Interfaces;

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

    public bool NetworkRoot { get; set; }

    private readonly ObservableCollection<OneFileSystem> _items;
    private Task? _taskCalculateSize;
    private CancellationTokenSource? _cancellationToken;
    private readonly Environment.SpecialFolder? _specialFolder;
    private readonly object _lockSize = new();

    public WpfExplorerViewModel? MainViewModel { get; set; }

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

    public bool HasDummyChild
    {
        get { return this.Children.Count == 1 && this.Children[0] == _dummyDir; }
    }

    partial void OnIsExpandedChanged(bool value)
    {
        if (IsExpanded && ParentDirectory != null)
            ParentDirectory.IsExpanded = true;

        if (this.HasDummyChild)
        {
            this.Children.Remove(_dummyDir);
            this.LoadChildren();
        }
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
            dir = new OneDirectory(newFullPath, this, hasSubFolder, directory);
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
            _cancellationToken?.Cancel();
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

    protected override void OnSelectIt()
    {
        RefreshListView();
    }

    protected override void DeSelectIt()
    {
        if (_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            _cancellationToken.Cancel();
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
                    _cancellationToken = new CancellationTokenSource();
                    _taskCalculateSize = new Task(() => CalculateFolderSize(), _cancellationToken.Token).ContinueWith(EndCalculationSize);
                    _taskCalculateSize.Start();
                }
            }
            return _lastSize!.Value;
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
        FastDirectoryEnumerator.FolderSize(FullPath, out ulong calculateSize, _cancellationToken!.Token);
        _lastSize = calculateSize;
        return Task.CompletedTask;
    }

    #endregion

    [RelayCommand()]
    public void ContextMenuBackgroundFolder()
    {
        // When right click on an item tree view
        ShellContextMenu scm = new()
        {
            Root = _specialFolder == Environment.SpecialFolder.MyComputer || _specialFolder == Environment.SpecialFolder.Desktop || FullPath?.StartsWith("::") == true,
        };
        DirectoryInfo dir;
        if (string.IsNullOrWhiteSpace(FullPath) || FullPath?.StartsWith("::") == true)
            dir = new DirectoryInfo(DisplayText);
        else
            dir = new DirectoryInfo(FullPath);
        scm.ShowContextMenu(dir, Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)), false);
    }

    public override void DoubleClickFile()
    {
        GetRootParent().MainViewModel!.BrowseTo(FullPath);
    }

    public override ImageSource? Icon
    {
        get
        {
            if (_icon == null && (_specialFolder != null || FullPath.StartsWith("::")))
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
                            _icon = IconImageConverter.GetDefaultIcon();
                        }
                    }
                    else
                    {
                        hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.Small, out hOverlay);
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
                        IntPtr hIcon = IconHelper.GetIconByPidl(pidl, ManagedShell.Common.Enums.IconSize.Small, out IntPtr hOverlay);
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
}
