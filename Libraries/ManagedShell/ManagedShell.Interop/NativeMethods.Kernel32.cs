using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
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

    [DllImport(Kernel32_DllName, SetLastError = true, ExactSpelling = true)]
    internal static extern bool CloseHandle(IntPtr handle);

    [DllImport(Kernel32_DllName, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWow64Process(
        [In()] IntPtr hProcess,
        [Out()] out bool wow64Process
    );

    [DllImport(Kernel32_DllName)]
    internal static extern uint GetCurrentProcessId();

    [DllImport(Kernel32_DllName)]
    internal static extern uint GetCurrentThreadId();

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern IntPtr OpenProcess(ProcessAccess processAccess, bool bInheritHandle, int processId);

#pragma warning disable IDE0079
#pragma warning disable S4070
    [Flags()]
    public enum ProcessAccess : uint
    {
        All = STANDARD_RIGHTS_REQUIRED | Synchronize | AllAccess,
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
        AllAccess = 0x0000FFFF,
    }
#pragma warning restore S4070
#pragma warning restore IDE0079

    [DllImport(Kernel32_DllName, SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, [Out(), MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpExeName, ref int lpdwSize);


    [DllImport(Kernel32_DllName, SetLastError = true, ExactSpelling = true)]
    internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, AllocationTypes flAllocationType, MemoryProtections flProtect);

    [Flags()]
    public enum AllocationTypes
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
    public enum MemoryProtections
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
        int dwSize, AllocationTypes dwFreeType);

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
    public enum ControlKeyStates
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
        public ControlKeyStates dwControlKeyState;
    }

    [Flags()]
    public enum MouseEvents
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
        public ControlKeyStates dwControlKeyState;
        public MouseEvents dwEventFlags;
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
    internal enum ELoadLibrairies : uint
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
    internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, ELoadLibrairies dwFlags);

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

    [Flags()]
    public enum Snapshots : uint
    {
        HeapList = 0x00000001,
        Process = 0x00000002,
        Thread = 0x00000004,
        Module = 0x00000008,
        Module32 = 0x00000010,
        All = (HeapList | Process | Thread | Module),
        Inherit = 0x80000000,
        NoHeaps = 0x40000000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessEntry32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
    };

    internal sealed class SafeSnapshotHandle : SafeHandleMinusOneIsInvalid
    {
        internal SafeSnapshotHandle() : base(true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeSnapshotHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return CloseHandle(base.handle);
        }
    }

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern SafeSnapshotHandle CreateToolhelp32Snapshot(Snapshots flags, uint id);

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool Process32First(SafeSnapshotHandle hSnapshot, ref ProcessEntry32 lppe);

    [DllImport(Kernel32_DllName, SetLastError = true)]
    internal static extern bool Process32Next(SafeSnapshotHandle hSnapshot, ref ProcessEntry32 lppe);

    [DllImport(Kernel32_DllName)]
    internal static extern uint GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

    [DllImport(Kernel32_DllName, SetLastError = false, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetDevicePowerState([In()] IntPtr hDevice, [Out(), MarshalAs(UnmanagedType.Bool)] out bool pfOn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct StartupInfo
    {
        public uint cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public uint dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars;
        public uint dwFillAttribute;
        public uint dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput, hStdOutput, hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess, hThread;
        public uint dwProcessId, dwThreadId;
    }

    [DllImport(Kernel32_DllName, SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool CreateProcess(
        string lpApplicationName,
        string lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        ECreateProcess dwCreationFlags,
        IntPtr lpEnvironment,
        string lpCurrentDirectory,
        ref StartupInfo lpStartupInfo,
        out ProcessInformation lpProcessInformation);

    [Flags()]
    public enum ECreateProcess : uint
    {
        /// <summary>
        /// The child processes of a process associated with a job are not associated with the job. If the calling process is not associated
        /// with a job, this constant has no effect. If the calling process is associated with a job, the job must set the
        /// JOB_OBJECT_LIMIT_BREAKAWAY_OK limit.
        /// </summary>
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,

        /// <summary>
        /// The new process does not inherit the error mode of the calling process. Instead, the new process gets the default error mode.
        /// This feature is particularly useful for multithreaded shell applications that run with hard errors disabled.The default behavior
        /// is for the new process to inherit the error mode of the caller. Setting this flag changes that default behavior.
        /// </summary>
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,

        /// <summary>
        /// The new process has a new console, instead of inheriting its parent's console (the default). For more information, see Creation
        /// of a Console. This flag cannot be used with DETACHED_PROCESS.
        /// </summary>
        CREATE_NEW_CONSOLE = 0x00000010,

        /// <summary>
        /// The new process is the root process of a new process group. The process group includes all processes that are descendants of this
        /// root process. The process identifier of the new process group is the same as the process identifier, which is returned in the
        /// lpProcessInformation parameter. Process groups are used by the GenerateConsoleCtrlEvent function to enable sending a CTRL+BREAK
        /// signal to a group of console processes.If this flag is specified, CTRL+C signals will be disabled for all processes within the
        /// new process group.This flag is ignored if specified with CREATE_NEW_CONSOLE.
        /// </summary>
        CREATE_NEW_PROCESS_GROUP = 0x00000200,

        /// <summary>
        /// The process is a console application that is being run without a console window. Therefore, the console handle for the
        /// application is not set.This flag is ignored if the application is not a console application, or if it is used with either
        /// CREATE_NEW_CONSOLE or DETACHED_PROCESS.
        /// </summary>
        CREATE_NO_WINDOW = 0x08000000,

        /// <summary>
        /// The process is to be run as a protected process. The system restricts access to protected processes and the threads of protected
        /// processes. For more information on how processes can interact with protected processes, see Process Security and Access Rights.To
        /// activate a protected process, the binary must have a special signature. This signature is provided by Microsoft but not currently
        /// available for non-Microsoft binaries. There are currently four protected processes: media foundation, audio engine, Windows error
        /// reporting, and system. Components that load into these binaries must also be signed. Multimedia companies can leverage the first
        /// two protected processes. For more information, see Overview of the Protected Media Path.Windows Server 2003 and Windows XP: This
        /// value is not supported.
        /// </summary>
        CREATE_PROTECTED_PROCESS = 0x00040000,

        /// <summary>
        /// Allows the caller to execute a child process that bypasses the process restrictions that would normally be applied automatically
        /// to the process.
        /// </summary>
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,

        /// <summary>This flag allows secure processes, that run in the Virtualization-Based Security environment, to launch.</summary>
        CREATE_SECURE_PROCESS = 0x00400000,

        /// <summary>
        /// This flag is valid only when starting a 16-bit Windows-based application. If set, the new process runs in a private Virtual DOS
        /// Machine (VDM). By default, all 16-bit Windows-based applications run as threads in a single, shared VDM. The advantage of running
        /// separately is that a crash only terminates the single VDM; any other programs running in distinct VDMs continue to function
        /// normally. Also, 16-bit Windows-based applications that are run in separate VDMs have separate input queues. That means that if
        /// one application stops responding momentarily, applications in separate VDMs continue to receive input. The disadvantage of
        /// running separately is that it takes significantly more memory to do so. You should use this flag only if the user requests that
        /// 16-bit applications should run in their own VDM.
        /// </summary>
        CREATE_SEPARATE_WOW_VDM = 0x00000800,

        /// <summary>
        /// The flag is valid only when starting a 16-bit Windows-based application. If the DefaultSeparateVDM switch in the Windows section
        /// of WIN.INI is TRUE, this flag overrides the switch. The new process is run in the shared Virtual DOS Machine.
        /// </summary>
        CREATE_SHARED_WOW_VDM = 0x00001000,

        /// <summary>
        /// The primary thread of the new process is created in a suspended state, and does not run until the ResumeThread function is called.
        /// </summary>
        CREATE_SUSPENDED = 0x00000004,

        /// <summary>
        /// If this flag is set, the environment block pointed to by lpEnvironment uses Unicode characters. Otherwise, the environment block
        /// uses ANSI characters.
        /// </summary>
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,

        /// <summary>
        /// The calling thread starts and debugs the new process. It can receive all related debug events using the WaitForDebugEvent function.
        /// </summary>
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,

        /// <summary>
        /// The calling thread starts and debugs the new process and all child processes created by the new process. It can receive all
        /// related debug events using the WaitForDebugEvent function. A process that uses DEBUG_PROCESS becomes the root of a debugging
        /// chain. This continues until another process in the chain is created with DEBUG_PROCESS.If this flag is combined with
        /// DEBUG_ONLY_THIS_PROCESS, the caller debugs only the new process, not any child processes.
        /// </summary>
        DEBUG_PROCESS = 0x00000001,

        /// <summary>
        /// For console processes, the new process does not inherit its parent's console (the default). The new process can call the
        /// AllocConsole function at a later time to create a console. For more information, see Creation of a Console. This value cannot be
        /// used with CREATE_NEW_CONSOLE.
        /// </summary>
        DETACHED_PROCESS = 0x00000008,

        /// <summary>
        /// The process is created with extended startup information; the lpStartupInfo parameter specifies a STARTUPINFOEX structure.Windows
        /// Server 2003 and Windows XP: This value is not supported.
        /// </summary>
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,

        /// <summary>
        /// The process inherits its parent's affinity. If the parent process has threads in more than one processor group, the new process
        /// inherits the group-relative affinity of an arbitrary group in use by the parent.Windows Server 2008, Windows Vista, Windows
        /// Server 2003 and Windows XP: This value is not supported.
        /// </summary>
        INHERIT_PARENT_AFFINITY = 0x00010000,

        /// <summary>Process with no special scheduling needs.</summary>
        NORMAL_PRIORITY_CLASS = 0x00000020,

        /// <summary>
        /// Process whose threads run only when the system is idle and are preempted by the threads of any process running in a higher
        /// priority class. An example is a screen saver. The idle priority class is inherited by child processes.
        /// </summary>
        IDLE_PRIORITY_CLASS = 0x00000040,

        /// <summary>
        /// Process that performs time-critical tasks that must be executed immediately for it to run correctly. The threads of a
        /// high-priority class process preempt the threads of normal or idle priority class processes. An example is the Task List, which
        /// must respond quickly when called by the user, regardless of the load on the operating system. Use extreme care when using the
        /// high-priority class, because a high-priority class CPU-bound application can use nearly all available cycles.
        /// </summary>
        HIGH_PRIORITY_CLASS = 0x00000080,

        /// <summary>
        /// Process that has the highest possible priority. The threads of a real-time priority class process preempt the threads of all
        /// other processes, including operating system processes performing important tasks. For example, a real-time process that executes
        /// for more than a very brief interval can cause disk caches not to flush or cause the mouse to be unresponsive.
        /// </summary>
        REALTIME_PRIORITY_CLASS = 0x00000100,

        /// <summary>Process that has priority above IDLE_PRIORITY_CLASS but below NORMAL_PRIORITY_CLASS.</summary>
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,

        /// <summary>Process that has priority above NORMAL_PRIORITY_CLASS but below HIGH_PRIORITY_CLASS.</summary>
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,

        /// <summary>Undocumented.</summary>
        CREATE_FORCEDOS = 0x00002000,

        /// <summary>Creates profiles for the user mode modeuls of the process.</summary>
        PROFILE_USER = 0x10000000,

        /// <summary>Undocumented.</summary>
        PROFILE_KERNEL = 0x20000000,

        /// <summary>Undocumented.</summary>
        PROFILE_SERVER = 0x40000000,

        /// <summary>Undocumented.</summary>
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,

        /// <summary>Undocumented.</summary>
        INHERIT_CALLER_PRIORITY = 0x00020000,

        /// <summary>
        /// Begin background processing mode. The system lowers the resource scheduling priorities of the process (and its threads) so that
        /// it can perform background work without significantly affecting activity in the foreground.
        /// <para>
        /// This value can be specified only if hProcess is a handle to the current process. The function fails if the process is already in
        /// background processing mode.
        /// </para>
        /// <para>Windows Server 2003 and Windows XP: This value is not supported.</para>
        /// </summary>
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,

        /// <summary>
        /// End background processing mode. The system restores the resource scheduling priorities of the process (and its threads) as they
        /// were before the process entered background processing mode.
        /// <para>
        /// This value can be specified only if hProcess is a handle to the current process. The function fails if the process is not in
        /// background processing mode.
        /// </para>
        /// <para>Windows Server 2003 and Windows XP: This value is not supported.</para>
        /// </summary>
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
    }

    [DllImport(Kernel32_DllName, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
}
