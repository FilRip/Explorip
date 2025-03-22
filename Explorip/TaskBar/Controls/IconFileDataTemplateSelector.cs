using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;

namespace Explorip.TaskBar.Controls;

public class IconFileDataTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        FrameworkElement element = container as FrameworkElement;
        if (container.FindVisualParent<Toolbar>().MyDataContext.CurrentShowLargeIcon)
            return (DataTemplate)element.FindResource("LargeIconTemplate");
        else
            return (DataTemplate)element.FindResource("SmallIconTemplate");
    }
}
