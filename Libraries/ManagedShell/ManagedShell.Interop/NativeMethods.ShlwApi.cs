using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    const string ShlwApi_DllName = "shlwapi.dll";

    [DllImport(ShlwApi_DllName)]
    internal static extern IntPtr SHLockShared(IntPtr hData, uint dwProcessId);

    [DllImport(ShlwApi_DllName, SetLastError = true)]
    internal static extern bool SHUnlockShared(IntPtr pvData);

    [DllImport(ShlwApi_DllName, EntryPoint = "StrRetToBuf", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);

    [DllImport(ShlwApi_DllName, CharSet = CharSet.Auto)]
    internal static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);
}
