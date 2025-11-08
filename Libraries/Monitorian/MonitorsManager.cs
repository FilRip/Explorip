using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using Monitorian.Enums;
using Monitorian.Models;
using Monitorian.Structs;

using static Monitorian.NativeMethods;

///
/// Partial code from Monitorian lib at https://github.com/emoacht/Monitorian.git
///

namespace Monitorian;

public static class MonitorsManager
{
    public static List<PhysicalItem> MonitorsList { get; set; } = null;

    public static void Clean()
    {
        if (MonitorsList?.Count > 0)
            foreach (SafePhysicalMonitorHandle mon in MonitorsList.Select(pi => pi.Handle))
                mon.Dispose();
        MonitorsList = null;
    }

    public static int NumberOfMonitorsOff(bool withCapabilities = false, bool forceRefreshMonitorList = false)
    {
        if (MonitorsList == null || forceRefreshMonitorList)
        {
            Clean();
            MonitorsList = [];
            HandleItem[] hMonitors = GetMonitorHandles();
            if (hMonitors.Length > 0)
                foreach (HandleItem hMon in hMonitors)
                    MonitorsList.AddRange(EnumeratePhysicalMonitors(hMon.MonitorHandle, withCapabilities: withCapabilities));
        }

        int result = 0;

        if (MonitorsList.Count == 0)
            return 0;

        foreach (SafePhysicalMonitorHandle physicalMonitorHandler in MonitorsList.Select(pm => pm.Handle))
        {
            if (GetCapabilitiesStringLength(physicalMonitorHandler, out uint size))
            {
                if (withCapabilities)
                {
                    StringBuilder sb = new((int)size);
                    bool on = CapabilitiesRequestAndCapabilitiesReply(physicalMonitorHandler, sb, size);
                    if (!on)
                        result++;
                }
            }
            else
                result++;
        }
        return result;
    }

    public static MonitorCapability GetMonitorCapability(SafePhysicalMonitorHandle physicalMonitorHandle, bool verbose)
    {
        bool isHighLevelSupported = GetMonitorCapabilities(
            physicalMonitorHandle,
            out McCaps caps,
            out _)
            && caps.HasFlag(McCaps.BRIGHTNESS);

        if (GetCapabilitiesStringLength(
            physicalMonitorHandle,
            out uint capabilitiesStringLength))
        {
            StringBuilder buffer = new((int)capabilitiesStringLength);

            if (CapabilitiesRequestAndCapabilitiesReply(
                physicalMonitorHandle,
                buffer,
                capabilitiesStringLength))
            {
                string capabilitiesString = buffer.ToString();
                Dictionary<byte, byte[]> vcpCodes = ExtractVcpCodes(capabilitiesString);

                return new MonitorCapability(
                    isHighLevelBrightnessSupported: isHighLevelSupported,
                    isLowLevelBrightnessSupported: vcpCodes.ContainsKey((byte)VcpCode.Luminance),
                    isContrastSupported: vcpCodes.ContainsKey((byte)VcpCode.Contrast),
                    capabilitiesCodes: FilterVcpCodes(vcpCodes),
                    capabilitiesString: (verbose ? capabilitiesString : null),
                    capabilitiesReport: (verbose ? MakeCapabilitiesReport(vcpCodes) : null),
                    capabilitiesData: (verbose && !vcpCodes.Any() ? GetCapabilitiesData(physicalMonitorHandle, capabilitiesStringLength) : null));
            }
        }
        return new MonitorCapability(
            isHighLevelBrightnessSupported: isHighLevelSupported,
            isLowLevelBrightnessSupported: false,
            isContrastSupported: false);

        static string MakeCapabilitiesReport(IReadOnlyDictionary<byte, byte[]> vcpCodeValues)
        {
            return $"Luminance: {vcpCodeValues.ContainsKey((byte)VcpCode.Luminance)}, " +
                   $"Contrast: {vcpCodeValues.ContainsKey((byte)VcpCode.Contrast)}, " +
                   $"Temperature: {vcpCodeValues.ContainsKey((byte)VcpCode.Temperature)}, " +
                   $"Input Source: {vcpCodeValues.ContainsKey((byte)VcpCode.InputSource)}, " +
                   $"Speaker Volume: {vcpCodeValues.ContainsKey((byte)VcpCode.SpeakerVolume)}, " +
                   $"Power Mode: {vcpCodeValues.ContainsKey((byte)VcpCode.PowerMode)}";
        }
#nullable enable
        static byte[]? GetCapabilitiesData(SafePhysicalMonitorHandle physicalMonitorHandle, uint capabilitiesStringLength)
        {
            IntPtr dataPointer = IntPtr.Zero;
            try
            {
                dataPointer = Marshal.AllocHGlobal((int)capabilitiesStringLength);

                if (CapabilitiesRequestAndCapabilitiesReply(
                    physicalMonitorHandle,
                    dataPointer,
                    capabilitiesStringLength))
                {
                    byte[] data = new byte[capabilitiesStringLength];
                    Marshal.Copy(dataPointer, data, 0, data.Length);
                    return data;
                }
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(dataPointer);
            }
        }
#nullable restore
    }

