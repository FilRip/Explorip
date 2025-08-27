using System;

namespace HookTaskbarList.TaskbarList.Interfaces;

[Flags()]
public enum EThumbButton : uint
{
    /// <summary>
    /// The button is disabled. It is present, but has a visual state that indicates that it will not respond to user action.
    /// </summary>
    THBF_DISABLED = 1,

    /// <summary>When the button is clicked, the taskbar button's flyout closes immediately.</summary>
    THBF_DISMISSONCLICK = 2,

    /// <summary>The button is active and available to the user.</summary>
    THBF_ENABLED = 0,

    /// <summary>The button is not shown to the user.</summary>
    THBF_HIDDEN = 8,

    /// <summary>Do not draw a button border, use only the image.</summary>
    THBF_NOBACKGROUND = 4,

    /// <summary>
    /// The button is enabled but not interactive; no pressed button state is drawn. This value is intended for instances where the
    /// button is used in a notification.
    /// </summary>
    THBF_NONINTERACTIVE = 0x10
}

