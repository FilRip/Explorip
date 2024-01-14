using System;

namespace ManagedShell.ShellFolders.Enums;

// Specifies how the shortcut menu can be changed when calling IContextMenu::QueryContextMenu
[Flags()]
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
public enum CMF : uint
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"
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
