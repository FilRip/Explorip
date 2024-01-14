using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    const string AdvApi32_DllName = "advapi32.dll";
    public const uint TOKENADJUSTPRIVILEGES = 0x00000020;
    public const uint TOKENQUERY = 0x00000008;

    /// <summary>
    /// Structure for the token privileges request.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TokenPriviles
    {
        /// <summary>
        /// The number of privileges.
        /// </summary>
        public int PrivilegeCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public LuidAndAttributes[] Privileges;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LuidAndAttributes
    {
        public Luid Luid;
        public uint Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Luid
    {
        public uint LowPart;
        public int HighPart;
    }

    [DllImport(AdvApi32_DllName, SetLastError = true)]
    internal static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

    [DllImport(AdvApi32_DllName, SetLastError = true)]
    internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, ref TokenPriviles newState, uint bufferLength, IntPtr previousState, IntPtr returnLength);

    [DllImport(AdvApi32_DllName, SetLastError = true)]
    internal static extern bool LookupPrivilegeValue(string host, string name, ref Luid pluid);
}
