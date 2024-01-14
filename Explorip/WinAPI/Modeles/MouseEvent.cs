using System;

namespace Explorip.WinAPI.Modeles;

[Flags()]
public enum MouseEvent
{
    MOUSE_MOVED = 0x1,
    DOUBLE_CLICK = 0x2,
    MOUSE_WHEELED = 0x4,
    MOUSE_HWHEELED = 0x8
}
