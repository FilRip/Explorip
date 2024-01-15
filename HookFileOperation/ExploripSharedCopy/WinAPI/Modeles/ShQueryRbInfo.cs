using System.Runtime.InteropServices;

namespace ExploripSharedCopy.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ShQueryRbInfo
    {
        public int cbSize;
        public long Size;
        public long NumItems;
    }
}
