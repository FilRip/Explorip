using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class SearchButton : UserControl
{
    public SearchButton()
    {
        InitializeComponent();
    }

    public new SearchButtonViewModel DataContext
    {
        get { return (SearchButtonViewModel)base.DataContext; }
    }
}
