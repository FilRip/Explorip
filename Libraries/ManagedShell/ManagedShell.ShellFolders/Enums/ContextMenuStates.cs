using System;

namespace ManagedShell.ShellFolders.Enums;

// Specifies how the shortcut menu can be changed when calling IContextMenu::QueryContextMenu
#pragma warning disable IDE0079
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
[Flags()]
public enum ContextMenuStates : uint
{
    NORMAL = 0x00000000,
    DEFAULTONLY = 0x00000001,
    VERBSONLY = 0x00000002,
    EXPLORE = 0x00000004,
    NOVERBS = 0x00000008,
    CANRENAME = 0x00000010,
    NODEFAULT = 0x00000020,
    INCLUDESTATIC = 0x00000040,
    ITEMMENU = 0x00000080,
    EXTENDEDVERBS = 0x00000100,
    RESERVED = 0xffff0000
}
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"
#pragma warning restore IDE0079
