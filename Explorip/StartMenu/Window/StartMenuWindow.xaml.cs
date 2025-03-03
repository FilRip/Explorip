using System;

using Explorip.StartMenu.ViewModels;

namespace Explorip.StartMenu.Window
{
    /// <summary>
    /// Logique d'interaction pour StartMenuWindow.xaml
    /// </summary>
    public partial class StartMenuWindow : System.Windows.Window
    {
        public StartMenuWindow()
        {
            InitializeComponent();
        }

        private StartMenuViewModel MyDataContext
        {
            get { return (StartMenuViewModel)DataContext; }
        }
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
                MyDataContext.Refresh();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
