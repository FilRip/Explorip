using System.Runtime.InteropServices;

namespace Monitorian.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct MonitorInfoEx
{
    public uint cbSize;
    public NativeRect rcMonitor;
    public NativeRect rcWork;
    public uint dwFlags;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string szDevice;
}
