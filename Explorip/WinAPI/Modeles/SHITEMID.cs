using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHITEMID
    {
        public ushort cb;
        [MarshalAs(UnmanagedType.LPArray)]
        public byte[] abID;

        [StructLayout(LayoutKind.Sequential)]
        public struct ITEMIDLIST
        {
            public SHITEMID mkid;
        }
    }
}
