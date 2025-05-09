using System.Windows.Controls;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.Helpers;

public static class ExtensionsAppBarEdge
{
    public static Orientation GetOrientation(this AppBarEdge appBar)
    {
        if (appBar == AppBarEdge.Left || appBar == AppBarEdge.Right)
            return Orientation.Vertical;
        else
            return Orientation.Horizontal;
    }
}
