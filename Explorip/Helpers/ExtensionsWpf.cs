using System.Windows;
using System.Windows.Media;

namespace Explorip.Helpers
{
    internal static class ExtensionsWpf
    {
        /// <summary>
        /// Retourne le premier parent de type T de l'objet donné
        /// </summary>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we've reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we're looking for
            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public static T FindChild<T>(this DependencyObject parent) where T : DependencyObject
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
                    childObject = FindChild<T>(childObject);
                    if (childObject is T retour)
                        return retour;
                }
            }
            return null;
        }
    }
}
