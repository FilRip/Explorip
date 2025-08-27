using System;

namespace HookTaskbarList.TaskbarList.Interfaces;

[Flags()]
public enum ThumbButtonMask : uint
{
    /// <summary>The iBitmap member contains valid information.</summary>
    THB_BITMAP = 1,

    /// <summary>The dwFlags member contains valid information.</summary>
    THB_FLAGS = 8,

    /// <summary>The hIcon member contains valid information.</summary>
    THB_ICON = 2,

    /// <summary>The szTip member contains valid information.</summary>
    THB_TOOLTIP = 4
}
