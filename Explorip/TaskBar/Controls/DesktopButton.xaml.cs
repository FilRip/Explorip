using System;
using System.Windows.Controls;

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
            try
            {
                MyDesktopButton.Width = ConfigManager.GetTaskbarConfig(((Taskbar)System.Windows.Window.GetWindow(this)).NumScreen).DesktopButtonWidth;
            }
            catch (Exception) { /* Ignore errors */ }
        }

        public DesktopButtonViewModel MyDataContext
        {
            get { return (DesktopButtonViewModel)DataContext; }
        }
    }
}
