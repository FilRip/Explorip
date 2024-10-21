using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;

namespace ExploripComponents;

public abstract partial class OneFileSystem(string fullPath, string displayText, OneDirectory? parentDirectory) : ObservableObject
{
    protected readonly OneDirectory? _parentDirectory = parentDirectory;
    protected ImageSource? _icon;
    protected ulong? _lastSize;

    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private string _fullPath = fullPath;
    [ObservableProperty()]
    private string _displayText = displayText;
    [ObservableProperty()]
    private ImageSource? _iconOverlay;

    public virtual ImageSource? Icon
    {
        get
        {
            if (_icon == null && !string.IsNullOrWhiteSpace(FullPath))
            {
                IntPtr hIcon = IconHelper.GetIconByFilename(FullPath, ManagedShell.Common.Enums.IconSize.Small, out nint hOverlay);
                _icon = IconImageConverter.GetImageFromHIcon(hIcon);
                if (hOverlay != IntPtr.Zero)
                    IconOverlay = IconImageConverter.GetImageFromHIcon(hOverlay);
            }
            return _icon;
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

    public FileAttributes FileAttributes
    {
        get { return File.GetAttributes(FullPath); }
    }

    public bool Hidden
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
}
