using System.Runtime.InteropServices;

namespace ManagedShell.Common.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct NmToolBar
{
    /// <summary>
    /// Structure that contains additional information about the notification.
    /// </summary>
    public NmHdr hdr;

    /// <summary>
    /// Command identifier of the button associated with the notification code.
    /// </summary>
    public int iItem;

    /// <summary>
    /// Structure that contains information about the toolbar button associated with the notification code. This member
    /// only contains valid information with the TBN_QUERYINSERT and TBN_QUERYDELETE notification codes.
    /// </summary>
    public TbButton tbButton;

    /// <summary>
    /// Count of characters in the button text.
    /// </summary>
    public int cchText;

    /// <summary>
    /// Address of a character buffer that contains the button text.
    /// </summary>
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pszText;

    /// <summary>
    /// Version 5.80. A RECT structure that defines the area covered by the button.
    /// </summary>
    public Interop.NativeMethods.Rect rcButton;
}
