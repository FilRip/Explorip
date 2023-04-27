using System.Runtime.InteropServices;

using ConsoleControlAPI;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KeyEventRecord
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
        public bool bKeyDown;
        [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
        public ushort wRepeatCount;
        [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
        public ManagedShell.Interop.NativeMethods.VK wVirtualKeyCode;
        [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
        public ushort wVirtualScanCode;
        [FieldOffset(10)]
        public char UnicodeChar;
        [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
        public ControlKeyState dwControlKeyState;
    }
}
