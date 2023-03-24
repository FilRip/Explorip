using System;
using System.Runtime.InteropServices;

using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders.Interfaces
{
    [ComImport()]
    [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemImageFactory
    {
        int GetImage(
            [In, MarshalAs(UnmanagedType.Struct)] Size size,
            [In()] SIIGBF flags,
            [Out] out IntPtr phbm);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public int cx;
        public int cy;

        public Size(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }
}
