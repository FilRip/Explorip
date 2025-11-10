using System.Runtime.InteropServices;
using System.Windows;

namespace Monitorian.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct NativeRect
{
    public int left;
    public int top;
    public int right;
    public int bottom;

    public static implicit operator Rect(NativeRect rect)
    {
        if ((rect.right < rect.left) || (rect.bottom < rect.top))
            return Rect.Empty;

        return new Rect(
            rect.left,
            rect.top,
            rect.right - rect.left,
            rect.bottom - rect.top);
    }

    public static implicit operator NativeRect(Rect rect)
    {
        return new NativeRect
        {
            left = (int)rect.Left,
            top = (int)rect.Top,
            right = (int)rect.Right,
            bottom = (int)rect.Bottom
        };
    }
}
