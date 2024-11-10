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

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace ExploripComponents;

public abstract partial class OneFileSystem(string fullPath, string displayText, OneDirectory? parentDirectory) : ObservableObject()
{
    #region Fields

    private readonly OneDirectory? _parentDirectory = parentDirectory;
    protected ImageSource? _icon;
    protected ulong? _lastSize;
    private NativeMethods.ShFileInfo _fileInfo = new();
    private FileAttributes _fileAttributes;
    #region Binding properties

    [ObservableProperty()]
    private bool _renameMode;
    [ObservableProperty()]
    private bool _readOnlyBox = true;
    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private string _fullPath = fullPath;
    [ObservableProperty()]
    private string _displayText = displayText;
    [ObservableProperty()]
    private ImageSource? _iconOverlay;
    [ObservableProperty()]
    private bool _isItemVisible;
    [ObservableProperty()]
    private string _newName = displayText;

    #endregion

    #endregion

    #region Events

    partial void OnIsItemVisibleChanged(bool value)
    {
        if (value)
        {
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(DisplayText));
            OnPropertyChanged(nameof(Size));
            if (string.IsNullOrWhiteSpace(_fileInfo.szTypeName))
            {
                _fileInfo = new();
                NativeMethods.SHGetFileInfo(FullPath, NativeMethods.FILE_ATTRIBUTE.NULL, ref _fileInfo, (uint)Marshal.SizeOf(_fileInfo), NativeMethods.SHGFI.TypeName);
                if (!FullPath.StartsWith("::"))
                    try
                    {
                        _fileAttributes = File.GetAttributes(FullPath);
                    }
                    catch (Exception) { /* Ignore errors */ }
            }
            OnPropertyChanged(nameof(TypeName));
            OnPropertyChanged(nameof(FileAttributes));
            OnPropertyChanged(nameof(Hidden));
            OnPropertyChanged(nameof(ReadOnly));
            OnPropertyChanged(nameof(Opacity));
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

    partial void OnRenameModeChanged(bool value)
    {
        ReadOnlyBox = !value;
    }

    public virtual void EditMode(bool activate)
    {
        if (_parentDirectory == null)
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
        RenameMode = false;
    }

    #region Properties

    public virtual ImageSource? Icon
    {
        get
        {
            if (_icon == null && IsItemVisible && !string.IsNullOrWhiteSpace(FullPath))
            {
                IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.Small, out IntPtr hOverlay);
                _icon = IconImageConverter.GetImageFromHIcon(hIcon);
                if (hOverlay != IntPtr.Zero)
                    IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
            }
            return _icon;
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
        get { return Hidden ? 0.8 : 1; }
    }

    #endregion

    #region Relay commands

    [RelayCommand()]
    public void ContextMenuFiles()
    {
        if (_parentDirectory != null && _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.Count > 0)
        {
            List<FileSystemInfo> listFiles = [];
            foreach (OneFile file in _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.OfType<OneFile>())
                listFiles.Add(new FileInfo(file.FullPath));
            foreach (OneDirectory file in _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.OfType<OneDirectory>())
                listFiles.Add(new DirectoryInfo(file.FullPath));
            new ShellContextMenu().ShowContextMenu([.. listFiles], _parentDirectory.FullPath, Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
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

        if (_parentDirectory.GetRootParent().MainViewModel!.SelectedItems.Count > 0 && (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed) &&
            !_parentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging)
        {
            _parentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging = true;
            DataObject data = new();
            data.SetFileDropList([.. _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.Select(fs => fs.FullPath)]);
            DragDrop.DoDragDrop(_parentDirectory.GetRootParent().MainViewModel!.CurrentControl, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }
    }

    public virtual void Drop(object sender, DragEventArgs e)
    {
        if (_parentDirectory == null)
            return;

        _parentDirectory.GetRootParent().MainViewModel!.CurrentlyDraging = false;
    }

    #endregion
}
