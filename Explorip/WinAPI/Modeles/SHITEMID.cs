using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShItemId
    {
        public ushort cb;
        [MarshalAs(UnmanagedType.LPArray)]
        public byte[] abID;

        [StructLayout(LayoutKind.Sequential)]
        public struct ItemIdList
        {
            public ShItemId mkid;
        }
    }
}
