using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential)]
public struct ImageInfo
{
    public IntPtr hbmImage;
    public IntPtr hbmMask;
    public int Unused1;
    public int Unused2;
    public Rect rcImage;
}
