using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ExploripComponents.Helpers;

public static class ExtensionsWpf
{
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
}
