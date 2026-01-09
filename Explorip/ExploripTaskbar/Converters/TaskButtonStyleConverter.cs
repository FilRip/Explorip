using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using ManagedShell.WindowsTasks;

namespace Explorip.TaskBar.Converters;

public class TaskButtonStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ApplicationWindow.WindowState state)
        {
            switch (state)
            {
                case ApplicationWindow.WindowState.Active:
                    return Application.Current.FindResource("TaskButtonActive");
                case ApplicationWindow.WindowState.Flashing:
                    return Application.Current.FindResource("TaskButtonFlashing");
            }
        }

        // Default style is Inactive...
        return Application.Current.FindResource("TaskButton");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
