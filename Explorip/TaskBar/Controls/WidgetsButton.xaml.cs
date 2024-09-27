using System.Windows;
using System.Windows.Controls;

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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    private void Widgets_OnClick(object sender, RoutedEventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    {
        ManagedShell.Common.Helpers.ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_W);
    }
}