    private static Dictionary<byte, byte[]> ExtractVcpCodes(string source)
    {
        Dictionary<byte, byte[]> dic = [];

        if (string.IsNullOrEmpty(source))
            return dic;

        int index = source.IndexOf("vcp", StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return dic;

        int depth = 0;
        StringBuilder buffer1 = new(2);
        byte? lastKey = null;
        StringBuilder buffer2 = new(2);
        List<byte> values = [];
        bool exitLoop;
        foreach (char c in source.Skip(index + 3))
        {
            exitLoop = false;
            if (!IsAscii(c))
                break;
            switch (c)
            {
                case '(':
                    depth++;
                    break;
                case ')':
                    depth--;
                    switch (depth)
                    {
                        case < 1:
                            exitLoop = true;
                            break;
                        case 1:
                            if (values.Any() && lastKey.HasValue)
                            {
                                dic[lastKey.Value] = [.. values];
                                values.Clear();
                            }
                            break;
                    }
                    break;
                default:
                    switch (depth)
                    {
                        case 1:
                            if (IsHexNumber(c))
                            {
                                buffer1.Append(c);
                                if (buffer1.Length == 1)
                                    continue;
                            }
                            if (0 < buffer1.Length)
                            {
                                lastKey = byte.Parse(buffer1.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                                buffer1.Clear();
                                dic[lastKey.Value] = null;
                            }
                            break;
                        case 2:
                            if (IsHexNumber(c))
                            {
                                buffer2.Append(c);
                                if (buffer2.Length == 1)
                                    continue;
                            }
                            if (0 < buffer2.Length)
                            {
                                byte value = byte.Parse(buffer2.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                                buffer2.Clear();
                                values.Add(value);
                            }
                            break;
                    }
                    break;
            }
            if (exitLoop)
                break;
        }
        return dic;

        static bool IsAscii(char c) => c <= 0x7F;
        static bool IsHexNumber(char c) => c is (>= '0' and <= '9') or (>= 'A' and <= 'F') or (>= 'a' and <= 'f');
    }

    private static Dictionary<byte, byte[]> FilterVcpCodes(Dictionary<byte, byte[]> dic)
    {
        if (dic.TryGetValue((byte)VcpCode.Temperature, out byte[] values)
            && (values is not null))
        {
            // The following color temperatures are defined.
            //  3:  4000° K
            //  4:  5000° K
            //  5:  6500° K
            //  6:  7500° K
            //  7:  8200° K
            //  8:  9300° K
            //  9: 10000° K
            // 10: 11500° K
            // Not all temperatures are supported in practice.
            // An additional temperature can be inserted depending on a specific model.
            dic[(byte)VcpCode.Temperature] = [.. values.Clip<byte>(3, 10)]; // 3 is warmest and 10 is coldest.
        }
        return dic;
    }

    public static HandleItem[] GetMonitorHandles()
    {
        List<HandleItem> handleItems = [];

        if (EnumDisplayMonitors(
            IntPtr.Zero,
            IntPtr.Zero,
            Proc,
            IntPtr.Zero))
        {
            return [.. handleItems];
        }
        return [];

        bool Proc(IntPtr monitorHandle, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData)
        {
            MonitorInfoEx monitorInfo = new() { cbSize = (uint)Marshal.SizeOf<MonitorInfoEx>() };

            if (GetMonitorInfo(monitorHandle, ref monitorInfo) &&
                TryGetDisplayIndex(monitorInfo.szDevice, out byte displayIndex))
            {
                handleItems.Add(new HandleItem(
                    displayIndex: displayIndex,
                    monitorRect: monitorInfo.rcMonitor,
                    monitorHandle: monitorHandle));
            }
            return true;
        }
    }

    private static bool TryGetDisplayIndex(string deviceName, out byte index)
    {
        // The typical format of device name is as follows:
        // EnumDisplayDevices (display), GetMonitorInfo : \\.\DISPLAY[index starting at 1]
        // EnumDisplayDevices (monitor)                 : \\.\DISPLAY[index starting at 1]\Monitor[index starting at 0]

        Match match = Regex.Match(deviceName, @"DISPLAY(?<index>\d{1,2})\s*$");
        if (match.Success)
        {
            index = byte.Parse(match.Groups["index"].Value);
            return true;
        }
        index = 0;
        return false;
    }

    public static List<PhysicalItem> EnumeratePhysicalMonitors(IntPtr monitorHandle, bool verbose = false, bool withCapabilities = false)
    {
        List<PhysicalItem> list = [];
        if (!GetNumberOfPhysicalMonitorsFromHMONITOR(
            monitorHandle,
            out uint count))
        {
            return list;
        }
        if (count == 0)
        {
            return list;
        }

        PhysicalMonitor[] physicalMonitors = new PhysicalMonitor[count];

        try
        {
            if (!GetPhysicalMonitorsFromHMONITOR(
                monitorHandle,
                count,
                physicalMonitors))
            {
                return list;
            }

            int monitorIndex = 0;

            foreach (PhysicalMonitor physicalMonitor in physicalMonitors)
            {
                SafePhysicalMonitorHandle handle = new(physicalMonitor.hPhysicalMonitor);

                list.Add(new PhysicalItem(
                    description: physicalMonitor.szPhysicalMonitorDescription,
                    monitorIndex: monitorIndex,
                    handle: handle,
                    capability: (withCapabilities ? GetMonitorCapability(handle, verbose) : null)));

                monitorIndex++;
            }
        }
        finally
        {
            // The physical monitor handles should be destroyed at a later stage.
        }
        return list;
    }
}
