using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ExploripConfig.Configuration;

using ManagedShell.ShellFolders;

using Securify.ShellLink;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuItemViewModel : ObservableObject
{
    private readonly ShellFile _shellFile;
    private readonly int _deep;
    private readonly StartMenuViewModel _window;
    private bool _mouseOver;

    [ObservableProperty()]
    private List<StartMenuItemViewModel> _children;
    [ObservableProperty()]
    private string _name;
    [ObservableProperty()]
    private ImageSource _icon;
    [ObservableProperty()]
    private string _tip;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ShowDown), nameof(ShowUp))]
    private bool _isExpanded;

    public StartMenuItemViewModel(ShellFile sf, int deep, StartMenuViewModel window)
    {
        _deep = deep;
        _children = [];
        _shellFile = sf;
        _name = sf.DisplayName;
        _window = window;
        if (sf.IsFolder)
        {
            _icon = Constants.Icons.Folder;
            LoadChildren(new ShellFolder(_shellFile.Path, IntPtr.Zero));
        }
        else
            _icon = sf.LargeIcon;
    }

    public void LoadChildren(ShellFolder sf)
    {
        foreach (ShellFile item in sf.Files)
            if (!Children.Any(i => i.Name == sf.DisplayName))
            {
                StartMenuItemViewModel smivm = new(item, _deep + 1, _window);
                if (!item.IsFolder || smivm.Children.Count > 0)
                    Children.Add(smivm);
            }
    }

    public bool ShowDown
    {
        get { return Children.Count > 0 && !IsExpanded; }
    }

    public bool ShowUp
    {
        get { return IsExpanded; }
    }

    [RelayCommand()]
    private void MouseUp(MouseButtonEventArgs e)
    {
        if (_shellFile.IsFolder)
        {
            if (e.ChangedButton == MouseButton.Left)
                IsExpanded = !IsExpanded;
        }
        else
        {
            if (e.ChangedButton == MouseButton.Left &&
                ManagedShell.Common.Helpers.ShellHelper.StartProcess(_shellFile.Path, useShellExecute: true))
            {
                _window.HideWindow();
            }
        }
    }

    [RelayCommand()]
    private void PinToStartMenu()
    {
        string path = ConfigManager.StartMenuPinnedShortcutPath;
        path = Environment.ExpandEnvironmentVariables(path);
        string file = _shellFile.Path;
        string dest = Path.Combine(path, Path.GetFileName(file));
        if (Path.GetExtension(file) == ".lnk")
        {
            if (!File.Exists(dest))
                File.Copy(file, dest);
        }
        else
        {
            Shortcut sc = Shortcut.CreateShortcut(file);
            dest = Path.Combine(path, Path.GetFileNameWithoutExtension(file) + ".lnk");
            if (!File.Exists(dest))
                sc.WriteToFile(dest);
        }
        _window.RefreshPinnedShortcut();
    }

    public GridLength SizeNameColumn
    {
        get { return new GridLength(195 - (10 * _deep)); }
    }

    public Brush BackgroundItem
    {
        get
        {
            if (_mouseOver)
                return ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject;
            else
                return Brushes.Transparent;
        }
    }

    [RelayCommand()]
    private void MouseEnter()
    {
        _mouseOver = true;
        OnPropertyChanged(nameof(BackgroundItem));
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        _mouseOver = false;
        OnPropertyChanged(nameof(BackgroundItem));
    }
}
