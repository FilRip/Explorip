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
    private ImageSource? _icon;
    protected long _lastSize;

    [ObservableProperty()]
    private bool _isSelected;
    [ObservableProperty()]
    private string _fullPath = fullPath;
    [ObservableProperty()]
    private string _displayText = displayText;
    [ObservableProperty()]
    private ImageSource? _iconOverlay;

    public ImageSource? Icon
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

    partial void OnIsSelectedChanged(bool value)
    {
        if (value)
            Application.Current.Dispatcher.BeginInvoke(RefreshListView);
    }

    protected virtual void RefreshListView()
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

    public virtual long Size
    {
        get
        {
            if (_lastSize == 0)
                _lastSize = new FileInfo(FullPath).Length;
            return _lastSize;
        }
    }

    [RelayCommand()]
    public void ContextMenuFiles()
    {
        if (_parentDirectory != null && _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.Count > 0)
        {
            List<FileInfo> listFiles = [];
            foreach (OneFile file in _parentDirectory.GetRootParent().MainViewModel!.SelectedItems.OfType<OneFile>())
                listFiles.Add(new FileInfo(file.FullPath));
            new ShellContextMenu().ShowContextMenu(listFiles.ToArray(), Application.Current.MainWindow.PointToScreen(Mouse.GetPosition(Application.Current.MainWindow)));
        }
    }

    [RelayCommand()]
    public abstract void DoubleClickFile();
}
