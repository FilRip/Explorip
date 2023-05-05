using System.Windows;
using System.Windows.Media;

namespace Explorip.Helpers
{
    internal static class ExtensionsWpf
    {
        /// <summary>
        /// Retourne le premier parent de type T de l'objet donné
        /// </summary>
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we've reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we're looking for
            if (parentObject is T parent)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

        public static T FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            int nbChild;
            if ((nbChild = VisualTreeHelper.GetChildrenCount(parent)) == 0)
                return null;
            for (int i = 0; i < nbChild; i++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(parent, i);
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

        public static T FindControlParent<T>(this FrameworkElement control) where T : FrameworkElement
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
    }
}
