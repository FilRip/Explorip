using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Desktop.Controls;
using Explorip.Desktop.Windows;
using Explorip.Helpers;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell.Common;

using Securify.ShellLink;

namespace Explorip.Desktop.ViewModels;

internal partial class ExploripDesktopViewModel : ObservableObject, IDisposable
{
    private readonly ExploripDesktop _parentDesktop;
    private readonly FileSystemWatcher _watcher;
    private int _currentCellX, _currentCellY;
    [ObservableProperty()]
    private int _nbColumns, _nbRows;
    private bool disposedValue;

    internal string DesktopPath { get; set; }

    public ExploripDesktopViewModel(ExploripDesktop parent, string path = null) : base()
    {
        if (string.IsNullOrWhiteSpace(path))
            DesktopPath = Environment.SpecialFolder.DesktopDirectory.FullPath();
        else
            DesktopPath = path;
        _parentDesktop = parent;
        _watcher = new FileSystemWatcher(DesktopPath)
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
        RefreshSystemIcons();
        RefreshDesktopContent(DesktopPath);
        RefreshDesktopContent(Environment.SpecialFolder.CommonDesktopDirectory.FullPath());
    }

    private void RefreshSystemIcons()
    {
        foreach (OneDesktopItemViewModel vm in Configuration.RegistrySettings.ListDesktopSystemIcons())
        {
            if (vm.Icon == null)
                vm.GetIcon();
            vm.CurrentDesktop = this;
            OneDesktopItem ctrl = new()
            {
                MyDataContext = vm,
            };
            _parentDesktop.AddItem(ctrl);
        }
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
                _parentDesktop.AddItem(item);
            }
            catch (Exception) { /* Unable to add item ??? So what to do */ }
        });
    }

    [RelayCommand()]
    private void Quit()
    {
        Application.Current.Shutdown(0);
    }

    internal void UnSelectAll(bool select = false)
    {
        foreach (OneDesktopItem item in _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>())
            item.MyDataContext.IsSelected = select;
    }

    internal List<OneDesktopItem> ListItems()
    {
        return _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>().ToList();
    }

    internal FileSystemInfo[] ListSelectedItem()
    {
        return ListItems().Where(i => i.MyDataContext.IsSelected && i.MyDataContext.FileSystemIO != null).Select(i => i.MyDataContext.FileSystemIO).ToArray();
    }

    [RelayCommand()]
    private void ActionRightClick(object args)
    {
        if (args is MouseButtonEventArgs)
        {
            if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneDesktopItemViewModel item)
            {
                if (!item.IsSelected)
                {
                    UnSelectAll();
                    item.IsSelected = true;
                }
            }
            else
                UnSelectAll();

            ManagedShell.ShellFolders.Models.ShellContextMenu contextMenu = new();
            Point position = _parentDesktop.PointToScreen(Mouse.GetPosition(_parentDesktop));
            FileSystemInfo[] listItems = ListSelectedItem();
            if (listItems.Length == 0)
            {
                if (ListItems().Exists(i => i.MyDataContext.IsSelected))
                    contextMenu.ShowContextMenu(ListItems()[0].MyDataContext.FullPath, position);
                else
                    contextMenu.ShowContextMenu(new DirectoryInfo(DesktopPath), position);
            }
            else
                contextMenu.ShowContextMenu(listItems, position);
        }
    }

    [RelayCommand()]
    internal void ActionOnKey(object args)
    {
        if (args is KeyEventArgs e)
        {
            if (e.Key == Key.PageDown && Keyboard.IsKeyDown(Key.RightCtrl))
                Quit();
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.A)
                    UnSelectAll(true);
                return;
            }
            int x = _currentCellX, y = _currentCellY;
            if (e.Key == Key.Up && y > 0)
                y--;
            else if (e.Key == Key.Down && y < NbRows - 1)
                y++;
            else if (e.Key == Key.Left && x > 0)
                x--;
            else if (e.Key == Key.Right && x < NbColumns - 1)
                x++;
            if (x != _currentCellX || y != _currentCellY)
            {
                OneDesktopItem item = ListItems().Find(i => Grid.GetColumn(i) == x && Grid.GetRow(i) == y);
                if (item != null)
                {
                    _currentCellX = x;
                    _currentCellY = y;
                    item.Focus();
                    Keyboard.Focus(item);
                }
            }
        }
    }

    #region Drag'n drop

    internal void StartDrag(OneDesktopItemViewModel item)
    {
        DataObject data = new();
        List<string> listDrag = [];
        foreach (FileSystemInfo fs in ListSelectedItem())
            listDrag.Add(fs.FullName);
        if (listDrag.Count == 0)
            listDrag.Add(item.FullPath);
        data.SetData("FileDrop", listDrag.ToArray());
        DragDrop.DoDragDrop(_parentDesktop, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
    }

    internal void Drop(DragEventArgs e)
    {
        double dpi = _parentDesktop.AssociateScreen.ScaleFactor;
        int x = (int)(e.GetPosition(_parentDesktop).X / (Constants.Desktop.ITEM_SIZE_X.Value * dpi));
        int y = (int)(e.GetPosition(_parentDesktop).Y / (Constants.Desktop.ITEM_SIZE_Y.Value * dpi));
        List<OneDesktopItem> listItems = ListItems();
        OneDesktopItem dest = listItems.Find(i => Grid.GetColumn(i) == x && Grid.GetRow(i) == y);
        if (e.Data is DataObject && e.Data.GetDataPresent("FileDrop"))
        {
            string[] itemsFromExplorer = (string[])(e.Data.GetData("FileDrop"));
            string destination = (dest == null ? DesktopPath : dest.MyDataContext.FullPath);
            string repSource = Path.GetDirectoryName(itemsFromExplorer[0]);
            bool sameDrive = false;
            if (!string.IsNullOrWhiteSpace(repSource))
                sameDrive = repSource[0] == DesktopPath[0];
            foreach (string fs in itemsFromExplorer)
            {
                OneDesktopItem item = listItems.Find(i => i.MyDataContext.FullPath == fs);
                if (item != null)
                {
                    if (item == dest)
                        break;
                    if (dest == null)
                    {
                        if (e.KeyStates == DragDropKeyStates.ControlKey)
                        {
                            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
                            fileOperation.ChangeOperationFlags(FilesOperations.Interfaces.EFileOperation.FOF_RENAMEONCOLLISION);
                            fileOperation.CopyItem(fs, destination, Path.GetFileName(fs));
                            fileOperation.PerformOperations();
                            fileOperation.Dispose();
                        }
                        else
                        {
                            Grid.SetColumn(item, x);
                            Grid.SetRow(item, y);
                            _parentDesktop.DesktopRegistryKey.SetValue(item.Name, $"{x}, {y}");
                        }
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
                            dest.MyDataContext.ExecuteCommand.Execute("\"" + item.MyDataContext.FullPath + "\"");
                    }
                    break;
                }
                else
                {
                    if (dest != null && Path.GetExtension(destination).ToLower() == ".lnk")
                    {
                        Shortcut shortcut = Shortcut.ReadFromFile(destination);
                        if (!string.IsNullOrWhiteSpace(shortcut.Target))
                        {
                            if (Directory.Exists(shortcut.Target))
                                destination = shortcut.Target;
                            else
                            {
                                dest.MyDataContext.ExecuteCommand.Execute("\"" + fs + "\"");
                                continue;
                            }
                        }
                    }
                    FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
                    if (e.Effects.HasFlag(DragDropEffects.Copy) && (e.KeyStates == DragDropKeyStates.ControlKey || !sameDrive))
                        fileOperation.CopyItem(fs, destination, Path.GetFileName(fs));
                    else if (e.Effects.HasFlag(DragDropEffects.Move) && (e.KeyStates == DragDropKeyStates.ShiftKey || sameDrive))
                        fileOperation.MoveItem(fs, destination, Path.GetFileName(fs));
                    else
                    {
                        // TODO : Operation seems impossible
                    }
                    fileOperation.PerformOperations();
                    fileOperation.Dispose();
                }
            }
        }
    }

    #endregion

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    internal void DragOver(DragEventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            e.Effects = DragDropEffects.Copy;
        else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            e.Effects = DragDropEffects.Link;
        else
            e.Effects = e.AllowedEffects.HasFlag(DragDropEffects.Move) ? DragDropEffects.Move : DragDropEffects.Copy;
        e.Handled = true;
    }

    #region IDisposable

    internal bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_watcher != null)
                {
                    _watcher.Created -= Watcher_Created;
                    _watcher.Deleted -= Watcher_Deleted;
                    _watcher.Renamed -= Watcher_Renamed;
                    _watcher.Dispose();
                }
                if (ListItems()?.Count > 0)
                    foreach (OneDesktopItem item in ListItems())
                    {
                        item.MyDataContext?.Dispose();
                        _parentDesktop.MainGrid.Children.Remove(item);
                    }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
