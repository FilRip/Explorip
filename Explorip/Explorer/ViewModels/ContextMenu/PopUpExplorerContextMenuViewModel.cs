using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls.ContextMenu;
using Explorip.Explorer.Controls.Tabs;
using Explorip.Explorer.Helpers;
using Explorip.Explorer.Helpers.ContextMenu;
using Explorip.HookFileOperations.FilesOperations;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.ViewModels;

public partial class PopUpExplorerContextMenuViewModel : ObservableObject
{
    private TabItemExplorerBrowser _parentTab;
    private ShellObject[] _listSelected;
    private ShellFolder _parentFolder;
    private bool _backgroundContextMenu;
    private readonly Thread _threadBuildContextMenu;
    [ObservableProperty()]
    private SolidColorBrush _foregroundCut, _foregroundCopy, _foregroundPaste, _foregroundDelete;
    [ObservableProperty()]
    private SolidColorBrush _background, _foreground;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(VisibleIcons))]
    private bool _visibleCut, _visibleCopy, _visibleDelete, _visiblePaste;
    [ObservableProperty()]
    private double _dpi;
    [ObservableProperty()]
    private ObservableCollection<ContextMenuEntryViewModel> _listContextMenuEntry;

    public PopUpExplorerContextMenuSubItems Popup { get; set; }

    public TabItemExplorerBrowser ParentTab
    {
        get { return _parentTab; }
    }

    public Action Close { get; set; }

    public void SetParentTab(TabItemExplorerBrowser parentTab)
    {
        _parentTab = parentTab;
        Dpi = VisualTreeHelper.GetDpi(parentTab).DpiScaleX;
    }

    public void SetSelected(ShellObject[] selectedItems, ShellFolder parentFolder, bool background = false)
    {
        _backgroundContextMenu = background;
        _listSelected = selectedItems;
        _parentFolder = parentFolder;
        if (_listSelected?.Length > 0)
        {
            VisibleCopy = !background;
            VisibleCut = !background;
            VisibleDelete = !background;
            if (_listSelected.Length == 1 &&
                _listSelected[0] is ShellFolder &&
                Clipboard.ContainsFileDropList())
            {
                VisiblePaste = true;
            }
            _threadBuildContextMenu.Start();
        }
        else
            VisiblePaste = true;
    }

    private void BuildContextMenu()
    {
        try
        {
            bool onlyFolder = !_listSelected.Any(so => so is not ShellFolder);
#pragma warning disable S3358
            ESourceType sourceType = (onlyFolder ? (_listSelected.Length == 1 ? ESourceType.Folder : ESourceType.MultipleFolders) :
                                                   (_listSelected.Length == 1 ? ESourceType.File : ESourceType.MultipleFiles));
#pragma warning restore S3358
            List<ShellContextMenuEntry> listToBuild = ExtensionsContextMenu.GetAllCommands(sourceType, (_listSelected.Length == 1 && sourceType == ESourceType.File ? Path.GetExtension(_listSelected[0].ParsingName) : ""));
            for (int i = listToBuild.Count - 1; i >= 0; i--)
                listToBuild[i].ExpandIt(ref listToBuild, _parentFolder, ListSelected);
            foreach (ShellContextMenuEntry entry in listToBuild)
            {
                ContextMenuEntryViewModel vm = new(entry, this, null);
                vm.SubItems = [.. ExpandSubItems(vm)];
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    ListContextMenuEntry.Add(vm);
                });
            }

            ContextMenuEntryViewModel[] ExpandSubItems(ContextMenuEntryViewModel item)
            {
                List<ContextMenuEntryViewModel> result = [];
                if (item.Entry.Subitems?.Count > 0)
                    foreach (ShellContextMenuEntry subItem in item.Entry.Subitems)
                    {
                        result.Add(new ContextMenuEntryViewModel(subItem, this, item));
                        if (subItem.Subitems?.Count > 0)
                            result[result.Count - 1].SubItems = [.. ExpandSubItems(result[result.Count - 1])];
                    }
                return [.. result];
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    public PopUpExplorerContextMenuViewModel() : base()
    {
        Background = ConfigManager.ExplorerContextMenuBackground ?? ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;
        Foreground = ConfigManager.ExplorerContextMenuForeground ?? ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
        _listContextMenuEntry = [];
        _threadBuildContextMenu = new Thread(new ThreadStart(BuildContextMenu));
        _foregroundDelete = ConfigManager.ExplorerContextMenuDeleteColor;
        _foregroundCut = ConfigManager.ExplorerContextMenuCutColor;
        _foregroundCopy = ConfigManager.ExplorerContextMenuCopyColor;
        _foregroundPaste = ConfigManager.ExplorerContextMenuPasteColor;
    }

    [RelayCommand()]
    private void ShowOlderContextMenu()
    {
        ForceClose();
        _parentTab.ShowDefaultContextMenu();
    }

    public bool VisibleIcons
    {
        get { return VisibleCopy || VisibleCut || VisibleDelete || VisiblePaste; }
    }

    [RelayCommand()]
    private void Cut()
    {
        ForceClose();
        if (_listSelected?.Length > 0)
        {
            List<string> list = [];
            foreach (ShellObject so in _listSelected)
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
        if (_listSelected?.Length > 0)
        {
            List<string> list = [];
            foreach (ShellObject so in _listSelected)
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
                fileOp.MoveItem(fs, _parentTab.CurrentDirectory.ParsingName, Path.GetFileName(fs));
            else
                fileOp.CopyItem(fs, _parentTab.CurrentDirectory.ParsingName, Path.GetFileName(fs));
        fileOp.PerformOperations();
        fileOp.Dispose();
    }

    [RelayCommand()]
    private void Delete()
    {
        ForceClose();
        FileOperation fileOp = new(NativeMethods.GetDesktopWindow());
        foreach (ShellObject fs in _listSelected)
            fileOp.DeleteItem(fs.ParsingName);
        fileOp.PerformOperations();
        fileOp.Dispose();
    }

    public void ForceClose()
    {
        if (_threadBuildContextMenu != null && _threadBuildContextMenu.ThreadState == ThreadState.Running)
            _threadBuildContextMenu.Abort();
        try
        {
            Close.Invoke();
        }
        catch (Exception) { /* Ignore errors, window certainly already closed */ }
    }

    [RelayCommand()]
    private void GetProperties()
    {
        if (_listSelected?.Length > 0)
        {
            if (_listSelected.Length == 1)
                ManagedShell.Common.Helpers.ShellHelper.ShowFileProperties(_listSelected[0].ParsingName);
            else
                ManagedShell.Common.Helpers.ShellHelper.ShowFileProperties(_parentTab.CurrentDirectory.ParsingName, _listSelected.Select(so => so.ParsingName));
        }
        ForceClose();
    }

    public ShellObject[] ListSelected
    {
        get { return _listSelected; }
    }

    public string GetSelectedAsArguments()
    {
        StringBuilder args = new();
        foreach (ShellObject so in ListSelected)
        {
            if (args.Length > 0)
                args.Append(' ');
            args.Append('\"');
            args.Append(so.ParsingName);
            args.Append('\"');
        }
        return args.ToString();
    }

    public bool BackgroundContextMenu
    {
        get { return _backgroundContextMenu; }
    }

    public ShellFolder ParentFolder
    {
        get { return _parentFolder; }
    }

    [RelayCommand()]
    private void MouseEnter()
    {
        Popup?.Close();
        Popup = null;
    }
}
