﻿using System.Windows;

namespace WpfScreenHelper
{
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
                var pt = new NativeMethods.Point();
                NativeMethods.GetCursorPos(pt);
                return new Point(pt.x, pt.y);
            }
        }
    }
}
