using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Interop
{
    public partial class NativeMethods
    {
        const string ShlwApi_DllName = "shlwapi.dll";

        [DllImport(ShlwApi_DllName)]
        internal static extern IntPtr SHLockShared(IntPtr hData, uint dwProcessId);

        [DllImport(ShlwApi_DllName, SetLastError = true)]
        internal static extern bool SHUnlockShared(IntPtr pvData);
    }
}
