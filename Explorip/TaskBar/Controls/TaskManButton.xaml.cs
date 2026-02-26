using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class TaskManButton : UserControl
{
    public TaskManButton()
    {
        InitializeComponent();
    }

    public new TaskManButtonViewModel DataContext
    {
        get { return (TaskManButtonViewModel)base.DataContext; }
    }
}
