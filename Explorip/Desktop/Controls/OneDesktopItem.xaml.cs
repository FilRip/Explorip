using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Desktop.ViewModels;

namespace Explorip.Desktop.Controls;

/// <summary>
/// Logique d'interaction pour OneDesktopItem.xaml
/// </summary>
public partial class OneDesktopItem : UserControl
{
    public OneDesktopItem()
    {
        InitializeComponent();
    }

    internal new OneDesktopItemViewModel DataContext
    {
        get { return (OneDesktopItemViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        DataContext.ExecuteCommand.Execute(null);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DataContext.SelectItCommand.Execute(null);
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F2 && DataContext.CurrentDesktop.ListSelectedItem().Length == 1)
            DataContext.RenameCommand.Execute(null);
    }
}
