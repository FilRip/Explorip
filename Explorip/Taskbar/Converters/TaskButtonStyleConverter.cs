using System;
using System.Windows;
using System.Windows.Data;
using ManagedShell.WindowsTasks;

namespace Explorip.TaskBar.Converters
{
    [ValueConversion(typeof(bool), typeof(Style))]
    public class TaskButtonStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(values[0] is FrameworkElement fxElement))
            {
                return null;
            }

            // Default style is Inactive...
            object fxStyle = fxElement.FindResource("TaskButton");

            if (values[1] == null)
            {
                // Default - couldn't get window state.
                return fxStyle;
            }

            if (values[1] is ApplicationWindow.WindowState state)
            {
                switch (state)
                {
                    case ApplicationWindow.WindowState.Active:
                        fxStyle = fxElement.FindResource("TaskButtonActive");
                        break;

                    case ApplicationWindow.WindowState.Flashing:
                        fxStyle = fxElement.FindResource("TaskButtonFlashing");
                        break;
                }
            }

            return fxStyle;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
