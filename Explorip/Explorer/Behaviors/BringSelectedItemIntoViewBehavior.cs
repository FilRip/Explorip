using System.Windows;
using System.Windows.Controls;

namespace Explorip.Explorer.Behaviors;

public static class BringSelectedItemIntoViewBehavior
{
    public static readonly DependencyProperty IsBringSelectedIntoViewProperty = DependencyProperty.RegisterAttached(
        "IsBringSelectedIntoView", typeof(bool), typeof(BringSelectedItemIntoViewBehavior), new PropertyMetadata(default(bool), PropertyChangedCallback));

    public static void SetIsBringSelectedIntoView(DependencyObject element, bool value)
    {
        element.SetValue(IsBringSelectedIntoViewProperty, value);
    }

    public static bool GetIsBringSelectedIntoView(DependencyObject element)
    {
        return (bool)element.GetValue(IsBringSelectedIntoViewProperty);
    }

    private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (dependencyObject is not TreeViewItem treeViewItem)
            return;

        if (!((bool)dependencyPropertyChangedEventArgs.OldValue) &&
            ((bool)dependencyPropertyChangedEventArgs.NewValue))
        {
            treeViewItem.Unloaded += TreeViewItemOnUnloaded;
            treeViewItem.Selected += TreeViewItemOnSelected;
        }
    }

    private static void TreeViewItemOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not TreeViewItem treeViewItem)
            return;

        treeViewItem.Unloaded -= TreeViewItemOnUnloaded;
        treeViewItem.Selected -= TreeViewItemOnSelected;
    }

    private static void TreeViewItemOnSelected(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is TreeViewItem treeViewItem)
            treeViewItem.BringIntoView();
    }
}
