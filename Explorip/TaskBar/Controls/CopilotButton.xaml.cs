using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class CopilotButton : UserControl
{
    public CopilotButton()
    {
        InitializeComponent();
    }

    public new CopilotButtonViewModel DataContext
    {
        get { return (CopilotButtonViewModel)base.DataContext; }
    }
}
