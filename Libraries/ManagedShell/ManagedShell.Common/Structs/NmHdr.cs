using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Common.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct NmHdr
{
    /// <summary>A window handle to the control sending the message.</summary>
    public IntPtr hwndFrom;

    /// <summary>An identifier of the control sending the message.</summary>
    public IntPtr idFrom;

    /// <summary>
    /// A notification code. This member can be one of the common notification codes (see Notifications under General Control
    /// Reference), or it can be a control-specific notification code.
    /// </summary>
    public int code;
}
