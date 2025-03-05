using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.ShellFolders;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuViewModel : ObservableObject
{
    private readonly double _iconSizeWidth, _iconSizeHeight, _iconSizeWidth2, _iconSizeHeight2;

    [ObservableProperty()]
    private ObservableCollection<StartMenuItemViewModel> _startMenuItems;
    [ObservableProperty()]
    private ObservableCollection<PinnedShortutViewModel> _pinnedShortcut;
    [ObservableProperty()]
    private ObservableCollection<PinnedShortutViewModel> _pinnedShortcut2;
    [ObservableProperty()]
    private bool _showPanel2;

    public Action ShowWindow { get; set; }
    public Action HideWindow { get; set; }

    public StartMenuViewModel()
    {
        _iconSizeHeight = ConfigManager.StartMenuIconSizeHeight;
        _iconSizeWidth = ConfigManager.StartMenuIconSizeWidth;
        _iconSizeHeight2 = ConfigManager.StartMenuIconSizeHeight2;
        _iconSizeWidth2 = ConfigManager.StartMenuIconSizeWidth2;
        _showPanel2 = ConfigManager.StartMenuShowPinnedApp2;
        RefreshPrograms();
        RefreshPinnedShortcut();
        RefreshPinnedShortcut2();
    }

    public void RefreshPrograms()
    {
        StartMenuItems = [];
        string commonSMFolder = Path.Combine(Environment.SpecialFolder.CommonStartMenu.FullPath(), "Programs");
        string mySMFolder = Path.Combine(Environment.SpecialFolder.StartMenu.FullPath(), "Programs");
        List<StartMenuItemViewModel> commonSM = GetRootSM(new ShellFolder(commonSMFolder, IntPtr.Zero));
        StartMenuItems.AddRange(commonSM);
        List<StartMenuItemViewModel> MySM = GetRootSM(new ShellFolder(mySMFolder, IntPtr.Zero));
        StartMenuItems.AddRange(MySM);
        StartMenuItems = [.. (StartMenuItems.OrderBy(i => i.Name))];
    }

    private List<StartMenuItemViewModel> GetRootSM(ShellFolder folder)
    {
        List<StartMenuItemViewModel> ret = [];
        foreach (ShellFile sf in folder.Files)
        {
            StartMenuItemViewModel alreadyExist = StartMenuItems.SingleOrDefault(i => i.Name == sf.DisplayName);
            if (alreadyExist != null)
            {
                if (sf.IsFolder)
                    alreadyExist.LoadChildren(new ShellFolder(sf.Path, IntPtr.Zero));
            }
            else
                StartMenuItems.Add(new StartMenuItemViewModel(sf, 0, this));
        }

        return ret;
    }

    private void RefreshPinnedShortcut()
    {
        PinnedShortcut = [];
        string path = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenuPinnedShortcutPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file, this));
    }

    private void RefreshPinnedShortcut2()
    {
        PinnedShortcut2 = [];
        string path = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenuPinnedShortcutPath2);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file, this, true));
    }

    public double IconSizeWidth
    {
        get { return _iconSizeWidth; }
    }

    public double IconSizeHeight
    {
        get { return _iconSizeHeight; }
    }

    public double IconSizeWidth2
    {
        get { return _iconSizeWidth2; }
    }

    public double IconSizeHeight2
    {
        get { return _iconSizeHeight2; }
    }
}
