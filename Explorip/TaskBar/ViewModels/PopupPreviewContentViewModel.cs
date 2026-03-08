using System;
using System.Collections.ObjectModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

namespace Explorip.TaskBar.ViewModels;

public partial class PopupPreviewContentViewModel : ObservableObject
{
    [ObservableProperty()]
    private ObservableCollection<ShellFile> _listItems;
    [ObservableProperty()]
    private bool _visible;
    [ObservableProperty()]
    private ShellFile _selectedItem;

    public bool DisposeItems { get; set; }

    public PopupPreviewContentViewModel()
    {
        ListItems = [];
    }

    partial void OnVisibleChanging(bool value)
    {
        if (Visible && !value && DisposeItems)
            foreach (ShellFile sf in ListItems)
                sf.Dispose();
    }

    [RelayCommand()]
    private void Launch()
    {
        InvokeContextMenu(SelectedItem, false, new ShellFolder(Path.GetDirectoryName(SelectedItem.Path), IntPtr.Zero));
    }

    [RelayCommand()]
    private void ContextMenu()
    {
        InvokeContextMenu(SelectedItem, true, new ShellFolder(Path.GetDirectoryName(SelectedItem.Path), IntPtr.Zero));
    }

    private void InvokeContextMenu(ShellFile file, bool isInteractive, ShellFolder parentFolder)
    {
        if (file == null)
            return;

        _ = new ShellItemContextMenu([file], parentFolder, IntPtr.Zero, HandleFileAction, isInteractive, false, new ShellMenuCommandBuilder(), GetFileCommandBuilder(file));
        parentFolder.Dispose();
    }

    private enum MenuItemId : uint
    {
        OpenParentFolder = CommonContextMenuItem.Paste + 1,
    }

    private static bool HandleFileAction(string action, ShellItem[] items, bool allFolders)
    {
        if (action == ((uint)MenuItemId.OpenParentFolder).ToString())
        {
            ShellHelper.StartProcess(items[0].Path);
            return true;
        }

        return false;
    }

    private ShellMenuCommandBuilder GetFileCommandBuilder(ShellFile file)
    {
        if (file == null)
        {
            return new ShellMenuCommandBuilder();
        }

        ShellMenuCommandBuilder builder = new();

        builder.AddSeparator();
        builder.AddCommand(new ShellMenuCommand()
        {
            Flags = MenuFlagsTypes.BYCOMMAND,
            Label = Constants.Localization.OPEN_FOLDER,
            UID = (uint)MenuItemId.OpenParentFolder,
        });

        return builder;
    }
}
