using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class WidgetsButton : UserControl
{
    public WidgetsButton()
    {
        InitializeComponent();
    }

    public new WidgetsButtonViewModel DataContext
    {
        get { return (WidgetsButtonViewModel)base.DataContext; }
    }
}
