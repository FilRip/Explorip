using System;
using System.Globalization;
using System.Windows.Data;

namespace ComputerInfo.Converters;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (int.TryParse(parameter?.ToString(), out int equal) && value is int compare)
        {
            return compare == equal;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
