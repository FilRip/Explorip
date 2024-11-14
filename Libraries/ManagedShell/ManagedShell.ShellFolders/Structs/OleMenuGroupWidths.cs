using System.Runtime.InteropServices;

namespace ManagedShell.ShellFolders.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct OleMenuGroupWidths
{
    /// <summary>
    /// An array whose elements contain the number of menu items in each of the six menu groups of a shared in-place editing menu.
    /// Each menu group can have any number of menu items. The container uses elements 0, 2, and 4 to indicate the number of menu
    /// items in its File, View, and Window menu groups. The object server uses elements 1, 3, and 5 to indicate the number of menu
    /// items in its Edit, Object, and Help menu groups.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public uint[] width;
}
