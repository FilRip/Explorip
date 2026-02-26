using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour LanguageButton.xaml
/// </summary>
public partial class LanguageButton : UserControl
{
    public LanguageButton()
    {
        InitializeComponent();
    }

    public new LanguageButtonViewModel DataContext
    {
        get { return (LanguageButtonViewModel)base.DataContext; }
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        DataContext.ParentTaskbar = (Taskbar)System.Windows.Window.GetWindow(this);
    }
}
