using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential)]
public struct BrowseInfo
{
    public IntPtr hwndOwner;
    public IntPtr pidlRoot;
    public IntPtr pszDisplayName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string lpszTitle;
    public uint ulFlags;
    public IntPtr lpfn;
    public int lParam;
    public IntPtr iImage;
}
