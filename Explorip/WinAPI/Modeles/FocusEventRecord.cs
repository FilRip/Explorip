using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FocusEventRecord
    {
        public uint bSetFocus;
    }
}
