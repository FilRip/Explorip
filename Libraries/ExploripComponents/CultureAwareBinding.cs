using System.Globalization;
using System.Windows.Data;

namespace ExploripComponents;

public class CultureAwareBinding : Binding
{
    public CultureAwareBinding()
    {
        ConverterCulture = CultureInfo.CurrentCulture;
    }
}
