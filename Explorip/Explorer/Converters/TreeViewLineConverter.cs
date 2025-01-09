using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Explorip.Explorer.Converters;

public class TreeViewLineConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        TreeViewItem item = (TreeViewItem)value;
        ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
        return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return false;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
