using System;

namespace ManagedShell.ShellFolders.Enums;

[Flags()]
public enum TBSTATE : byte
{
    /// <summary>The button has the TBSTYLE_CHECK style and is being clicked.</summary>
    TBSTATE_CHECKED = 0x01,

    /// <summary>Version 4.70. The button's text is cut off and an ellipsis is displayed.</summary>
    TBSTATE_ELLIPSES = 0x40,

    /// <summary>The button accepts user input. A button that does not have this state is grayed.</summary>
    TBSTATE_ENABLED = 0x04,

    /// <summary>The button is not visible and cannot receive user input.</summary>
    TBSTATE_HIDDEN = 0x08,

    /// <summary>The button is grayed.</summary>
    TBSTATE_INDETERMINATE = 0x10,

    /// <summary>Version 4.71. The button is marked. The interpretation of a marked item is dependent upon the application.</summary>
    TBSTATE_MARKED = 0x80,

    /// <summary>The button is being clicked.</summary>
    TBSTATE_PRESSED = 0x02,

    /// <summary>The button is followed by a line break. The button must also have the TBSTATE_ENABLED state.</summary>
    TBSTATE_WRAP = 0x20,
}
