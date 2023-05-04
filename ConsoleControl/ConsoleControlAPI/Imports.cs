using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace ConsoleControlAPI
{
    internal static class Imports
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ConsoleCharAttribute
        {
            public short attr;
        }

        /// <summary>
        /// Sends a specified signal to a console process group that shares the console associated with the calling process.
        /// </summary>
        /// <param name="dwCtrlEvent">The type of signal to be generated.</param>
        /// <param name="dwProcessGroupId">The identifier of the process group to receive the signal. A process group is created when the CREATE_NEW_PROCESS_GROUP flag is specified in a call to the CreateProcess function. The process identifier of the new process is also the process group identifier of a new process group. The process group includes all processes that are descendants of the root process. Only those processes in the group that share the same console as the calling process receive the signal. In other words, if a process in the group creates a new console, that process does not receive the signal, nor do its descendants.
        /// If this parameter is zero, the signal is generated in all processes that share the console of the calling process.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("Kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(CTRL_EVENT dwCtrlEvent, UInt32 dwProcessGroupId);

        [DllImport("kernel32.dll")]
        internal static extern int ReadConsoleOutputAttribute(IntPtr hConsoleOutput, out ushort[] lpAttribute, uint nLength, Coord dwReadCoord, out uint lpNumberOfAttrsRead);

        [DllImport("kernel32.dll", EntryPoint = "AttachConsole", SetLastError = true)]
        internal static extern bool AttachConsole(int IdProcessus);

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true)]
        internal static extern bool FreeConsole();

        [DllImport("kernel32")]
        internal static extern IntPtr GetStdHandle(StdHandle index);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateConsoleScreenBuffer(ConsoleAccess dwDesiredAccess, ConsoleAccess dwShareMode, [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes, int dwFlags, IntPtr lpScreenBufferData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, [MarshalAs(UnmanagedType.LPStruct)] out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

        internal enum StdHandle
        {
            OutputHandle = -11,
            InputHandle = -10,
            ErrorHandle = -12
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ConsoleScreenBufferInfo
        {
            public Coord dwSize;
            public Coord dwCursorPosition;
            public short wAttributes;
            [MarshalAs(UnmanagedType.Struct)] public SmallRect srWindow;
            public Coord dwMaximumWindowSize;
        }

#pragma warning disable S2292
        [StructLayout(LayoutKind.Sequential)]
        public class SmallRect
        {
            public short left;
            public short top;
            public short right;
            public short bottom;

            /// <summary>
            /// Creates a new instance of the SmallRect structure.
            /// </summary>
            /// <param name="mLeft">Column position of top left corner.</param>
            /// <param name="mTop">Row position of the top left corner.</param>
            /// <param name="mRight">Column position of the bottom right corner.</param>
            /// <param name="mBottom">Row position of the bottom right corner.</param>
            public SmallRect(short mLeft, short mTop, short mRight, short mBottom)
            {
                left = mLeft;
                top = mTop;
                right = mRight;
                bottom = mBottom;
            }

            /// <summary>
            /// Gets or sets the column position of the top left corner of a rectangle.
            /// </summary>
            public short Left
            {
                get { return left; }
                set { left = value; }
            }

            /// <summary>
            /// Gets or sets the row position of the top left corner of a rectangle.
            /// </summary>
            public short Top
            {
                get { return top; }
                set { top = value; }
            }

            /// <summary>
            /// Gets or sets the column position of the bottom right corner of a rectangle.
            /// </summary>
            public short Right
            {
                get { return right; }
                set { right = value; }
            }

            /// <summary>
            /// Gets or sets the row position of the bottom right corner of a rectangle.
            /// </summary>
            public short Bottom
            {
                get { return bottom; }
                set { bottom = value; }
            }

            /// <summary>
            /// Gets or sets the width of a rectangle.  When setting the width, the
            /// column position of the bottom right corner is adjusted.
            /// </summary>
            public short Width
            {
                get { return (short)(right - left + 1); }
                set { right = (short)(left + value - 1); }
            }

            /// <summary>
            /// Gets or sets the height of a rectangle.  When setting the height, the
            /// row position of the bottom right corner is adjusted.
            /// </summary>
            public short Height
            {
                get { return (short)(bottom - top + 1); }
                set { bottom = (short)(top + value - 1); }
            }
        }
#pragma warning restore S2292

        [Flags()]
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
        internal enum ConsoleAccess
        {
            FILE_SHARE_READ = 1,
            FILE_SHARE_WRITE = 2,
            GENERIC_READ = unchecked((int)0x80000000),
            GENERIC_WRITE = 0x40000000,
        }
#pragma warning restore S4070 // Non-flags enums should not be marked with "FlagsAttribute"

        /// <summary>
        /// The type of signal to be generated.
        /// </summary>
        internal enum CTRL_EVENT : uint
        {
            /// <summary>
            /// Generates a CTRL+C signal. This signal cannot be generated for process groups. If dwProcessGroupId is nonzero, this function will succeed, but the CTRL+C signal will not be received by processes within the specified process group.
            /// </summary>
            CTRL_C_EVENT = 0,

            /// <summary>
            /// Generates a CTRL+BREAK signal.
            /// </summary>
            CTRL_BREAK_EVENT = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SecurityAttributes
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }
    }
}