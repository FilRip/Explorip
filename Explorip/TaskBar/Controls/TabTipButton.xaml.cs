using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class TabTipButton : UserControl
{
    public TabTipButton()
    {
        InitializeComponent();
    }

    public new TabTipViewModel DataContext
    {
        get { return (TabTipViewModel)base.DataContext; }
    }
}
