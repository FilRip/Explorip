using System;
using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public const int NAMESIZE = 80;
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Shell32.MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
        public string szTypeName;
    };
}
