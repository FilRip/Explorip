using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ExploripComponents.Helpers;
using ExploripComponents.ViewModels;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

namespace ExploripComponents.Models;

public abstract partial class OneFileSystem(string fullPath, string displayText, OneDirectory? parentDirectory) : ObservableObject()
{
    #region Fields

    private readonly OneDirectory? _parentDirectory = parentDirectory;
    protected ImageSource? _icon;
    protected ulong? _lastSize;
    private NativeMethods.ShFileInfo _fileInfo = new();
    protected FileAttributes _fileAttributes;
    private ImageSource? _thumbnail;
    protected DateTime _lastWriteTime;
    protected bool _fromRecycledBin;
    protected bool _isNetworkResource;
    protected IntPtr _pidl = IntPtr.Zero;
    protected IntPtr _pidlRelative = IntPtr.Zero;

    #region Binding properties

    [ObservableProperty()]
    private bool _renameMode;
    [ObservableProperty()]
    private bool _readOnlyBox = true;
    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private string _fullPath = fullPath;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(NameFirstLetter))]
    private string _displayText = displayText;
    [ObservableProperty()]
    private ImageSource? _iconOverlay;
    [ObservableProperty()]
    private bool _isItemVisible;
    [ObservableProperty()]
    private string _newName = displayText;
    [ObservableProperty()]
    private SolidColorBrush _backgroundColor = Brushes.Transparent;

    #endregion

    #endregion

    #region Events

    partial void OnIsItemVisibleChanged(bool value)
    {
        if (value)
        {
            OnPropertyChanged(nameof(IconOrThumbnail));
            OnPropertyChanged(nameof(DisplayText));
            OnPropertyChanged(nameof(Size));
            if (string.IsNullOrWhiteSpace(_fileInfo.szTypeName) && !_isNetworkResource)
            {
                _fileInfo = new();
                NativeMethods.SHGetFileInfo(FullPath, NativeMethods.FILE_ATTRIBUTE.NULL, ref _fileInfo, (uint)Marshal.SizeOf(_fileInfo), NativeMethods.SHGFI.TypeName);
                if (!FullPath.StartsWith("::"))
                {
                    try
                    {
                        _fileAttributes = File.GetAttributes(FullPath);
                    }
                    catch (Exception) { /* Ignore errors */ }
                    try
                    {
                        _lastWriteTime = File.GetLastWriteTime(FullPath);
                    }
                    catch (Exception) { /* Ignore errors */ }
                }
            }
            OnPropertyChanged(nameof(TypeName));
            OnPropertyChanged(nameof(FileAttributes));
            OnPropertyChanged(nameof(Hidden));
            OnPropertyChanged(nameof(ReadOnly));
            OnPropertyChanged(nameof(Opacity));
            OnPropertyChanged(nameof(LastModified));
            OnPropertyChanged(nameof(Duration));
        }
    }

    partial void OnIsSelectedChanged(bool oldValue, bool newValue)
    {
        if (!oldValue && newValue)
            OnSelectIt();
        else if (oldValue && !newValue)
            DeSelectIt();
    }

    protected virtual void OnSelectIt()
    {
    }

    protected virtual void DeSelectIt()
    {
    }

    #endregion

    #region Rename file/folder

    partial void OnRenameModeChanged(bool value)
    {
        ReadOnlyBox = !value;
    }

    public virtual void EditMode(bool activate)
    {
        if (_parentDirectory == null || _fromRecycledBin)
            return;

        if (activate)
        {
            if (RenameMode)
                return;
            NewName = Path.GetFileName(FullPath);
            RenameMode = true;
            _parentDirectory.GetRootParent().MainViewModel!.SetCurrentlyRenaming(this);
        }
        else
        {
            _parentDirectory.GetRootParent().MainViewModel!.SetCurrentlyRenaming(null);
            RenameMode = false;
        }
    }

    public virtual void Rename()
    {
        EditMode(false);
    }

    #endregion

    #region Properties

    public IntPtr Pidl
    {
        get { return _pidl; }
    }

    public IntPtr RelativePidl
    {
        get { return _pidlRelative; }
    }

    public virtual ImageSource? Icon
    {
        get
        {
            if (_icon == null && IsItemVisible && (!string.IsNullOrWhiteSpace(FullPath) || _pidl != IntPtr.Zero))
            {
                WpfExplorerViewModel vm = _parentDirectory!.GetRootParent().MainViewModel!;
                IntPtr hIcon, hOverlay;
                if (_pidl == IntPtr.Zero)
                    hIcon = IconHelper.GetIconByFilename(FullPath, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out hOverlay);
                else
                    hIcon = IconHelper.GetIconByPidl(_pidl, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out hOverlay);

                if (hIcon != IntPtr.Zero)
                    _icon = IconImageConverter.GetImageFromHIcon(hIcon);
                if (hOverlay != IntPtr.Zero)
                    IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
            }
            return _icon;
        }
    }

    public ImageSource? Thumbnail
    {
        get
        {
            if (_thumbnail == null && IsItemVisible && (!string.IsNullOrWhiteSpace(FullPath) || _pidl != IntPtr.Zero))
            {
                WpfExplorerViewModel vm = _parentDirectory!.GetRootParent().MainViewModel!;
                if (!string.IsNullOrWhiteSpace(FullPath))
                {
                    try
                    {
                        Guid guid = typeof(IShellItem).GUID;
                        NativeMethods.SHCreateItemFromParsingName(FullPath, IntPtr.Zero, ref guid, out IntPtr ptrSi);
                        if (ptrSi != IntPtr.Zero)
                        {
                            IShellItemImageFactory factory = (IShellItemImageFactory)Marshal.GetObjectForIUnknown(ptrSi);
                            factory.GetImage(new ManagedShell.ShellFolders.Interfaces.Size((int)vm.IconSizePx.Value, (int)vm.IconSizePx.Value), ManagedShell.ShellFolders.Enums.SIIGBF.THUMBNAILONLY, out IntPtr hIcon);
                            if (hIcon != IntPtr.Zero)
                            {
                                _thumbnail = IconImageConverter.GetImageFromHBitmap(hIcon);
                                NativeMethods.DestroyIcon(hIcon);
                            }
                            Marshal.ReleaseComObject(factory);
                            Marshal.Release(ptrSi);
                        }
                    }
                    catch (Exception)
                    {
                        _thumbnail = Icon;
                    }
                }
                if (IconOverlay == null)
                {
                    IntPtr hIcon, hOverlay;
                    if (_pidl == IntPtr.Zero)
                        hIcon = IconHelper.GetIconByFilename(FullPath, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out hOverlay);
                    else
                        hIcon = IconHelper.GetIconByPidl(_pidl, (vm.ViewDetails ? ManagedShell.Common.Enums.IconSize.Small : vm.CurrentIconSize), out hOverlay);
                    if (hIcon != IntPtr.Zero)
                    {
                        if (_pidl != IntPtr.Zero)
                            _thumbnail = IconImageConverter.GetImageFromHIcon(hIcon);
                        NativeMethods.DestroyIcon(hIcon);
                        if (hOverlay != IntPtr.Zero)
                            IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
                    }
                }
            }
            return _thumbnail;
        }
    }

    public ImageSource? IconOrThumbnail
    {
        get
        {
            WpfExplorerViewModel vm = _parentDirectory!.GetRootParent().MainViewModel!;
            if (vm.ShowThumbnail)
                return Thumbnail;
            else
                return Icon;
        }
    }

    public FileAttributes FileAttributes
    {
        get { return _fileAttributes; }
    }

    public bool ReadOnly
    {
        get { return FileAttributes.HasFlag(FileAttributes.ReadOnly); }
    }

    public virtual bool Hidden
    {
        get { return FileAttributes.HasFlag(FileAttributes.Hidden); }
    }

    public virtual ulong Size
    {
        get
        {
            _lastSize ??= (ulong)new FileInfo(FullPath).Length;
            return _lastSize.Value;
        }
    }

    public OneDirectory? ParentDirectory
    {
        get { return _parentDirectory; }
    }

    public string TypeName
    {
        get { return _fileInfo.szTypeName; }
    }

    public double Opacity
    {
        get { return Hidden ? 0.75 : 1; }
    }

    public char NameFirstLetter
    {
        get { return DisplayText.ToUpper()[0]; }
    }

    public DateTime LastModified
    {
        get { return _lastWriteTime; }
    }

    public virtual string Duration
    {
        get { return null; }
    }

    #endregion

    #region Relay commands

    [RelayCommand()]
    public void ContextMenuFiles(MouseButtonEventArgs e)
    {
        if (_parentDirectory != null && _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.Count > 0)
        {
            ContextMenuFilesOrFolder();
            e.Handled = true;
        }
    }

    public void ContextMenuFilesOrFolder(OneDirectory? forceDirectory = null)
    {
        Point positionPopupMenu = Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow));
        WpfExplorerViewModel viewModel = _parentDirectory!.GetRootParent()!.MainViewModel!;
        if (_pidlRelative == IntPtr.Zero || forceDirectory != null)
        {
            List<FileSystemInfo> listFiles = [];
            if (forceDirectory is OneDirectory dir)
                listFiles.Add(new DirectoryInfo(dir.FullPath));
            else
            {
                foreach (OneFile file in viewModel.SelectedItems.OfType<OneFile>())
                    listFiles.Add(new FileInfo(file.FullPath));
                foreach (OneDirectory file in viewModel.SelectedItems.OfType<OneDirectory>())
                    listFiles.Add(new DirectoryInfo(file.FullPath));
            }
            new ShellContextMenu(viewModel)
            {
                RecycledBin = _fromRecycledBin,
            }.ShowContextMenu([.. listFiles], _parentDirectory.FullPath, positionPopupMenu);
        }
        else
        {
            List<IntPtr> listItems = [];
            foreach (OneFile file in viewModel.SelectedItems.OfType<OneFile>())
                listItems.Add(file.RelativePidl);
            foreach (OneDirectory file in viewModel.SelectedItems.OfType<OneDirectory>())
                listItems.Add(file.RelativePidl);
            new ShellContextMenu(viewModel)
            {
                RecycledBin = _fromRecycledBin,
                NetworkNeighbohood = _isNetworkResource,
                ReleasePidls = false,
            }.ShowContextMenu([.. listItems], _parentDirectory.FullPath, positionPopupMenu);
        }
    }

    [RelayCommand()]
    public abstract void DoubleClickFile();

    #endregion

    #region Drag'n Drop

    [RelayCommand()]
    public virtual void MouseMove()
    {
        if (_parentDirectory == null)
            return;

        WpfExplorerViewModel viewModel = _parentDirectory.GetRootParent().MainViewModel!;
        if (viewModel.SelectedItems.Count > 0 && (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed) &&
            !viewModel.CurrentlyDraging && viewModel.CurrentlyRenaming == null)
        {
            viewModel.CurrentlyDraging = true;
            DataObject data = new();
            data.SetFileDropList([.. viewModel.SelectedItems.Select(fs => fs.FullPath)]);
            DragDrop.DoDragDrop(viewModel.CurrentControl, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }
    }

    public virtual void Drop(object sender, DragEventArgs e)
    {
        if (_parentDirectory == null)
            return;
        BackgroundColor = Brushes.Transparent;
        _parentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging = false;
    }

    #endregion
}
