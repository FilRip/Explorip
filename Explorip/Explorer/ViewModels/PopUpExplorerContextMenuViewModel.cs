using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.HookFileOperations.FilesOperations;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

namespace Explorip.Explorer.ViewModels;

public partial class PopUpExplorerContextMenuViewModel : ObservableObject
{
    private TabItemExplorerBrowser _parentTab;
    private bool _sendMouseClick;

    [ObservableProperty()]
    private SolidColorBrush _background, _foreground;
    [ObservableProperty()]
    private bool _isOpen = true, _visibleCut, _visibleCopy, _visibleDelete, _visiblePaste;

    public void SetParentTab(TabItemExplorerBrowser parentTab)
    {
        _parentTab = parentTab;
        if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0)
        {
            VisibleCopy = true;
            VisibleCut = true;
            VisibleDelete = true;
        }
        else
            VisiblePaste = true;
    }

    partial void OnIsOpenChanged(bool value)
    {
        if (!value && _sendMouseClick)
        {
            // TODO : Send message to click
        }
    }

    public PopUpExplorerContextMenuViewModel() : base()
    {
        Background = ConfigManager.ExplorerContextMenuBackground ?? ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
        Foreground = ConfigManager.ExplorerContextMenuForeground ?? ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
    }

    [RelayCommand()]
    private void ShowOlderContextMenu()
    {
        _sendMouseClick = false;
        IsOpen = false;
        _parentTab.ShowDefaultContextMenu();
    }

    [RelayCommand()]
    private void Cut()
    {
        _sendMouseClick = false;
        IsOpen = false;
    }

    [RelayCommand()]
    private void Copy()
    {
        _sendMouseClick = false;
        IsOpen = false;
    }

    [RelayCommand()]
    private void Paste()
    {
        _sendMouseClick = false;
        IsOpen = false;
        DataObject data = (DataObject)Clipboard.GetDataObject();
        string[] listItems = (string[])data.GetData(DataFormats.FileDrop);
        byte[] buffer = new byte[4];
        bool move = false;
        if (((MemoryStream)Clipboard.GetData("Preferred DropEffect")).Read(buffer, 0, 4) > 0)
        {
            int dropEffect = BitConverter.ToInt32(buffer, 0);
            if (dropEffect == 2)
                move = true;
        }
        FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
        foreach (string fs in listItems)
            if (move)
                fileOp.MoveItem(fs, _parentTab.DataContext.EditPath, Path.GetFileName(fs));
            else
                fileOp.CopyItem(fs, _parentTab.DataContext.EditPath, Path.GetFileName(fs));
        fileOp.ChangeOperationFlags(EFileOperation.FOF_RENAMEONCOLLISION |
            EFileOperation.FOF_NOCONFIRMMKDIR |
            EFileOperation.FOFX_ADDUNDORECORD);
        fileOp.PerformOperations();
        fileOp.Dispose();
    }

    [RelayCommand()]
    private void Delete()
    {
        _sendMouseClick = false;
        IsOpen = false;
        FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
        foreach (Microsoft.WindowsAPICodePack.Shell.Common.ShellObject fs in _parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems)
            fileOp.DeleteItem(fs.ParsingName);
        fileOp.ChangeOperationFlags(EFileOperation.FOF_RENAMEONCOLLISION |
            EFileOperation.FOF_NOCONFIRMMKDIR |
            EFileOperation.FOFX_ADDUNDORECORD);
        fileOp.PerformOperations();
        fileOp.Dispose();
    }
}
