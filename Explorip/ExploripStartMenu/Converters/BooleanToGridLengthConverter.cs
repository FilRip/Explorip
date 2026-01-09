using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Explorip.StartMenu.Converters;

public class BooleanToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = (bool)value;
        GridLength gridLength = (GridLength)new GridLengthConverter().ConvertFromString(null, CultureInfo.InvariantCulture, parameter.ToString());
        return isVisible ? gridLength : new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
