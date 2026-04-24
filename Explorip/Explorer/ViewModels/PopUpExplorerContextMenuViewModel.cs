using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.Explorer.Helpers;
using Explorip.HookFileOperations.FilesOperations;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.ViewModels;

public partial class PopUpExplorerContextMenuViewModel : ObservableObject
{
    private TabItemExplorerBrowser _parentTab;
    private bool _sendMouseClick = true;

    [ObservableProperty()]
    private SolidColorBrush _background, _foreground;
    [ObservableProperty()]
    private bool _isOpen = true, _visibleCut, _visibleCopy, _visibleDelete, _visiblePaste;
    [ObservableProperty()]
    private double _dpi;

    public void SetParentTab(TabItemExplorerBrowser parentTab)
    {
        _parentTab = parentTab;
        Dpi = VisualTreeHelper.GetDpi(parentTab).DpiScaleX;
        if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0)
        {
            VisibleCopy = true;
            VisibleCut = true;
            VisibleDelete = true;
            if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.Count == 1 &&
                _parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems[0] is ShellFolder &&
                Clipboard.ContainsFileDropList())
            {
                VisiblePaste = true;
            }
        }
        else
            VisiblePaste = true;
    }

    public PopUpExplorerContextMenuViewModel() : base()
    {
        Background = ConfigManager.ExplorerContextMenuBackground ?? ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
        Foreground = ConfigManager.ExplorerContextMenuForeground ?? ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
    }

    [RelayCommand()]
    private void ShowOlderContextMenu()
    {
        ForceClose();
        _parentTab.ShowDefaultContextMenu();
    }

    [RelayCommand()]
    private void Cut()
    {
        ForceClose();
        if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0)
        {
            List<string> list = [];
            foreach (ShellObject so in _parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems)
                list.Add(so.ParsingName);
            DataObject data = new();
            data.SetFileDropList([.. list]);
            data.SetData("Preferred DropEffect", new byte[] { 2, 0, 0, 0 });
            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);
        }
    }

    [RelayCommand()]
    private void Copy()
    {
        ForceClose();
        if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0)
        {
            List<string> list = [];
            foreach (ShellObject so in _parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems)
                list.Add(so.ParsingName);
            DataObject data = new();
            data.SetFileDropList([.. list]);
            data.SetData("Preferred DropEffect", new byte[] { 0, 0, 0, 0 });
            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);
        }
    }

    [RelayCommand()]
    private void Paste()
    {
        ForceClose();
        DataObject data = (DataObject)Clipboard.GetDataObject();
        string[] listItems = (string[])data.GetData(DataFormats.FileDrop);
        bool move = false;
        if (Clipboard.GetData("Preferred DropEffect") is byte[] buffer)
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
        ForceClose();
        FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
        foreach (ShellObject fs in _parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems)
            fileOp.DeleteItem(fs.ParsingName);
        fileOp.ChangeOperationFlags(EFileOperation.FOF_RENAMEONCOLLISION |
            EFileOperation.FOF_NOCONFIRMMKDIR |
            EFileOperation.FOFX_ADDUNDORECORD);
        fileOp.PerformOperations();
        fileOp.Dispose();
    }

    public void ForceClose()
    {
        _sendMouseClick = false;
        IsOpen = false;
    }

    public void Closing(bool leftClick)
    {
        System.Drawing.Point point = new();
        NativeMethods.GetCursorPos(ref point);
        IntPtr handle = NativeMethods.GetParent(NativeMethods.WindowFromPoint(point));
        // HACK : Smulate click, because the previous click has been already used to close the popup
        if (_sendMouseClick && !IsOpen && handle == _parentTab.ExplorerBrowser.ExplorerBrowserControl.ShellViewHandle)
        {
            new Thread(() =>
            {
                Thread.Sleep(10);
                NativeMethods.Input[] inputs = new NativeMethods.Input[2];
                inputs[0].type = NativeMethods.TypeInput.Mouse;
                inputs[0].mkhi.mi = new NativeMethods.MouseInput()
                {
                    dwFlags = NativeMethods.MouseEventScans.LEFTDOWN,
                };
                inputs[1].type = NativeMethods.TypeInput.Mouse;
                inputs[1].mkhi.mi = new NativeMethods.MouseInput()
                {
                    dwFlags = NativeMethods.MouseEventScans.LEFTUP,
                };
                NativeMethods.SendInput(2, inputs, Marshal.SizeOf<NativeMethods.Input>());
                if (!leftClick)
                {
                    inputs[0].mkhi.mi.dwFlags = NativeMethods.MouseEventScans.RIGHTDOWN;
                    inputs[1].mkhi.mi.dwFlags = NativeMethods.MouseEventScans.RIGHTUP;
                }
            }).Start();
        }
    }

    [RelayCommand()]
    private void GetProperties()
    {
        if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems?.Count > 0)
        {
            if (_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.Count == 1)
                ManagedShell.Common.Helpers.ShellHelper.ShowFileProperties(_parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems[0].ParsingName);
            else
                ManagedShell.Common.Helpers.ShellHelper.ShowFileProperties(_parentTab.DataContext.EditPath, _parentTab.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.Select(so => so.ParsingName));
        }
    }
}
