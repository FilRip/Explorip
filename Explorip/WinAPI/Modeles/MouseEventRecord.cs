using System.Runtime.InteropServices;

using ConsoleControlAPI;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseEventRecord
    {
        public Coord dwMousePosition;
        public MouseButtonState dwButtonState;
        public ControlKeyState dwControlKeyState;
        public MouseEvent dwEventFlags;
    }
}
