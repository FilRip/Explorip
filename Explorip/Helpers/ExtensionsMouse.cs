using System;
using System.Windows;

namespace Explorip.Helpers;

internal static class ExtensionsMouse
{
    public static Point GetDelta(this Point positionMouse, Point newPositionMouse)
    {
        Point ret = new()
        {
            X = Math.Abs(positionMouse.X - newPositionMouse.X),
            Y = Math.Abs(positionMouse.Y - newPositionMouse.Y),
        };
        return ret;
    }
}
