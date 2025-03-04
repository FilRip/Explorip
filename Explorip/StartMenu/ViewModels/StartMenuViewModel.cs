using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;

using ManagedShell.ShellFolders;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuViewModel : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<StartMenuItemViewModel> _startMenuItems;
    [ObservableProperty()]
    private ObservableCollection<PinnedShortutViewModel> _pinnedShortcut;

    public StartMenuViewModel()
    {
        RefreshPrograms();
        RefreshPinnedShortcut();
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
                StartMenuItems.Add(new StartMenuItemViewModel(sf, 0));
        }

        return ret;
    }

    private void RefreshPinnedShortcut()
    {
        PinnedShortcut = [];
        string path = Path.Combine(Environment.SpecialFolder.ApplicationData.FullPath(), "CoolBytes", "Explorip", "StartMenu");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file));
    }
}
