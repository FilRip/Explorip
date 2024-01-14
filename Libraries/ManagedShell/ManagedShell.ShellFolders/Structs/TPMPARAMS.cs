using System.Runtime.InteropServices;

using ManagedShell.Interop;

namespace ManagedShell.ShellFolders.Structs;

// Contains extended parameters for the TrackPopupMenuEx function
#pragma warning disable IDE0044, S1144
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct TpmParams
{
    int cbSize;
    NativeMethods.Rect rcExclude;
}
#pragma warning restore IDE0044, S1144

