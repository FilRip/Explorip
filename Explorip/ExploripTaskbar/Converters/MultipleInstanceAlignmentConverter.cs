using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Explorip.TaskBar.Converters;

public class MultipleInstanceAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isMultipleInstance && isMultipleInstance)
            return HorizontalAlignment.Left;
        return HorizontalAlignment.Center;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
