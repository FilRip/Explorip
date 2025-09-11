using System;
using System.Runtime.InteropServices;

namespace HookTaskbarList.TaskbarList.Interfaces;

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
