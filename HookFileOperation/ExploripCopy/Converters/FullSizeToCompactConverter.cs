using System;
using System.Globalization;
using System.Windows.Data;

using ExploripCopy.Helpers;

namespace ExploripCopy.Converters;

public class FullSizeToCompactConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && ulong.TryParse(value.ToString(), out ulong size) && parameter is string str)
            return ExtensionsDirectory.SizeInText(size, str);
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
