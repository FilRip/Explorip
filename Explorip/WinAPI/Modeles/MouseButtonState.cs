using System;

namespace Explorip.WinAPI.Modeles;

[Flags()]
public enum MouseButtonState
{
    FROM_LEFT_1ST_BUTTON_PRESSED = 0x1,
    RIGHTMOST_BUTTON_PRESSED = 0x2,
    FROM_LEFT_2ND_BUTTON_PRESSED = 0x4,
    FROM_LEFT_3RD_BUTTON_PRESSED = 0x8,
    FROM_LEFT_4TH_BUTTON_PRESSED = 0x10
}
