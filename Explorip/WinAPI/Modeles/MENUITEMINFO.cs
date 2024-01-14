using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
[Flags()]
public enum MFT : uint
{
    GRAYED = 0x00000003,
    DISABLED = 0x00000003,
    BITMAP = 0x00000004,
    MENUBARBREAK = 0x00000020,
    MENUBREAK = 0x00000040,
    OWNERDRAW = 0x00000100,
    RADIOCHECK = 0x00000200,
    RIGHTJUSTIFY = 0x00004000,
    RIGHTORDER = 0x00002000,
    SEPARATOR = 0x00000800,
    STRING = 0x00000000,
    CHECKED = 0x00000008,
    BYCOMMAND = 0x00000000,
    BYPOSITION = 0x00000400,
    POPUP = 0x00000010,
}
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"

// Specifies the state of the new menu item
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
[Flags()]
public enum MFS : uint
{
    GRAYED = 0x00000003,
    DISABLED = 0x00000003,
    CHECKED = 0x00000008,
    HILITE = 0x00000080,
    ENABLED = 0x00000000,
    UNCHECKED = 0x00000000,
    UNHILITE = 0x00000000,
    DEFAULT = 0x00001000
}
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"

// Specifies the content of the new menu item
[Flags()]
public enum MIIM : uint
{
    STATE = 0x01,
    ID = 0x02,
    SUBMENU = 0x04,
    CHECKMARKS = 0x08,
    TYPE = 0x10,
    DATA = 0x20,
    STRING = 0x40,
    BITMAP = 0x80,
    FTYPE = 0x100,
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct MenuItemInfo
{
    public uint cbSize;
    public MIIM fMask;
    public MFT fType;
    public MFS fState;
    public uint wID;
    public IntPtr hSubMenu;
    public IntPtr hbmpChecked;
    public IntPtr hbmpUnchecked;
    public IntPtr dwItemData;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string dwTypeData;
    public int cch;
    public IntPtr hbmpItem;
}
