using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

using Microsoft.Win32.SafeHandles;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    const string Kernel32_DllName = "kernel32.dll";

    [StructLayout(LayoutKind.Explicit)]
    public struct ByHandleFileInformation
    {
        [FieldOffset(0)]
        public uint FileAttributes;

        [FieldOffset(4)]
        public FILETIME CreationTime;

        [FieldOffset(12)]
        public FILETIME LastAccessTime;

        [FieldOffset(20)]
        public FILETIME LastWriteTime;

        [FieldOffset(28)]
        public uint VolumeSerialNumber;

        [FieldOffset(32)]
        public uint FileSizeHigh;

        [FieldOffset(36)]
        public uint FileSizeLow;

        [FieldOffset(40)]
        public uint NumberOfLinks;

        [FieldOffset(44)]
        public uint FileIndexHigh;

        [FieldOffset(48)]
        public uint FileIndexLow;
    }

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool GetFileInformationByHandle(SafeFileHandle hFile,
        out ByHandleFileInformation lpFileInformation);

    [DllImport(Kernel32_DllName, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern SafeFileHandle CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename,
        [MarshalAs(UnmanagedType.U4)] FileAccess access,
        [MarshalAs(UnmanagedType.U4)] FileShare share,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
        IntPtr templateFile);

    // Handling the close splash screen event
    [DllImport(Kernel32_DllName)]
    internal static extern Int32 OpenEvent(Int32 DesiredAccess, bool InheritHandle, string Name);

    // OpenEvent DesiredAccess defines
    public const int EVENT_MODIFY_STATE = 0x00000002;

    [DllImport(Kernel32_DllName)]
    internal static extern Int32 SetEvent(Int32 Handle);

    [DllImport(Kernel32_DllName)]
    internal static extern Int32 CloseHandle(Int32 Handle);

    [DllImport(Kernel32_DllName, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWow64Process(
        [In()] IntPtr hProcess,
        [Out()] out bool wow64Process
    );

    [DllImport(Kernel32_DllName)]
    internal static extern uint GetCurrentProcessId();

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern IntPtr OpenProcess(ProcessAccess processAccess, bool bInheritHandle, int processId);

#pragma warning disable S4070
    [Flags()]
    public enum ProcessAccess : uint
    {
        All = STANDARD_RIGHTS_REQUIRED | Synchronize,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Delete = 0x00010000,
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        Synchronize = 0x00100000,
    }
#pragma warning restore S4070

    [DllImport(Kernel32_DllName, SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, [Out(), MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpExeName, ref int lpdwSize);


    [DllImport(Kernel32_DllName, SetLastError = true, ExactSpelling = true)]
    internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

    [Flags()]
    public enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags()]
    public enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        IntPtr lpBuffer,
        Int32 nSize,
        out IntPtr lpNumberOfBytesRead);

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool ReadProcessMemory(
        IntPtr hProcess,
        UIntPtr lpBaseAddress,
        IntPtr lpBuffer,
        Int32 nSize,
        out IntPtr lpNumberOfBytesRead);

    [DllImport(Kernel32_DllName, SetLastError = true, ExactSpelling = true)]
    internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
        int dwSize, AllocationType dwFreeType);

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref uint applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbAppUserModelID);

    [StructLayout(LayoutKind.Sequential)]
    public struct Coord(short X, short Y)
    {
        public short X = X;
        public short Y = Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowBufferSizeRecord(short x, short y)
    {
        public Coord dwSize = new(x, y);
    }

    public enum EventType : ushort
    {
        KEY_EVENT = 0x1,
        MOUSE_EVENT = 0x2,
    }

    [Flags()]
    public enum ControlKeyState
    {
        RIGHT_ALT_PRESSED = 0x1,
        LEFT_ALT_PRESSED = 0x2,
        RIGHT_CTRL_PRESSED = 0x4,
        LEFT_CTRL_PRESSED = 0x8,
        SHIFT_PRESSED = 0x10,
        NUMLOCK_ON = 0x20,
        SCROLLLOCK_ON = 0x40,
        CAPSLOCK_ON = 0x80,
        ENHANCED_KEY = 0x100
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KeyEventRecord
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
        public bool bKeyDown;
        [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
        public ushort wRepeatCount;
        [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
        public VK wVirtualKeyCode;
        [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
        public ushort wVirtualScanCode;
        [FieldOffset(10)]
        public char UnicodeChar;
        [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
        public ControlKeyState dwControlKeyState;
    }

    [Flags()]
    public enum MouseEvent
    {
        MOUSE_MOVED = 0x1,
        DOUBLE_CLICK = 0x2,
        MOUSE_WHEELED = 0x4,
        MOUSE_HWHEELED = 0x8
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseEventRecord
    {
        public Coord dwMousePosition;
        public MouseButtonState dwButtonState;
        public ControlKeyState dwControlKeyState;
        public MouseEvent dwEventFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MenuEventRecord
    {
        public uint dwCommandId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FocusEventRecord
    {
        public uint bSetFocus;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputRecord
    {
        [FieldOffset(0)]
        public EventType EventType;
        [FieldOffset(4)]
        public KeyEventRecord KeyEvent;
        [FieldOffset(4)]
        public MouseEventRecord MouseEvent;
        [FieldOffset(4)]
        public WindowBufferSizeRecord WindowBufferSizeEvent;
        [FieldOffset(4)]
        public MenuEventRecord MenuEvent;
        [FieldOffset(4)]
        public FocusEventRecord FocusEvent;
    }

    [DllImport(Kernel32_DllName, EntryPoint = "WriteConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool WriteConsoleInput(
            IntPtr hConsoleInput,
            InputRecord[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsWritten);

    [DllImport(Kernel32_DllName, CharSet = CharSet.Auto)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

    [DllImport(Kernel32_DllName, SetLastError = true, CharSet = CharSet.Unicode)]
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

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, ELoadLibrary dwFlags);

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool FreeLibrary(IntPtr hModule);

    [DllImport(Kernel32_DllName, EntryPoint = "AttachConsole", SetLastError = true)]
    internal static extern bool AttachConsole(int IdProcessus);

    [DllImport(Kernel32_DllName, EntryPoint = "FreeConsole", SetLastError = true)]
    internal static extern bool FreeConsole();

    public enum StdHandle
    {
        OutputHandle = -11,
        InputHandle = -10,
        ErrorHandle = -12
    }

    [DllImport(Kernel32_DllName)]
    internal static extern IntPtr GetStdHandle(StdHandle index);

    [DllImport(Kernel32_DllName)]
    internal static extern IntPtr GetConsoleWindow();

    [DllImport(Kernel32_DllName)]
    internal static extern uint GetLastError();

    [DllImport(Kernel32_DllName)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FindClose(IntPtr handle);

    [DllImport(Kernel32_DllName, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern SafeSearchHandle FindFirstFile(string fileName, out Win32FindData data);

    [DllImport(Kernel32_DllName, CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FindNextFile(SafeSearchHandle hndFindFile, out Win32FindData lpFindFileData);

    [StructLayout(LayoutKind.Sequential)]
    public struct LcId
    {
        public uint _value;
    }

    [DllImport(Kernel32_DllName, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetThreadLocale(LcId locate);

    public sealed class SafeSearchHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeSearchHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            FindClose(handle);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct Win32FindData
    {
        public FileAttributes dwFileAttributes;
        public uint ftCreationTime_dwLowDateTime;
        public uint ftCreationTime_dwHighDateTime;
        public uint ftLastAccessTime_dwLowDateTime;
        public uint ftLastAccessTime_dwHighDateTime;
        public uint ftLastWriteTime_dwLowDateTime;
        public uint ftLastWriteTime_dwHighDateTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }
}
