using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExploripComponents;

public static class ExtensionsWpf
{
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

    internal static List<T> GetVisualChildren<T>(this DependencyObject obj) where T : DependencyObject
    {
        List<T> childList = [];
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child is T item)
                childList.Add(item);
        }

        return childList;
    }

    internal static Rect GetAbsoluteRectangle(this Control control)
    {
        Point location = control.TranslatePoint(new Point(0, 0), control);
        return new Rect(location, control.RenderSize);
    }
}
