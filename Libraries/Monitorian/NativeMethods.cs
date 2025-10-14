using System;
using System.Runtime.InteropServices;
using System.Text;

using Monitorian.Enums;
using Monitorian.Models;
using Monitorian.Structs;

namespace Monitorian;

internal static class NativeMethods
{
    private const string User32Dll = "user32.dll";
    private const string Dxva2Dll = "dxva2.dll";

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
        IntPtr hMonitor,
        out uint pdwNumberOfPhysicalMonitors);

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CapabilitiesRequestAndCapabilitiesReply(
        SafePhysicalMonitorHandle hMonitor,
        IntPtr pszASCIICapabilitiesString,
        uint dwCapabilitiesStringLengthInCharacters);

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CapabilitiesRequestAndCapabilitiesReply(
        SafePhysicalMonitorHandle hMonitor,

        [MarshalAs(UnmanagedType.LPStr)]
        [Out()] StringBuilder pszASCIICapabilitiesString,

        uint dwCapabilitiesStringLengthInCharacters);

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetPhysicalMonitorsFromHMONITOR(
        IntPtr hMonitor,
        uint dwPhysicalMonitorArraySize,
        [Out()] PhysicalMonitor[] pPhysicalMonitorArray);

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetMonitorCapabilities(
        SafePhysicalMonitorHandle hMonitor,
        out McCaps pdwMonitorCapabilities,
        out McSupportedColorTemperature pdwSupportedColorTemperatures);

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCapabilitiesStringLength(
        SafePhysicalMonitorHandle hMonitor,
        out uint pdwCapabilitiesStringLengthInCharacters);

    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool MonitorEnumProc(
        IntPtr hMonitor,
        IntPtr hdcMonitor,
        IntPtr lprcMonitor,
        IntPtr dwData);

    [DllImport(User32Dll)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplayMonitors(
        IntPtr hdc,
        IntPtr lprcClip,
        MonitorEnumProc lpfnEnum,
        IntPtr dwData);

    [DllImport(Dxva2Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyPhysicalMonitor(
        IntPtr hMonitor);

    [DllImport(User32Dll, EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetMonitorInfo(
        IntPtr hMonitor,
        ref MonitorInfoEx lpmi);
}
