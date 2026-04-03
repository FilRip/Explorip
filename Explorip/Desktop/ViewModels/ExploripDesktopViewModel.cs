using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Desktop.Controls;
using Explorip.Desktop.Windows;
using Explorip.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell.Common;

using Securify.ShellLink;

namespace Explorip.Desktop.ViewModels;

public partial class ExploripDesktopViewModel : ObservableObject, IDisposable
{
    private readonly ExploripDesktop _parentDesktop;
    private readonly FileSystemWatcher _watcher;
    private int _currentCellX, _currentCellY;
    [ObservableProperty()]
    private int _nbColumns, _nbRows;
    private bool disposedValue;
    [ObservableProperty()]
    private Thickness _borderSize;
    [ObservableProperty()]
    private SolidColorBrush _borderColor;
    [ObservableProperty()]
    private CornerRadius _borderRadius;
    [ObservableProperty()]
    private SolidColorBrush _mouseOverColor;
    [ObservableProperty()]
    private SolidColorBrush _selectedColor;

    internal string DesktopPath { get; set; }

    public ExploripDesktop ParentDesktop
    {
        get { return _parentDesktop; }
    }

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

        BorderSize = new Thickness(ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).BorderSize);
        BorderColor = new SolidColorBrush(ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).BorderColor);
        BorderRadius = new CornerRadius(ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).BorderRadius);

        MouseOverColor = new SolidColorBrush(ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).MouseOverBackgroundColor);
        SelectedColor = new SolidColorBrush(ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).SelectedItemBackgroundColor);
    }

    #region Refresh list icons

    internal void RefreshDesktopContent()
    {
        if (_parentDesktop.MainGrid.Children.Count > 0)
            foreach (OneDesktopItem item in _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>())
                item.DataContext.Dispose();
        _parentDesktop.MainGrid.Children.Clear();
        RefreshSystemIcons();
        RefreshDesktopContent(DesktopPath);
        if (ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).ShowCommonDesktop)
            RefreshDesktopContent(Environment.SpecialFolder.CommonDesktopDirectory.FullPath());
    }

    private void RefreshDesktopContent(string desktopPath)
    {
        ShellFolder desktop = (ShellFolder)ShellObject.FromParsingName(desktopPath);
        OneDesktopItemViewModel item;
        foreach (ShellObject filename in desktop.Where(filename => filename.Name.Trim().ToLower() != "desktop.ini"))
        {
            int numScreen = ConfigManager.GetDesktopScreenForIcon(filename.Name);
            if ((numScreen < 0 && ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).ShowAllIcons) ||
                numScreen == _parentDesktop.ScreenId ||
                (numScreen >= 0 && !WpfScreenHelper.Screen.AllScreens.Any(s => s.DisplayNumber == numScreen)))
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
                    DataContext = item,
                };
                _parentDesktop.AddItem(ctrl);
            }
        }
    }

    private void RefreshSystemIcons()
    {
        foreach (OneDesktopItemViewModel vm in Configuration.RegistrySettings.ListDesktopSystemIcons(withCommon: ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).ShowCommonIcons))
        {
            int numScreen = ConfigManager.GetDesktopScreenForIcon(vm.Name);
            if ((numScreen < 0 && ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).ShowAllIcons) ||
                numScreen == _parentDesktop.ScreenId ||
                (numScreen >= 0 && !WpfScreenHelper.Screen.AllScreens.Any(s => s.DisplayNumber == numScreen)))
            {
                if (vm.Icon == null)
                    vm.GetIcon();
                vm.CurrentDesktop = this;
                OneDesktopItem ctrl = new()
                {
                    DataContext = vm,
                };
                _parentDesktop.AddItem(ctrl);
            }
            else
                vm.Dispose();
        }
    }

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            try
            {
                OneDesktopItem item = ListItems().Find(i => i.DataContext.Filename == e.Name);
                if (item != null)
                {
                    _parentDesktop.MainGrid.Children.Remove(item);
                    item.DataContext.Dispose();
                }
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
                ListItems().First(item => item.DataContext.Filename == e.OldName).DataContext.Filename = e.Name;
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
                item.DataContext.Name = e.Name;
                item.DataContext.FullPath = e.FullPath;
                item.DataContext.GetIcon();
                item.DataContext.CurrentDesktop = this;
                _parentDesktop.AddItem(item);
            }
            catch (Exception) { /* Unable to add item ??? So what to do */ }
        });
    }

    #endregion

    [RelayCommand()]
    private void Quit()
    {
        Application.Current.Shutdown(0);
    }

    internal void UnSelectAll(bool select = false)
    {
        foreach (OneDesktopItem item in _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>())
            item.DataContext.IsSelected = select;
    }

    internal List<OneDesktopItem> ListItems()
    {
        return [.. _parentDesktop.MainGrid.Children.OfType<OneDesktopItem>()];
    }

    internal void RemoveItem(OneDesktopItem item)
    {
        _parentDesktop.MainGrid.Children.Remove(item);
    }

    internal FileSystemInfo[] ListSelectedItem()
    {
        return [.. ListItems().Where(i => i.DataContext.IsSelected && i.DataContext.FileSystemIO != null).Select(i => i.DataContext.FileSystemIO)];
    }

    internal OneDesktopItem[] ListSelectedControl()
    {
        return [.. ListItems().Where(i => i.DataContext.IsSelected && i.DataContext.FileSystemIO != null)];
    }

    [RelayCommand()]
    private void ActionRightClick(object args)
    {
        DragGhostAdorner.StopDragGhost();
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
                if (ListItems().Exists(i => i.DataContext.IsSelected))
                    contextMenu.ShowContextMenu(ListItems().First(i => i.DataContext.IsSelected).DataContext.FullPath, position);
                else
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        contextMenu.ShowContextMenu(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)), position);
                    else
                        contextMenu.ShowContextMenu("shell:::{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", position, true);
                }
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
            int x = _currentCellX, y = _currentCellY;
            if (e.Key == Key.PageDown && Keyboard.IsKeyDown(Key.RightCtrl))
                Quit();
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.A)
                    UnSelectAll(true);
            }
            else if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                foreach (OneDesktopItem item in ListSelectedControl())
                    item.DataContext.ExecuteCommand.Execute(null);
            }
            else if (e.Key == Key.Delete)
            {
                foreach (OneDesktopItem item in ListSelectedControl())
                    item.DataContext.Delete();
            }
            else if (e.Key == Key.F5)
                RefreshDesktopContent();
            else if (e.Key == Key.Up && y > 0)
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
                item ??= _parentDesktop.DataContext.ListItems()?.FirstOrDefault();
                if (item != null)
                {
                    UnSelectAll(false);
                    item.DataContext.IsSelected = true;
                    _currentCellX = x;
                    _currentCellY = y;
                    item.Focus();
                    Keyboard.Focus(item);
                }
                else
                {
                    _currentCellX = x;
                    _currentCellY = y;
                }
            }
        }
    }

    #region Drag'n drop

    internal void StartDrag(OneDesktopItem item, MouseEventArgs e)
    {
        DataObject data = new();
        List<string> listDrag = [];
        foreach (FileSystemInfo fs in ListSelectedItem())
            listDrag.Add(fs.FullName);
        if (listDrag.Count == 0)
            listDrag.Add(item.DataContext.FullPath);
        data.SetData("FileDrop", listDrag.ToArray());
        DragGhostAdorner.StartDragGhost(item, e);
        DragDrop.DoDragDrop(_parentDesktop, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
    }

    internal void Drop(DragEventArgs e)
    {
        double dpi = _parentDesktop.AssociateScreen.ScaleFactor;
        int x = (int)(e.GetPosition(_parentDesktop).X / (ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).ItemSizeX * dpi));
        int y = (int)(e.GetPosition(_parentDesktop).Y / (ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).ItemSizeY * dpi));
        List<OneDesktopItem> listItems = ListItems();
        OneDesktopItem dest = listItems.Find(i => Grid.GetColumn(i) == x && Grid.GetRow(i) == y);
        if (e.Data is DataObject && e.Data.GetDataPresent("FileDrop"))
        {
            string[] itemsFromExplorer = (string[])(e.Data.GetData("FileDrop"));
            string destination = (dest == null ? DesktopPath : dest.DataContext.FullPath);
            string repSource = Path.GetDirectoryName(itemsFromExplorer[0]);
            bool sameDrive = false;
            if (!string.IsNullOrWhiteSpace(repSource))
                sameDrive = repSource[0] == DesktopPath[0];
            foreach (string fs in itemsFromExplorer)
            {
                OneDesktopItem item = listItems.Find(i => i.DataContext.FullPath == fs);
                if (item != null)
                {
                    if (item == dest) // Item drop on the same place
                        break;
                    if (dest == null) // Dropped on a empty space of desktop
                    {
                        if (e.KeyStates == DragDropKeyStates.ControlKey)
                        {
                            // If we move in same desktop with Ctrl pressed, then we make a copy of this item
                            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
                            fileOperation.ChangeOperationFlags(FilesOperations.Interfaces.EFileOperation.FOF_RENAMEONCOLLISION);
                            fileOperation.CopyItem(fs, destination, Path.GetFileName(fs));
                            fileOperation.PerformOperations();
                            fileOperation.Dispose();
                        }
                        else
                        {
                            // If we move item in the same desktop, just change the place
                            Grid.SetColumn(item, x);
                            Grid.SetRow(item, y);
                            ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).SetItemPosition(item.DataContext.Name, (x, y));
                        }
                    }
                    else
                    {
                        // If we drop to a place where it's occupied by a folder in the desktop, then we move item in this folder
                        if (dest.DataContext.IsDirectory)
                        {
                            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
                            fileOperation.MoveItem(item.DataContext.FullPath, dest.DataContext.FullPath, Path.GetFileName(item.DataContext.FullPath));
                            fileOperation.PerformOperations();
                            fileOperation.Dispose();
                        }
                        else // If we drop to a place where it's occupied by an item that it's not a folder, then we launch it with the item as parameter (like, for example ; drop txt item file in a shortcut of notepad)
                            dest.DataContext.ExecuteCommand.Execute("\"" + item.DataContext.FullPath + "\"");
                    }
                    break;
                }
                else
                {
                    foreach (ExploripDesktop ed in MyDesktopApp.ListDesktop)
                    {
                        // If we move between desktop of different monitors
                        item = ed.DataContext.ListItems().Find(i => i.DataContext.FullPath == fs);
                        if (item != null)
                        {
                            ConfigManager.GetDesktopConfig(ed.ScreenId).SetItemPosition(item.DataContext.Name, (-1, -1));
                            ed.DataContext.RemoveItem(item);
                            listItems.Add(item);
                            ConfigManager.GetDesktopConfig(_parentDesktop.ScreenId).SetItemPosition(item.DataContext.Name, (x, y));
                            break;
                        }
                    }
                    // The dropped item come from another directory than the desktops (of all monitors) (like picked from explorer then dropped to desktop). Then it's for add items in desktop
                    if (item == null)
                    {
                        if (dest != null && Path.GetExtension(destination).ToLower() == ".lnk")
                        {
                            Shortcut shortcut = Shortcut.ReadFromFile(destination);
                            if (!string.IsNullOrWhiteSpace(shortcut.Target))
                            {
                                // If we drop to a place where it's occupied by a folder in the desktop, then we move/copy item in this folder
                                // But this time, item to drop does not come from desktop, but another place (like explorer)
                                if (Directory.Exists(shortcut.Target))
                                    destination = shortcut.Target;
                                else
                                {
                                    // If we drop to a place where it's occupied by an item that it's not a folder, then we launch it with the item as parameter (like, for example ; drop txt item file in a shortcut of notepad)
                                    // But this time, item to drop does not come from desktop, but another place (like explorer)
                                    dest.DataContext.ExecuteCommand.Execute("\"" + fs + "\"");
                                    continue;
                                }
                            }
                        }
                        // Execute file operation (move/copy) from another place (like explorer) to a desktop
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
    }

    internal static void GiveFeedback(GiveFeedbackEventArgs e)
    {
        DragGhostAdorner.UpdateDragGhost();
        e.UseDefaultCursors = false;
        e.Handled = true;
    }

    internal static void DragOver(DragEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            e.Effects = DragDropEffects.Copy;
        else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            e.Effects = DragDropEffects.Link;
        else
            e.Effects = e.AllowedEffects.HasFlag(DragDropEffects.Move) ? DragDropEffects.Move : DragDropEffects.Copy;
        e.Handled = true;
    }

    #endregion

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
                {
                    foreach (OneDesktopItem item in ListItems())
                        item.DataContext?.Dispose();
                    _parentDesktop.MainGrid.Children.Clear();
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
