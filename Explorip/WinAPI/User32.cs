using System;
using System.Runtime.InteropServices;
using System.Text;

using Explorip.WinAPI.Modeles;

namespace Explorip.WinAPI
{
    public static class User32
    {
        [Flags()]
        public enum TPM : uint
        {
            LEFTBUTTON = 0x0000,
            RIGHTBUTTON = 0x0002,
            LEFTALIGN = 0x0000,
            CENTERALIGN = 0x0004,
            RIGHTALIGN = 0x0008,
            TOPALIGN = 0x0000,
            VCENTERALIGN = 0x0010,
            BOTTOMALIGN = 0x0020,
            HORIZONTAL = 0x0000,
            VERTICAL = 0x0040,
            NONOTIFY = 0x0080,
            RETURNCMD = 0x0100,
            RECURSE = 0x0001,
            HORPOSANIMATION = 0x0400,
            HORNEGANIMATION = 0x0800,
            VERPOSANIMATION = 0x1000,
            VERNEGANIMATION = 0x2000,
            NOANIMATION = 0x4000,
            LAYOUTRTL = 0x8000
        }

        [DllImport("user32.dll")]
        internal static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        internal static extern int GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("User32.dll")]
        internal static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, ref MenuItemInfo lpmii);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowsHookEx(HookType code, LocalWindowsHook.HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        internal static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        internal static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern uint TrackPopupMenuEx(IntPtr hmenu, TPM flags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        // Define the callback delegate's type.
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int EnumWindows(EnumDelegate callPtr, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("USER32.DLL")]
        internal static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        public enum GWL
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, GWL gwl);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, GWL gwl);

        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr hWnd);

        public delegate bool EnumDesktopsDelegate(string desktop, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern bool EnumDesktops(IntPtr hwinsta, EnumDesktopsDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", EntryPoint = "AttachConsole", SetLastError = true)]
        internal static extern bool AttachConsole(int IdProcessus);

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true)]
        internal static extern bool FreeConsole();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public enum ShowWindowCommand
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

        [Flags()]
        public enum SWP
        {
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000,
        }
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, int X, int Y, int cx, int cy, SWP uFlags);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        internal static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out Rect rect);

        [DllImport("USER32.DLL")]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnableWindow(IntPtr hWnd, int bEnable);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateWindow(IntPtr handle);
    }
}
