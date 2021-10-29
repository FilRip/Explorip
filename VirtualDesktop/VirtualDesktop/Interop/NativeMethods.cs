﻿using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern uint RegisterWindowMessage(string lpProcName);

        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);
    }
}
