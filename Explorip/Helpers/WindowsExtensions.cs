using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using ManagedShell.Interop;

namespace Explorip.Helpers;

internal static class WindowsExtensions
{
    internal static List<IntPtr> ListWindowsOfProcess(uint processId)
    {
        List<IntPtr> windows = [];
        NativeMethods.EnumWindows(new NativeMethods.EnumWindowDelegate((handle, param) =>
        {
            NativeMethods.GetWindowThreadProcessId(handle, out uint id);
            if (id == processId)
                windows.Add(handle);
            return true;
        }), 0);
        return windows;
    }

    internal static List<int> ListChildProcess(int processId, string processName = null)
    {
        List<int> listProcess = [];
        foreach (Process process in Process.GetProcesses().Where(p => processName == null || p.ProcessName == Path.GetFileNameWithoutExtension(processName)))
        {
            try
            {
                NativeMethods.ProcessBasicInformation psi = new();
                NativeMethods.NtQueryInformationProcess(process.Handle, NativeMethods.ProcessInfoClass.ProcessBasicInformation, ref psi, Marshal.SizeOf<NativeMethods.ProcessBasicInformation>(), out _);
                if ((int)psi.InheritedFromUniqueProcessId == processId)
                    listProcess.Add(process.Id);
            }
            catch (Exception) { /* Ignore errors */ }
        }
        return listProcess;
    }
}
