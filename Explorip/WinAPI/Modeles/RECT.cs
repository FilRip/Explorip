using System.Runtime.InteropServices;

namespace Explorip.WinAPI.Modeles;

[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int left, top, right, bottom;
}
