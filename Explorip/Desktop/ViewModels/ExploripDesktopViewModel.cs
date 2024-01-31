using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Desktop.Controls;
using Explorip.Desktop.Windows;
using Explorip.Helpers;

using GongSolutions.Wpf.DragDrop;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell;

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
                item.GetIcon();
                OneDesktopItem ctrl = new()
                {
                    MyDataContext = item,
                };
                _parentDesktop.AddItem(ctrl);
            }
    }

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            try
            {
                OneDesktopItem item = ListItems().Find(i => i.MyDataContext.Name == e.Name);
                if (item != null)
                    _parentDesktop.MainGrid.Children.Remove(item);
            }
            catch (Exception) { /* Ignore errors, file not found/already removed from list */ }
        });
    }

    private void Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            try
            {
                ListItems().First(item => item.MyDataContext.Name == e.OldName).MyDataContext.Name = e.Name;
            }
            catch (Exception) { /* Ignore errors, file (old name) not found ? Must add ? */ }
        });
    }

    private void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            try
            {
                OneDesktopItem item = new();
                item.MyDataContext.Name = e.Name;
                item.MyDataContext.FullPath = e.FullPath;
                item.MyDataContext.GetIcon();
                item.MyDataContext.CurrentDesktop = this;
                item.MyDataContext.CurrentShellObject = ShellObject.FromParsingName(e.FullPath);
                _parentDesktop.AddItem(item);
            }
            catch (Exception) { /* Unable to add item ??? So what to do */ }
        });
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
        // Nothing to do here. Not yet.
    }

    public void DragOver(IDropInfo dropInfo)
    {
        dropInfo.Effects = DragDropEffects.Move;
    }

    public void DragLeave(IDropInfo dropInfo)
    {
        // Nothing to do here. Not yet.
    }

    public void Drop(IDropInfo dropInfo)
    {
        double dpi = _parentDesktop.AssociateScreen.ScaleFactor;
        int x = (int)(dropInfo.DropPosition.X / (Constants.Desktop.ITEM_SIZE_X.Value * dpi));
        int y = (int)(dropInfo.DropPosition.Y / (Constants.Desktop.ITEM_SIZE_Y.Value * dpi));
        List<OneDesktopItem> listItems = ListItems();
        OneDesktopItem dest = listItems.Find(i => Grid.GetColumn(i) == x && Grid.GetRow(i) == y);
        if (dropInfo.Data is OneDesktopItemViewModel itemToDrop)
        {
            OneDesktopItem item = listItems.Find(i => i.MyDataContext == itemToDrop);
            if (item != null)
            {
                if (dest == null)
                {
                    Grid.SetColumn(item, x);
                    Grid.SetRow(item, y);
                }
                else
                {
                    if (dest.MyDataContext.IsDirectory)
                    {
                        FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
                        fileOperation.MoveItem(item.MyDataContext.FullPath, dest.MyDataContext.FullPath, Path.GetFileName(item.MyDataContext.FullPath));
                        fileOperation.PerformOperations();
                        fileOperation.Dispose();
                    }
                    else
                        dest.MyDataContext.ExecuteCommand.Execute(item.MyDataContext.FullPath);
                }
            }
        }
        else if (dropInfo.Data is DataObject)
        {
            string[] itemsFromExplorer = (string[])((IDataObject)dropInfo.Data).GetData("FileDrop");
            string destination = (dest == null ? Environment.SpecialFolder.DesktopDirectory.FullPath() : dest.MyDataContext.FullPath);
            foreach (string file in itemsFromExplorer)
            {
                FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
                if (dropInfo.Effects == DragDropEffects.Copy)
                    fileOperation.CopyItem(file, destination, Path.GetFileName(file));
                else if (dropInfo.Effects == DragDropEffects.Move)
                    fileOperation.MoveItem(file, destination, Path.GetFileName(file));
                fileOperation.PerformOperations();
                fileOperation.Dispose();
            }
        }
    }

    #endregion
}
