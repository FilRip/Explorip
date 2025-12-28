using System;
using System.Runtime.InteropServices;

namespace VirtualDesktop.Utils;

internal static class NativeMethods
{
    private const string User32Dll = "user32.dll";
    private const string ComBaseDll = "combase.dll";

    [DllImport(User32Dll)]
    public static extern bool CloseWindow(IntPtr hWnd);

    [DllImport(User32Dll)]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport(User32Dll, CharSet = CharSet.Unicode)]
    public static extern uint RegisterWindowMessage(string lpProcName);

    [DllImport(ComBaseDll)]
    public static extern int WindowsCreateString([MarshalAs(UnmanagedType.LPWStr)] string sourceString, int length, out IntPtr hstring);

    [DllImport(ComBaseDll)]
    public static extern IntPtr WindowsGetStringRawBuffer(IntPtr hstring, out uint length);

    [DllImport(ComBaseDll)]
    public static extern int WindowsDeleteString(IntPtr hstring);
}
