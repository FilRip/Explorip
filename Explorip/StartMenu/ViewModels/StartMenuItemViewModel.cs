using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ManagedShell.ShellFolders;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuItemViewModel : ObservableObject
{
    private readonly ShellFile _shellFile;

    [ObservableProperty()]
    private List<StartMenuItemViewModel> _children;
    [ObservableProperty()]
    private string _name;
    [ObservableProperty()]
    private ImageSource _icon;
    [ObservableProperty()]
    private string _tip;

    public StartMenuItemViewModel(ShellFile sf)
    {
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
                Children.Add(new StartMenuItemViewModel(item));
    }

    public bool HasChild
    {
        get { return Children.Count > 0; }
    }
}
