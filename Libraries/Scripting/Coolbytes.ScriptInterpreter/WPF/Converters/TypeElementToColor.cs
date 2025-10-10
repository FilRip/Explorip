using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using CoolBytes.ScriptInterpreter.Models;
using CoolBytes.ScriptInterpreter.WPF.ViewModels;

namespace CoolBytes.ScriptInterpreter.WPF.Converters;

internal class TypeElementToColor : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ElementTypes castParam;
        WpfInterpreteScriptViewModel castDataContext;
        if ((parameter == null) || (value == null))
        {
            return null;
        }
        castDataContext = (WpfInterpreteScriptViewModel)((WpfInterpreteScript)parameter).DataContext;
        castParam = (ElementTypes)Enum.Parse(typeof(ElementTypes), value.ToString());
        switch (castParam)
        {
            case ElementTypes.METHOD:
                return castDataContext.MethodColor;
            case ElementTypes.NAMESPACE:
                return castDataContext.NamespaceColorInternal;
            case ElementTypes.PROPERTIES:
                return castDataContext.PropertyColor;
            default:
                break;
        }
        return castDataContext.FieldColorInternal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return this;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
