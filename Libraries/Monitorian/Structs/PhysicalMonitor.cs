using System;
using System.Runtime.InteropServices;

namespace Monitorian.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct PhysicalMonitor
{
    public IntPtr hPhysicalMonitor;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szPhysicalMonitorDescription;
}
