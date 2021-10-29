using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI
{
    public static class Dwmapi
    {
        [DllImport("dwmapi.dll")]
        public static extern bool DwmSetWindowAttribute(IntPtr hwnd, int attribut, ref int attrValeur, int attrSize);
    }
}
