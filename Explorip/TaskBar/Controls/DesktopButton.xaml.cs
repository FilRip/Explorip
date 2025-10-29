using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Logique d'interaction pour DesktopButton.xaml
    /// </summary>
    public partial class DesktopButton : UserControl
    {
        public DesktopButton()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDesktopButton.Width = ConfigManager.GetTaskbarConfig(this.FindVisualParent<Taskbar>().NumScreen).DesktopButtonWidth;
        }

        public DesktopButtonViewModel MyDataContext
        {
            get { return (DesktopButtonViewModel)DataContext; }
        }
    }
}
