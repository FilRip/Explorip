using System;
using System.Runtime.InteropServices;

using ConsoleControlAPI;
using Explorip.WinAPI.Modeles;

namespace Explorip.WinAPI
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool WriteConsoleInput(
                IntPtr hConsoleInput,
                InputRecord[] lpBuffer,
                uint nLength,
                out uint lpNumberOfEventsWritten);

        [DllImport("kernel32.dll", EntryPoint = "AttachConsole", SetLastError = true)]
        internal static extern bool AttachConsole(int IdProcessus);

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true)]
        internal static extern bool FreeConsole();

        [DllImport("kernel32")]
        internal static extern IntPtr GetStdHandle(StdHandle index);
    }
}
