﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Explorip.WinAPI
{
    public static class Shlwapi
    {
        [DllImport("shlwapi.dll", EntryPoint = "StrRetToBuf", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);
    }
}