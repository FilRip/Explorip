using System.Runtime.InteropServices;

namespace HookTaskbarList.TaskbarList.Interfaces;

[StructLayout(LayoutKind.Sequential)]
public struct Rect(int left, int top, int right, int bottom)
{
    public int Left = left;
    public int Top = top;
    public int Right = right;
    public int Bottom = bottom;

    public readonly int Width
    {
        get { return Right - Left; }
    }

    public readonly int Height
    {
        get { return Bottom - Top; }
    }
}
