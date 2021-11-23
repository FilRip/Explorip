using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
