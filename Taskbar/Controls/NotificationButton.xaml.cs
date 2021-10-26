using System.Windows;
using System.Windows.Controls;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for StartButton.xaml
    /// </summary>
    public partial class NotificationButton : UserControl
    {
        public NotificationButton()
        {
            InitializeComponent();
        }

        private void Notification_OnClick(object sender, RoutedEventArgs e)
        {
            WindowsInput.InputSimulator inputSimulator = new WindowsInput.InputSimulator();
            inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.LWIN, WindowsInput.Native.VirtualKeyCode.VK_A);
        }
    }
}
