using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour FloatingButton.xaml
/// </summary>
public partial class FloatingButton : UserControl
{
    public FloatingButton()
    {
        InitializeComponent();
    }

    public FloatingButtonViewModel MyDataContext
    {
        get { return (FloatingButtonViewModel)DataContext; }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        MyDataContext.ParentTaskbar = (Taskbar)Window.GetWindow(this);
    }
}
