﻿using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    const string PowrProf_DllName = "powrprof.dll";

#pragma warning disable S1104 // Fields should not have public accessibility
    public struct BatteryReportingScale
    {
        public uint Granularity;
        public uint Capacity;
    }
#pragma warning restore S1104 // Fields should not have public accessibility

    public enum SYSTEM_POWER_STATE
    {
        PowerSystemUnspecified = 0,
        PowerSystemWorking = 1,
        PowerSystemSleeping1 = 2,
        PowerSystemSleeping2 = 3,
        PowerSystemSleeping3 = 4,
        PowerSystemHibernate = 5,
        PowerSystemShutdown = 6,
        PowerSystemMaximum = 7
    }

#pragma warning disable S1104 // Fields should not have public accessibility
    public struct SystemPowerCapabilities
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool PowerButtonPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool SleepButtonPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool LidPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS1;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS2;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS3;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS4;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS5;
        [MarshalAs(UnmanagedType.U1)]
        public bool HiberFilePresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool FullWake;
        [MarshalAs(UnmanagedType.U1)]
        public bool VideoDimPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool ApmPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool UpsPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool ThermalControl;
        [MarshalAs(UnmanagedType.U1)]
        public bool ProcessorThrottle;
        public byte ProcessorMinThrottle;
        public byte ProcessorMaxThrottle;    // Also known as ProcessorThrottleScale before Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool FastSystemS4;   // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool Hiberboot;  // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool WakeAlarmPresent;   // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool AoAc;   // Ignore if earlier than Windows XP
        [MarshalAs(UnmanagedType.U1)]
        public bool DiskSpinDown;
        public byte HiberFileType;  // Ignore if earlier than Windows 10 (10.0.10240.0)
        [MarshalAs(UnmanagedType.U1)]
        public bool AoAcConnectivitySupported;  // Ignore if earlier than Windows 10 (10.0.10240.0)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public readonly byte[] spare3;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemBatteriesPresent;
        [MarshalAs(UnmanagedType.U1)]
        public bool BatteriesAreShortTerm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public BatteryReportingScale[] BatteryScale;
        public SYSTEM_POWER_STATE AcOnLineWake;
        public SYSTEM_POWER_STATE SoftLidWake;
        public SYSTEM_POWER_STATE RtcWake;
        public SYSTEM_POWER_STATE MinDeviceWakeState;
        public SYSTEM_POWER_STATE DefaultLowLatencyWake;
    }
#pragma warning restore S1104 // Fields should not have public accessibility

    // There is a method for this in System.Windows.Forms, however it calls the same p/invoke and I would prefer not to reference that lib
    [DllImport(PowrProf_DllName)]
    internal static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

    [DllImport(PowrProf_DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static extern bool GetPwrCapabilities(out SystemPowerCapabilities systemPowerCapabilites);
}
