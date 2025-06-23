using System;
using System.Globalization;
using System.Windows.Data;

namespace Explorip.TaskBar.Converters;

public class EnumToBool : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && parameter != null && value.ToString() == parameter.ToString())
            return true;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
