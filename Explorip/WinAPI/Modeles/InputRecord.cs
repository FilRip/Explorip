using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Explicit)]
    public struct InputRecord
    {
        [FieldOffset(0)]
        public EventType EventType;
        [FieldOffset(4)]
        public KeyEventRecord KeyEvent;
        [FieldOffset(4)]
        public MouseEventRecord MouseEvent;
        [FieldOffset(4)]
        public WindowBufferSizeRecord WindowBufferSizeEvent;
        [FieldOffset(4)]
        public MenuEventRecord MenuEvent;
        [FieldOffset(4)]
        public FocusEventRecord FocusEvent;
    }
}
