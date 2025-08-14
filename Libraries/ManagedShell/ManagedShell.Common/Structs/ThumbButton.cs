using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Common.Structs;

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

[StructLayout(LayoutKind.Sequential)]
public struct ThumbButton
{
    /// <summary>
    /// A combination of THUMBBUTTONMASK values that specify which members of this structure contain valid data; other members are
    /// ignored, with the exception of iId, which is always required.
    /// </summary>
    public ThumbButtonMask dwMask;

    /// <summary>The application-defined identifier of the button, unique within the toolbar.</summary>
    public uint iId;

    /// <summary>The zero-based index of the button image within the image list set through ITaskbarList3::ThumbBarSetImageList.</summary>
    public uint iBitmap;

    /// <summary>The handle of an icon to use as the button image.</summary>
    public IntPtr hIcon;

    /// <summary>
    /// A wide character array that contains the text of the button's tooltip, displayed when the mouse pointer hovers over the button.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szTip;

    /// <summary>A combination of THUMBBUTTONFLAGS values that control specific states and behaviors of the button.</summary>
    public EThumbButton dwFlags;

    /// <summary>The default</summary>
    public static readonly ThumbButton Default = new() { dwMask = ThumbButtonMask.THB_FLAGS, dwFlags = EThumbButton.THBF_HIDDEN };
}
