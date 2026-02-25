using System;
using System.Runtime.InteropServices;

using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport()]
[Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItemImageFactory
{
    int GetImage(
        [In(), MarshalAs(UnmanagedType.Struct)] Size size,
        [In()] ShellItemImageGetBitmaps flags,
        [Out()] out IntPtr phbm);
}

[StructLayout(LayoutKind.Sequential)]
public struct Size(int cx, int cy)
{
    public int cx = cx;
    public int cy = cy;
}
