using System;
using System.Windows;
using System.Windows.Controls;

namespace Explorip.Helpers;

public static class ExtensionsMenuItem
{
    public static MenuItem AddEntry(this ContextMenu cm, string title, Action click)
    {
        MenuItem mi = new()
        {
            Header = title,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Margin = new Thickness(0),
            BorderThickness = new Thickness(0),
        };
        mi.Click += (object sender, RoutedEventArgs e) =>
        {
            click();
        };
        cm.Items.Add(mi);
        return mi;
    }
}
