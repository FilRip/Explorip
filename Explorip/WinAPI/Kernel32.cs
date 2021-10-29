using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }
}
