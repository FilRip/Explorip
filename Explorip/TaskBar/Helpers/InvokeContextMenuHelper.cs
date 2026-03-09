using System;
using System.IO;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

namespace Explorip.TaskBar.Helpers;

public static class InvokeContextMenuHelper
{
    private enum MenuItemId : uint
    {
        OpenParentFolder = CommonContextMenuItem.Paste + 1,
    }

    public static bool InvokeContextMenu(ShellFile file, bool isInteractive, ShellFolder parentFolder = null)
    {
        try
        {
            if (file != null)
            {
                parentFolder ??= new ShellFolder(Directory.GetParent(file.Path).FullName, IntPtr.Zero);
                _ = new ShellItemContextMenu([file], parentFolder, IntPtr.Zero, HandleFileAction, isInteractive, false, new ShellMenuCommandBuilder(), GetFileCommandBuilder(file));
                return true;
            }
        }
        finally
        {
            parentFolder?.Dispose();
        }
        return false;
    }

    private static bool HandleFileAction(string action, ShellItem[] items, bool allFolders)
    {
        if (action == ((uint)MenuItemId.OpenParentFolder).ToString() && items?.Length > 0)
        {
            ShellHelper.StartProcess(items[0].Path);
            return true;
        }

        return false;
    }

    private static ShellMenuCommandBuilder GetFileCommandBuilder(ShellFile file)
    {
        if (file == null)
            return new ShellMenuCommandBuilder();

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
