using System;

namespace ManagedShell.ShellFolders.Enums;

[Flags()]
public enum FolderCommandTypes : uint
{
    /// <summary>
    /// Merge the toolbar items instead of replacing all of the buttons with those provided by the view. This is the recommended choice.
    /// </summary>
    FCT_MERGE = 0x0001,

    /// <summary>Not implemented.</summary>
    FCT_CONFIGABLE = 0x0002,

    /// <summary>Add at the right side of the toolbar.</summary>
    FCT_ADDTOEND = 0x0004,
}
