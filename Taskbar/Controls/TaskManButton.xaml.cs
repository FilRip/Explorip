using System.Windows;
using System.Windows.Controls;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for StartButton.xaml
    /// </summary>
    public partial class TaskManButton : UserControl
    {
        public TaskManButton()
        {
            InitializeComponent();
        }

        private void TaskMan_OnClick(object sender, RoutedEventArgs e)
        {
            WindowsInput.InputSimulator inputSimulator = new WindowsInput.InputSimulator();
            inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.LWIN, WindowsInput.Native.VirtualKeyCode.TAB);
        }
    }
}
