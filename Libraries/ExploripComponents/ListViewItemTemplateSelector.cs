using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;

namespace ExploripComponents;

public class ListViewItemTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        FrameworkElement? element = container as FrameworkElement;
        MainWindow control = element.FindVisualParent<MainWindow>();
        if (control.MyDataContext.ViewDetails)
            return (DataTemplate)control.FindResource("DetailsTemplate");
        else
            return (DataTemplate)control.FindResource("IconsTemplate");
    }
}
