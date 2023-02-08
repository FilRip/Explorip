using System;
using System.Runtime.InteropServices;

using ManagedShell.Common.Exceptions;
using ManagedShell.Interop;

namespace ManagedShell.Common.Helpers
{
    public static class PowerHelper
    {
        static NativeMethods.SystemPowerCapabilities spc;
        static bool hasFetchedCapabilities = false;

        /// <summary>
        /// Adjusts the current process's token privileges to allow it to shut down or reboot the machine.
        /// Throws an ApplicationException if an error is encountered.
        /// </summary>
        private static void AdjustTokenPrivilegesForShutdown()
        {
            IntPtr procHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

            bool tokenOpenResult = NativeMethods.OpenProcessToken(procHandle, NativeMethods.TOKENADJUSTPRIVILEGES | NativeMethods.TOKENQUERY, out IntPtr tokenHandle);
            if (!tokenOpenResult)
            {
                throw new ManagedShellException("Error attempting to open process token to raise level for shutdown.\nWin32 Error Code: " + Marshal.GetLastWin32Error());
            }

            NativeMethods.Luid luid = new();
            bool privLookupResult = NativeMethods.LookupPrivilegeValue(null, "SeShutdownPrivilege", ref luid);
            if (!privLookupResult)
            {
                throw new ManagedShellException("Error attempting to lookup value for shutdown privilege.\n Win32 Error Code: " + Marshal.GetLastWin32Error());
            }

            NativeMethods.TokenPriviles newPriv = new()
            {
                PrivilegeCount = 1,
                Privileges = new NativeMethods.LuidAndAttributes[1]
            };
            newPriv.Privileges[0].Luid = luid;
            newPriv.Privileges[0].Attributes = 0x00000002;

            bool tokenPrivResult = NativeMethods.AdjustTokenPrivileges(tokenHandle, false, ref newPriv, 0, IntPtr.Zero, IntPtr.Zero);
            if (!tokenPrivResult)
            {
                throw new ManagedShellException("Error attempting to adjust the token privileges to allow shutdown.\n Win32 Error Code: " + Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Calls the shutdown method on the Win32 API.
        /// </summary>
        public static void Shutdown()
        {
            AdjustTokenPrivilegesForShutdown();
            NativeMethods.ExitWindowsEx((uint)(NativeMethods.ExitWindows.Shutdown | NativeMethods.ExitWindows.ForceIfHung), 0x40000000);
        }

        /// <summary>
        /// Calls the reboot method on the Win32 API.
        /// </summary>
        public static void Reboot()
        {
            AdjustTokenPrivilegesForShutdown();
            NativeMethods.ExitWindowsEx((uint)(NativeMethods.ExitWindows.Reboot | NativeMethods.ExitWindows.ForceIfHung), 0x40000000);
        }

        /// <summary>
        /// Calls the Sleep method on the Win32 Power Profile API.
        /// </summary>
        public static void Sleep()
        {
            NativeMethods.SetSuspendState(false, false, false);
        }

        /// <summary>
        /// Calls the Hibernate method on the Win32 Power Profile API.
        /// </summary>
        public static void Hibernate()
        {
            NativeMethods.SetSuspendState(true, false, false);
        }

        private static void FetchCapabilities()
        {
            if (!hasFetchedCapabilities)
            {
                NativeMethods.GetPwrCapabilities(out spc);
                hasFetchedCapabilities = true;
            }
        }

        /// <summary>
        /// Returns true if the system supports hibernation.
        /// </summary>
        public static bool CanHibernate()
        {
            FetchCapabilities();

            return spc.HiberFilePresent && spc.SystemS4;
        }

        /// <summary>
        /// Returns true if the system supports sleep.
        /// </summary>
        public static bool CanSleep()
        {
            FetchCapabilities();

            return spc.SystemS3 || spc.SystemS2 || spc.SystemS1;
        }
    }
}
