using System;
using System.Runtime.InteropServices;

using ManagedShell.Common.Enums;

namespace ManagedShell.Common.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct TbButton
{
    /// <summary>
    /// Zero-based index of the button image. Set this member to I_IMAGECALLBACK, and the toolbar will send the TBN_GETDISPINFO
    /// notification code to retrieve the image index when it is needed.
    /// <para>
    /// Version 5.81. Set this member to I_IMAGENONE to indicate that the button does not have an image.The button layout will not
    /// include any space for a bitmap, only text.
    /// </para>
    /// <para>
    /// If the button is a separator, that is, if fsStyle is set to BTNS_SEP, iBitmap determines the width of the separator, in
    /// pixels.For information on selecting button images from image lists, see TB_SETIMAGELIST message.
    /// </para>
    /// </summary>
    public int iBitmap;

    /// <summary>
    /// Command identifier associated with the button. This identifier is used in a WM_COMMAND message when the button is chosen.
    /// </summary>
    public int idCommand;

    // Funky holder to make preprocessor directives work
    private TbButtonU union;

    /// <summary>Button state flags.</summary>
    public TbStates State { readonly get => union.fsState; set => union.fsState = value; }

    /// <summary>Button style.</summary>
    public ToolbarStyles Style { readonly get => union.fsStyle; set => union.fsStyle = value; }

    /// <summary>Application-defined value.</summary>
    public IntPtr dwData;

    /// <summary>Zero-based index of the button string, or a pointer to a string buffer that contains text for the button.</summary>
    public IntPtr iString;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private struct TbButtonU
    {
        [FieldOffset(0)] private readonly IntPtr bReserved;
        [FieldOffset(0)] public TbStates fsState;
        [FieldOffset(1)] public ToolbarStyles fsStyle;
    }
}
