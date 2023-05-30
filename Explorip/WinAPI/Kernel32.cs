using System;
using System.Runtime.InteropServices;

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

        [Flags()]
        internal enum ELoadLibrary : uint
        {
            None = 0,
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
            LOAD_LIBRARY_REQUIRE_SIGNED_TARGET = 0x00000080,
            LOAD_LIBRARY_SAFE_CURRENT_DIRS = 0x00002000,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, ELoadLibrary dwFlags);

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
