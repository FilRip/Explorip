using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    public enum ProcessInfoClass
    {
        ProcessBasicInformation = 0x00,
        ProcessDebugPort = 0x07,
        ProcessExceptionPort = 0x08,
        ProcessAccessToken = 0x09,
        ProcessWow64Information = 0x1A,
        ProcessImageFileName = 0x1B,
        ProcessDebugObjectHandle = 0x1E,
        ProcessDebugFlags = 0x1F,
        ProcessExecuteFlags = 0x22,
        ProcessInstrumentationCallback = 0x28,
        MaxProcessInfoClass = 0x64,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessBasicInformation
    {
        public IntPtr Reserved1;
        public IntPtr PebAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniquePid;
        public IntPtr InheritedFromUniqueProcessId;
    }

    [DllImport("NTDLL.DLL", SetLastError = true)]
    internal static extern int NtQueryInformationProcess(IntPtr hProcess, ProcessInfoClass pic, ref ProcessBasicInformation pbi, int cb, out int pSize);
}
