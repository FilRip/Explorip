﻿using System;
using System.Windows.Controls;
using System.Windows.Data;

using Explorip.TaskBar.Utilities;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.Converters
{
    [ValueConversion(typeof(bool), typeof(Orientation))]
    public class EdgeOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Settings.Instance.Edge == (int)AppBarEdge.Left || Settings.Instance.Edge == (int)AppBarEdge.Right)
            {
                return Orientation.Vertical;
            }
            else
            {
                return Orientation.Horizontal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
