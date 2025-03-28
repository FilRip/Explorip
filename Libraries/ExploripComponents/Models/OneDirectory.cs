﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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

using ExploripComponents.Helpers;
using ExploripComponents.ViewModels;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

using NativeMethods = ManagedShell.Interop.NativeMethods;

namespace ExploripComponents.Models;

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
    private readonly ObservableCollection<OneFileSystem> _networkItems;
    private Task? _taskCalculateSize;
    private CancellationTokenSource? _cancellationToken;
    private readonly Environment.SpecialFolder? _specialFolder;
    private readonly object _lockSize = new();
    private ImageSource? _treeViewIcon;
    private Task? _taskSearchNetwork;

    #endregion

    #region Constructor

    private OneDirectory(OneDirectory? parent, string fullPath, bool hasSubFolder, string displayText) : base(fullPath, displayText, parent)
    {
        _children = [];
        _items = [];
        _networkItems = [];
        _specialFolder = null;
        if (hasSubFolder)
            _children.Add(_dummyDir);
        if (FullPath?.Contains("$Recycle.Bin") == true && parent != null &&
            parent.GetRootParent().MainViewModel!.FullPathRecycledBin?.ToLower() == fullPath.ToLower())
        {
            DisplayText = parent.GetRootParent().MainViewModel!.LocalizedNameRecycleBin ?? DisplayText;
            _children.Clear();
            RecycledBin = true;
        }
    }

    public OneDirectory(string fullPath, OneDirectory? parent, bool hasSubFolder, string displayText) : this(parent, fullPath, hasSubFolder, displayText)
    {
    }

    public OneDirectory(Environment.SpecialFolder knownFolder, OneDirectory? parent, bool hasSubFolder) : this(parent, Environment.GetFolderPath(knownFolder), hasSubFolder, Explorip.Helpers.ExtensionsDirectory.RealName(knownFolder))
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
        ExtensionsDirectory.EnumerateFolderContent(FullPath, out List<string> listDirectories, out _);
        string newFullPath;
        foreach (string directory in listDirectories)
        {
            newFullPath = Path.Combine(FullPath, directory);
            try
            {
                ExtensionsDirectory.EnumerateFolderContent(newFullPath, out List<string> subDir, out _);
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
            WpfExplorerViewModel viewModel = GetRootParent().MainViewModel!;
            _items.Clear();
            if (_specialFolder == Environment.SpecialFolder.MyComputer)
            {
                foreach (OneFileSystem fs in Children.OfType<OneFileSystem>())
                    _items.Add(fs);
            }
            else if (NetworkRoot)
            {
                viewModel.SelectedFolder = this;
                viewModel.PleaseWait = true;
                if (_taskSearchNetwork == null || _taskSearchNetwork.Status != TaskStatus.Running)
                {
                    _taskSearchNetwork = new Task(() =>
                    {
                        try
                        {
                            Application.Current.Dispatcher.Invoke(() => _networkItems.Clear());
                            IShellFolder? networkShellFolder = null;
                            Guid guidSf = typeof(IShellFolder).GUID;

                            // Get IShellFolder from WinApi
                            NativeMethods.SHGetDesktopFolder(out IntPtr pidl);
                            IShellFolder desktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidl, typeof(IShellFolder));
                            Marshal.Release(pidl);
                            NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_NETWORK, ref pidl);
                            desktopFolder.BindToObject(pidl, IntPtr.Zero, ref guidSf, out IntPtr ptrsh);
                            Marshal.ReleaseComObject(desktopFolder);
                            networkShellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrsh, typeof(IShellFolder));
                            Marshal.Release(ptrsh);
                            if (networkShellFolder != null)
                            {
                                networkShellFolder.EnumObjects(IntPtr.Zero, SHCONTF.FOLDERS | SHCONTF.NONFOLDERS | SHCONTF.INCLUDEHIDDEN | SHCONTF.NETPRINTERSRCH, out IntPtr data);
                                IEnumIDList itemsEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(data, typeof(IEnumIDList));
                                while (itemsEnum.Next(1, out IntPtr subItemPtr, out uint fetch) == 0 && fetch == 1)
                                {
                                    IntPtr absolutePidl = NativeMethods.ILCombine(pidl, subItemPtr);
                                    string name = networkShellFolder.GetDisplayNameOf(subItemPtr, SHGDN.NORMAL);
                                    if (!string.IsNullOrWhiteSpace(name))
                                        Application.Current.Dispatcher.Invoke(() => _networkItems.Add(new OneFile(name, this, absolutePidl, subItemPtr)));

                                }
                                Marshal.ReleaseComObject(itemsEnum);
                            }

                            Marshal.Release(pidl);
                            Marshal.ReleaseComObject(networkShellFolder);
                        }
                        catch (Exception)
                        {
                            if (Debugger.IsAttached)
                                Debugger.Break();
                        }
                    });
                    _taskSearchNetwork.ContinueWith(SetNewListItems);
                    _taskSearchNetwork.Start();
                    viewModel.SelectedFolder = this;
                    return;
                }
                else
                    return;
            }
            else if (RecycledBin)
            {
                IShellFolder? sfDesktop = null;
                IShellFolder2? sfRecycledBin = null;
                IntPtr pidlDesktop = IntPtr.Zero, pidlRecycledBin = IntPtr.Zero, enumIDList = IntPtr.Zero;
                IEnumIDList? itemsEnum = null;
                try
                {
                    string recycledBinFullPath = GetRootParent().MainViewModel!.FullPathRecycledBin!;
                    NativeMethods.SHGetDesktopFolder(out pidlDesktop);
                    sfDesktop = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidlDesktop, typeof(IShellFolder));
                    NativeMethods.SHGetSpecialFolderLocation(IntPtr.Zero, NativeMethods.CSIDL.CSIDL_BITBUCKET, ref pidlRecycledBin);
                    Guid guidSF = typeof(IShellFolder2).GUID;
                    sfDesktop.BindToObject(pidlRecycledBin, IntPtr.Zero, ref guidSF, out IntPtr ptrRecycledBin);
                    sfRecycledBin = (IShellFolder2)Marshal.GetTypedObjectForIUnknown(ptrRecycledBin, typeof(IShellFolder2));
                    sfRecycledBin.EnumObjects(IntPtr.Zero, SHCONTF.FOLDERS | SHCONTF.NONFOLDERS | SHCONTF.INCLUDEHIDDEN, out enumIDList);
                    itemsEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(enumIDList, typeof(IEnumIDList));
                    while (itemsEnum.Next(1, out IntPtr pidlItem, out uint fetch) == 0 && fetch == 1)
                    {
                        sfRecycledBin.GetDetailsOf(pidlItem, (uint)RecycledBinColumnName.Name, out ShellDetails sd);
                        string displayName = Marshal.PtrToStringUni(sd.str.OleStr);
                        sd.str.Free();

                        sfRecycledBin.GetDetailsOf(pidlItem, (uint)RecycledBinColumnName.Size, out sd);
                        string sizeStr = Marshal.PtrToStringUni(sd.str.OleStr);
                        sd.str.Free();
                        double.TryParse(sizeStr.ConvertFromUniToAscii().RemoveNotDigitOrSeparator(), out double size);
                        for (int i = 0; i < ExtensionsDirectory.NumberOfMultiply(sizeStr); i++)
                            size *= 1024;

                        sfRecycledBin.GetDetailsOf(pidlItem, (uint)RecycledBinColumnName.DateTimeDeleted, out sd);
                        string dtDeleteStr = Marshal.PtrToStringUni(sd.str.OleStr);
                        sd.str.Free();
                        DateTime.TryParse(dtDeleteStr.ConvertFromUniToAscii(), CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime dtDeleted);

                        sfRecycledBin.GetDetailsOf(pidlItem, (uint)RecycledBinColumnName.DateTimeLastModified, out sd);
                        string dtLastWriteStr = Marshal.PtrToStringUni(sd.str.OleStr);
                        sd.str.Free();
                        DateTime.TryParse(dtLastWriteStr.ConvertFromUniToAscii(), CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime dtLastWrite);

                        sfRecycledBin.GetDetailsOf(pidlItem, 196, out sd);
                        string realName = Marshal.PtrToStringUni(sd.str.OleStr);
                        sd.str.Free();

                        realName = Path.Combine(recycledBinFullPath, Path.GetFileName(realName));

                        _items.Add(new OneFile(realName, dtDeleted, dtLastWrite, (ulong)size, this)
                        {
                            DisplayText = displayName,
                        });
                    }
                }
                catch (Exception)
                {
                    // Ignore errors for this time
                }
                finally
                {
                    if (enumIDList != IntPtr.Zero)
                        Marshal.Release(enumIDList);
                    if (pidlRecycledBin != IntPtr.Zero)
                        Marshal.Release(pidlRecycledBin);
                    if (pidlDesktop != IntPtr.Zero)
                        Marshal.Release(pidlDesktop);
                    if (sfDesktop != null)
                        Marshal.ReleaseComObject(sfDesktop);
                    if (sfRecycledBin != null)
                        Marshal.ReleaseComObject(sfRecycledBin);
                    if (itemsEnum != null)
                        Marshal.ReleaseComObject(itemsEnum);
                }
            }
            else
            {
                ExtensionsDirectory.EnumerateFolderContent(FullPath, out List<string> listSubFolder, out List<string> listFiles);
                foreach (string subFolder in listSubFolder)
                    _items.Add(new OneDirectory(Path.Combine(FullPath, subFolder), this, true, subFolder));
                foreach (string file in listFiles)
                    _items.Add(new OneFile(Path.Combine(FullPath, file), this));
            }
            CancelCalculateSize();
            viewModel.FileListView = _items;
            viewModel.SelectedFolder = this;
            viewModel.PleaseWait = false;
        });
    }

    private void SetNewListItems(Task task)
    {
        try
        {
            WpfExplorerViewModel viewModel = GetRootParent().MainViewModel!;
            if (viewModel.SelectedFolder!.NetworkRoot)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.FileListView = _networkItems;
                    viewModel.PleaseWait = false;
                    viewModel.ForceSelectedFolder();
                });
            }
            task?.Dispose();
        }
        catch (Exception)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }
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
        WpfExplorerViewModel viewModel = GetRootParent().MainViewModel!;
        viewModel.SetDisplay();
        string? specialFolder = null;
        if (specialFolder != null || FullPath.StartsWith("::") || RecycledBin)
            specialFolder = DisplayText;
        viewModel.AddNavigation(FullPath, specialFolder);
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
                            ExtensionsDirectory.EnumerateFolderContent(di.RootDirectory.FullName, out List<string> subDir, out _);
                            hasSubFolder = subDir.Count > 0;
                        }
                        else
                            hasSubFolder = true;
                    }
                    catch (Exception)
                    {
                        hasSubFolder = false;
                    }
                    dir = new OneDirectory(di, this, hasSubFolder, new ManagedShell.ShellFolders.ShellItem(di.RootDirectory.FullName).DisplayName) { IsItemVisible = true };
                    Children.Add(dir);
                }
            }
            else
            {
                LoadChildren();
            }
        }
        RefreshListView();
        GetRootParent().MainViewModel!.ScrollToFirstItem();
    }

    public override void EditMode(bool activate)
    {
        if (Drive != null || _specialFolder != null)
            return;
        base.EditMode(activate);
    }

    public override void Rename()
    {
        if (NewName != Path.GetFileName(FullPath))
        {
            Directory.Move(FullPath, Path.Combine(Path.GetDirectoryName(FullPath), NewName));
            DisplayText = NewName;
            if (ParentDirectory?.IsExpanded == true)
            {
                ParentDirectory.Children.Clear();
                ParentDirectory.LoadChildren();
            }
        }
        base.Rename();
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
        ExtensionsDirectory.FolderSize(FullPath, ref calculateSize, _cancellationToken!.Token);
        _lastSize = calculateSize;
        return Task.CompletedTask;
    }

    #endregion

    #region Relay commands

    [RelayCommand()]
    public void ContextMenuFolder()
    {
        bool root = _specialFolder == Environment.SpecialFolder.MyComputer || _specialFolder == Environment.SpecialFolder.Desktop || FullPath?.StartsWith("::") == true;
        if (RecycledBin && !root)
        {
            ContextMenuFilesOrFolder(this);
            return;
        }
        // When right click on an item tree view
        ShellContextMenu scm = new(GetRootParent().MainViewModel!)
        {
            Root = root,
            RecycledBin = RecycledBin,
        };
        DirectoryInfo dir;
        if (string.IsNullOrWhiteSpace(FullPath) || FullPath?.StartsWith("::") == true || RecycledBin)
            dir = new DirectoryInfo(DisplayText);
        else
            dir = new DirectoryInfo(FullPath);
        scm.ShowContextMenu(dir, Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
    }

    public override void DoubleClickFile()
    {
        WpfExplorerViewModel viewModel = GetRootParent().MainViewModel!;
        viewModel.BrowseTo(FullPath);
        viewModel.SetDisplay();
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

    public bool RecycledBin { get; set; }

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
                            hIcon = IconHelper.GetIconByPidl(pidl, (vm.ViewDetails ? IconSize.Small : vm.CurrentIconSize), out hOverlay);
                            NativeMethods.ILFree(pidl);
                        }
                        else
                        {
                            _icon = IconImageConverter.GetDefaultIcon();
                        }
                    }
                    else
                    {
                        hIcon = IconHelper.GetIconByFilename(FullPath, (vm.ViewDetails ? IconSize.Small : vm.CurrentIconSize), out hOverlay);
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
                            _icon = Explorip.Helpers.IconManager.Convert(icon);
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
                        IntPtr hIcon = IconHelper.GetIconByPidl(pidl, (vm.ViewDetails ? IconSize.Small : vm.CurrentIconSize), out IntPtr hOverlay);
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
                            hIcon = IconHelper.GetIconByPidl(pidl, IconSize.Small, out hOverlay);
                            NativeMethods.ILFree(pidl);
                        }
                        else
                        {
                            _treeViewIcon = IconImageConverter.GetDefaultIcon();
                        }
                    }
                    else
                    {
                        hIcon = IconHelper.GetIconByFilename(FullPath, IconSize.Small, out hOverlay);
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
                            _treeViewIcon = Explorip.Helpers.IconManager.Convert(icon);
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
                        IntPtr hIcon = IconHelper.GetIconByPidl(pidl, IconSize.Small, out IntPtr hOverlay);
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
                IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, IconSize.Small, out IntPtr hOverlay);
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
        WpfExplorerViewModel viewModel = GetRootParent().MainViewModel!;
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && viewModel.NbMillisecondsStartDragging > Constants.DelayIgnoreDrag)
        {
            List<string> filesAndFolders = [.. ((DataObject)e.Data).GetFileDropList().OfType<string>()];
            if (viewModel.DragDropKeyStates.HasFlag(DragDropKeyStates.LeftMouseButton))
            {
                if (Path.GetDirectoryName(filesAndFolders[0]) != FullPath || viewModel.DragDropKeyStates.HasFlag(DragDropKeyStates.ControlKey))
                {
                    Explorip.HookFileOperations.FilesOperations.FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
                    fileOp.ChangeOperationFlags(Explorip.HookFileOperations.FilesOperations.EFileOperation.FOF_RENAMEONCOLLISION |
                        Explorip.HookFileOperations.FilesOperations.EFileOperation.FOFX_ADDUNDORECORD |
                        Explorip.HookFileOperations.FilesOperations.EFileOperation.FOF_NOCONFIRMMKDIR);
                    foreach (string fs in filesAndFolders)
                    {
                        if (viewModel.DragDropKeyStates.HasFlag(DragDropKeyStates.ControlKey))
                            fileOp.CopyItem(fs, FullPath, Path.GetFileName(fs));
                        else
                            fileOp.MoveItem(fs, FullPath, Path.GetFileName(fs));
                    }
                    fileOp.PerformOperations();
                    fileOp.Dispose();
                }
            }
            else if (viewModel.DragDropKeyStates.HasFlag(DragDropKeyStates.RightMouseButton))
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

        WpfExplorerViewModel root = ParentDirectory.GetRootParent().MainViewModel!;
        if (root.CurrentControl.FileLV.DrawSelection)
            return;

        if ((Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed) &&
            !root.CurrentlyDraging)
        {
            root.CurrentlyDraging = true;
            DataObject data = new();
            data.SetFileDropList([FullPath]);
            DragDrop.DoDragDrop(root.CurrentControl, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }
    }

    #endregion
}
