using System;
using System.Globalization;
using System.Windows.Data;

namespace Explorip.TaskBar.Converters;

public class ProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double progress && parameter is double max)
            return max / progress * 100;
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
