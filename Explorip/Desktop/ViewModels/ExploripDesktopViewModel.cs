using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Constants;
using Explorip.Desktop.Controls;
using Explorip.Desktop.Windows;
using Explorip.Helpers;

using GongSolutions.Wpf.DragDrop;

using Microsoft.WindowsAPICodePack.Shell;

using Securify.ShellLink;

namespace Explorip.Desktop.ViewModels;

internal partial class ExploripDesktopViewModel : ObservableObject, IDropTarget
{
    private readonly ExploripDesktop _parentDesktop;
    private readonly FileSystemWatcher _watcher;

    public ExploripDesktopViewModel(ExploripDesktop parent) : base()
    {
        _parentDesktop = parent;
        _watcher = new FileSystemWatcher(Environment.SpecialFolder.DesktopDirectory.FullPath())
        {
            EnableRaisingEvents = true,
        };
        _watcher.Created += Watcher_Created;
        _watcher.Deleted += Watcher_Deleted;
        _watcher.Renamed += Watcher_Renamed;
    }

    internal void RefreshDesktopContent()
    {
        _parentDesktop.MainGrid.Children.Clear();
        // TODO : Add system icons
        RefreshDesktopContent(Environment.SpecialFolder.DesktopDirectory.FullPath());
        RefreshDesktopContent(Environment.SpecialFolder.CommonDesktopDirectory.FullPath());
    }

    private void RefreshDesktopContent(string desktopPath)
    {
        ShellFolder desktop = (ShellFolder)ShellObject.FromParsingName(desktopPath);
        OneDesktopItemViewModel item;
        foreach (ShellObject filename in desktop)
            if (filename.Name.Trim().ToLower() != "desktop.ini")
            {
                item = new()
                {
                    Name = filename.Name,
                    FullPath = filename.ParsingName,
                    CurrentShellObject = filename,
                    CurrentDesktop = this,
                };
                try
                {
                    if (filename is ShellFolder)
                    {
                        item.Icon = Icons.Folder;
                        item.IsDirectory = true;
                    }
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
                    item.Icon ??= IconManager.Convert(System.Drawing.Icon.ExtractAssociatedIcon(filename.ParsingName));
                }
                catch (Exception) { /* Ignore errors, can't get icon */ }
                OneDesktopItem ctrl = new()
                {
                    MyDataContext = item,
                };
                _parentDesktop.AddItem(ctrl);
            }
    }

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        try
        {
            OneDesktopItem item = ListItems().Find(i => i.MyDataContext.Name == e.Name);
            if (item != null)
                _parentDesktop.MainGrid.Children.Remove(item);
        }
        catch (Exception) { /* Ignore errors, file not found/already removed from list */ }
    }

    private void Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        try
        {
            ListItems().First(item => item.MyDataContext.Name == e.OldName).MyDataContext.Name = e.Name;
        }
        catch (Exception) { /* Ignore errors, file (old name) not found ? Must add ? */ }
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        OneDesktopItem item = new();
        item.MyDataContext.Name = e.Name;
        _parentDesktop.AddItem(item);
    }

    [RelayCommand()]
    private void Quit()
    {
        Environment.Exit(0);
    }

    internal void UnselectAll()
    {
        foreach (OneDesktopItem item in _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>())
            item.MyDataContext.IsSelected = false;
    }

    internal List<OneDesktopItem> ListItems()
    {
        return _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>().ToList();
    }

    internal FileSystemInfo[] ListSelectedItem()
    {
        return ListItems().Where(i => i.MyDataContext.IsSelected).Select(i => i.MyDataContext.FileSystemIO).ToArray();
    }

    #region Drag'n drop

    public void DragEnter(IDropInfo dropInfo)
    {
        // Nothing to to here, atm
    }

    public void DragOver(IDropInfo dropInfo)
    {
        dropInfo.Effects = DragDropEffects.Move;
    }

    public void DragLeave(IDropInfo dropInfo)
    {
        // Nothing to to here, atm
    }

    public void Drop(IDropInfo dropInfo)
    {
        double dpi = _parentDesktop.AssociateScreen.ScaleFactor;
        int x = (int)(dropInfo.DropPosition.X / (Constants.Desktop.ITEM_SIZE_X.Value * dpi));
        int y = (int)(dropInfo.DropPosition.Y / (Constants.Desktop.ITEM_SIZE_Y.Value * dpi));
        OneDesktopItem item = ListItems().Find(i => i.MyDataContext == (OneDesktopItemViewModel)dropInfo.Data);
        if (item != null)
        {
            Grid.SetColumn(item, x);
            Grid.SetRow(item, y);
        }
    }

    #endregion
}
