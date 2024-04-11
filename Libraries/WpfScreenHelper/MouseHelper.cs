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

    /// <summary>
    /// Get the screen where the mouse is
    /// </summary>
    public static Screen MouseScreen
    {
        get
        {
            return Screen.FromPoint(MousePosition);
        }
    }
}
