using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.ShellFolders;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuItemViewModel : ObservableObject
{
    private readonly ShellFile _shellFile;
    private readonly int _deep;
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

    public StartMenuItemViewModel(ShellFile sf, int deep)
    {
        _deep = deep;
        _children = [];
        _shellFile = sf;
        _name = sf.DisplayName;
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
                Children.Add(new StartMenuItemViewModel(item, _deep + 1));
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
            if (e.ChangedButton == MouseButton.Left)
                ManagedShell.Common.Helpers.ShellHelper.StartProcess(_shellFile.Path);
            // TODO : Context menu when right click
        }
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
