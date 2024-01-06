using System;
using System.Runtime.InteropServices;

namespace ExploripSharedCopy.WinAPI
{
    public static class Dwmapi
    {
        [DllImport("dwmapi.dll")]
        internal static extern bool DwmSetWindowAttribute(IntPtr hwnd, int attribut, ref int attrValeur, int attrSize);
    }
}
