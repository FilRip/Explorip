using System;
using System.Runtime.InteropServices;

namespace Filexplorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr Reserved1;
        public IntPtr PebAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniquePid;
        public IntPtr InheritedFromUniqueProcessId;
    }
}
