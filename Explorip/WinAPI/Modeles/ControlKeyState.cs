using System;

namespace Explorip.WinAPI.Modeles
{
    [Flags()]
    public enum ControlKeyState
    {
        RIGHT_ALT_PRESSED = 0x1,
        LEFT_ALT_PRESSED = 0x2,
        RIGHT_CTRL_PRESSED = 0x4,
        LEFT_CTRL_PRESSED = 0x8,
        SHIFT_PRESSED = 0x10,
        NUMLOCK_ON = 0x20,
        SCROLLLOCK_ON = 0x40,
        CAPSLOCK_ON = 0x80,
        ENHANCED_KEY = 0x100
    }
}
