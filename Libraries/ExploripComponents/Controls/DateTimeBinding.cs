using System.Globalization;
using System.Windows.Data;

namespace ExploripComponents.Controls;

public class DateTimeBinding : Binding
{
    public DateTimeBinding()
    {
        ConverterCulture = CultureInfo.CurrentCulture;
        StringFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
    }
}
