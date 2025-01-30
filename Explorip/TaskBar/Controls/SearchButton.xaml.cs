using System.Windows;
using System.Windows.Controls;

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

    private void Search_OnClick(object sender, RoutedEventArgs e)
    {
        ManagedShell.Common.Helpers.ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_S);
    }
}
