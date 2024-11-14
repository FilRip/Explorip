using System;

namespace ManagedShell.ShellFolders.Enums;

[Flags()]
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
public enum FCW : uint
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"
{
    /// <summary>The browser's media bar.</summary>
    FCW_INTERNETBAR = 0x0006,

    /// <summary>The browser's progress bar.</summary>
    FCW_PROGRESS = 0x0008,

    /// <summary>The browser's status bar.</summary>
    FCW_STATUS = 0x0001,

    /// <summary>The browser's toolbar.</summary>
    FCW_TOOLBAR = 0x0002,

    /// <summary>The browser's tree view.</summary>
    FCW_TREE = 0x0003,
}
