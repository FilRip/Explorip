using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour SearchBar.xaml
/// </summary>
public partial class SearchZone : UserControl
{
    public SearchZone()
    {
        InitializeComponent();
    }

    public new SearchZoneViewModel DataContext
    {
        get { return (SearchZoneViewModel)base.DataContext; }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        DataContext.SetTaskbar((Taskbar)Window.GetWindow(this));
    }
}
