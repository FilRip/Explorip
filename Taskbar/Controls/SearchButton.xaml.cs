using System.Windows;
using System.Windows.Controls;

namespace Explorip.TaskBar.Controls
{
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
            WindowsInput.InputSimulator inputSimulator = new WindowsInput.InputSimulator();
            inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.LWIN, WindowsInput.Native.VirtualKeyCode.VK_S);
        }
    }
}
