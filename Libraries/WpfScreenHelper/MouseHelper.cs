using System.Windows;

namespace WpfScreenHelper;

/// <summary>
/// Provides helper functions for mouse cursor.
/// </summary>
public static class MouseHelper
{
    /// <summary>
    /// Gets the position of the mouse cursor in screen coordinates.
    /// </summary>
    public static Point MousePosition
    {
        get
        {
            NativeMethods.Point pt = new();
            NativeMethods.GetCursorPos(pt);
            return new Point(pt.x, pt.y);
        }
    }
}
