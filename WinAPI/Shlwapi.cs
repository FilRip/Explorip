using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Filexplorip.WinAPI
{
    public static class Shlwapi
    {
        [DllImport("shlwapi.dll", EntryPoint = "StrRetToBuf", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);
    }
}
