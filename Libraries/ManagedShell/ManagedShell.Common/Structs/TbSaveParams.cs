using System;
using System.Runtime.InteropServices;

namespace ManagedShell.Common.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct TbSaveParams
{
    public IntPtr hkr; // HKEY
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszSubKey;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszValueName;
}
