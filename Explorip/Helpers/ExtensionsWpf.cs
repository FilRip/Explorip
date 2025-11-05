using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Explorip.Helpers;

#nullable enable

internal static class ExtensionsWpf
{
    /// <summary>
    /// Return the first parent of a specified type from a control
    /// </summary>
    public static T? FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
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

    public static T? FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
    {
        int nbChild;
        if ((nbChild = VisualTreeHelper.GetChildrenCount(parent)) == 0)
            return null;
        for (int i = 0; i < nbChild; i++)
        {
            DependencyObject? childObject = VisualTreeHelper.GetChild(parent, i);
            if (childObject is T result)
                return result;
            else
            {
                childObject = FindVisualChild<T>(childObject);
                if (childObject is T retour)
                    return retour;
            }
        }
        return null;
    }

    public static T? FindControlParent<T>(this FrameworkElement control) where T : FrameworkElement
    {
        // get parent item
        DependencyObject @object = control.Parent;

        if (@object is not FrameworkElement parentObject)
            return null;

        // check if the parent matches the type we're looking for
        if (parentObject is T parent)
            return parent;
        else
            return FindControlParent<T>(parentObject);
    }

    internal static ScrollViewer? GetScrollViewer(this Control control)
    {
        if (VisualTreeHelper.GetChildrenCount(control) == 0)
            return null;
        var x = VisualTreeHelper.GetChild(control, 0);
        if (x == null)
            return null;
        if (VisualTreeHelper.GetChildrenCount(x) == 0)
            return null;
        return (ScrollViewer)VisualTreeHelper.GetChild(x, 0);
    }

    internal static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> toAdd)
    {
        foreach (T item in toAdd)
            source.Add(item);
    }

    internal static Rect GetAbsoluteRectangle(this Control control)
    {
        Point location = control.PointToScreen(new Point(0, 0));
        return new Rect(location, control.RenderSize);
    }

    public static ImageSource CreateImageFromWpfControl(UIElement source)
    {
        Rect bounds = VisualTreeHelper.GetDescendantBounds(source);
        RenderTargetBitmap rtb = new((int)bounds.Width,
                                     (int)bounds.Height,
                                     96,
                                     96,
                                     PixelFormats.Pbgra32);

        DrawingVisual dv = new();
        using (DrawingContext ctx = dv.RenderOpen())
        {
            VisualBrush vb = new(source);
            ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
        }

        rtb.Render(dv);

        return rtb;
    }
}
