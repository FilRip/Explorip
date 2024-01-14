using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ShFileInfo
{
    public IntPtr hIcon;
    public int iIcon;
    public uint dwAttributes;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ManagedShell.Common.Helpers.ShellHelper.MAX_PATH)]
    public string szDisplayName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Shell32.NAMESIZE)]
    public string szTypeName;
};
