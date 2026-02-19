using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ManagedShell.Interop;

namespace ExploripCopy.Helpers;

public static class ExtensionsWpf
{
    public static void AdaptSize(ListView lv, GridLength[] columnsSize)
    {
        GridView gv = ((GridView)lv.View);
        double totalWidth = 0;
        for (int i = 0; i < gv.Columns.Count; i++)
            if (!columnsSize[i].IsStar)
                totalWidth += gv.Columns[i].Width;
        double remainingSize = lv.ActualWidth - totalWidth - SystemParameters.VerticalScrollBarWidth;
        remainingSize = Math.Max(remainingSize, 0);
        for (int i = 0; i < gv.Columns.Count; i++)
            if (columnsSize[i].IsStar)
                gv.Columns[i].Width = columnsSize[i].Value * remainingSize;
    }

    public static Point GetMousePosition(Visual relativeTo)
    {
        System.Drawing.Point pos = new();
        NativeMethods.GetCursorPos(ref pos);
        return relativeTo.PointFromScreen(new Point(pos.X, pos.Y));
    }

    internal static ScrollViewer GetScrollViewer(this Control control)
    {
        if (VisualTreeHelper.GetChildrenCount(control) == 0)
            return null;
        DependencyObject x = VisualTreeHelper.GetChild(control, 0);
        if (x == null)
            return null;
        if (VisualTreeHelper.GetChildrenCount(x) == 0)
            return null;
        return (ScrollViewer)VisualTreeHelper.GetChild(x, 0);
    }

    public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
    {
        // get parent item
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        // we've reached the end of the tree
        if (parentObject == null)
            return null;

        // check if the parent matches the type we're looking for
        if (parentObject is T parent)
            return parent;
        else
            return FindVisualParent<T>(parentObject);
    }
}
