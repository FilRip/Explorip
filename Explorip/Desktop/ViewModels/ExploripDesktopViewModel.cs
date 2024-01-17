﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Constants;
using Explorip.Desktop.Models;
using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Shell;

using Securify.ShellLink;

namespace Explorip.Desktop.ViewModels;

internal partial class ExploripDesktopViewModel : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<OneItem> _listDesktopFolder;
    [ObservableProperty()]
    private OneItem _selectedItem;

    private readonly FileSystemWatcher _watcher;

    public ExploripDesktopViewModel() : base()
    {
        _watcher = new FileSystemWatcher(Environment.SpecialFolder.DesktopDirectory.FullPath())
        {
            EnableRaisingEvents = true,
        };
        _watcher.Created += Watcher_Created;
        _watcher.Deleted += Watcher_Deleted;
        _watcher.Renamed += Watcher_Renamed;
        RefreshDesktopContent();
    }

    private void RefreshDesktopContent()
    {
        ObservableCollection<OneItem> newList = [];
        ShellFolder desktop = (ShellFolder)ShellObject.FromParsingName(Environment.SpecialFolder.DesktopDirectory.FullPath());
        OneItem item;
        foreach (ShellObject filename in desktop)
            if (filename.Name.ToLower() != "desktop.ini")
            {
                item = new()
                {
                    Name = filename.Name,
                };
                try
                {
                    if (filename is ShellFolder)
                        item.Icon = Icons.Folder;
                    else
                        item.Icon = IconManager.GetIconFromFile(filename.ParsingName, 0);
                    if (Path.GetExtension(filename.ParsingName) == ".lnk")
                    {
                        Shortcut shortcut = Shortcut.ReadFromFile(filename.ParsingName);
                        string iconLocation = shortcut.StringData?.IconLocation;
                        if (string.IsNullOrWhiteSpace(iconLocation))
                            iconLocation = shortcut.StringData?.RelativePath;
                        if (!string.IsNullOrWhiteSpace(iconLocation))
                        {
                            if (iconLocation.StartsWith(".") || iconLocation.StartsWith(Path.DirectorySeparatorChar.ToString()))
                                iconLocation = Path.GetFullPath(Environment.SpecialFolder.Desktop.FullPath() + Path.DirectorySeparatorChar + iconLocation);
                            if (!File.Exists(iconLocation))
                                iconLocation = shortcut.LinkInfo?.LocalBasePath;
                            item.Icon = IconManager.GetIconFromFile(iconLocation, shortcut.IconIndex);
                        }
                    }
                    item.Icon ??= IconManager.Convert(Icon.ExtractAssociatedIcon(filename.ParsingName));
                }
                catch (Exception) { /* Ignore errors, can't get icon */ }
                newList.Add(item);
            }
        ListDesktopFolder = newList;
    }

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        try
        {
            ListDesktopFolder.Remove(ListDesktopFolder.First(item => item.Name == e.Name));
        }
        catch (Exception) { /* Ignore errors, file not found/already removed from list */ }
    }

    private void Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        try
        {
            ListDesktopFolder.First(item => item.Name == e.OldName).Name = e.Name;
        }
        catch (Exception) { /* Ignore errors, file (old name) not found ? Must add ? */ }
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        OneItem item = new()
        {
            Name = e.Name,
        };
        ListDesktopFolder.Add(item);
    }

    [RelayCommand()]
    private void Quit()
    {
        Environment.Exit(0);
    }
}