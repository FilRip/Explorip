using System.Windows;
using System.Windows.Controls;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for StartButton.xaml
    /// </summary>
    public partial class WidgetsButton : UserControl
    {
        public WidgetsButton()
        {
            InitializeComponent();
        }

        private void Widgets_OnClick(object sender, RoutedEventArgs e)
        {
            ManagedShell.Common.Helpers.ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_W);
        }
    }
}
