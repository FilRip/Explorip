using System.Windows;
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
}
