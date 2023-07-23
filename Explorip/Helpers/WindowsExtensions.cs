using System;
using System.Collections.Generic;

using ManagedShell.Interop;

namespace Explorip.Helpers
{
    internal static class WindowsExtensions
    {
        internal static List<IntPtr> ListWindowsOfProcess(uint processId)
        {
            List<IntPtr> windows = new();
            NativeMethods.EnumWindows(new NativeMethods.CallBackPtr((handle, param) =>
            {
                NativeMethods.GetWindowThreadProcessId(handle, out uint id);
                if (id == processId)
                    windows.Add(handle);
                return true;
            }), 0);
            return windows;
        }
    }
}
