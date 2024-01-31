using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Desktop.ViewModels;

namespace Explorip.Desktop.Controls
{
    /// <summary>
    /// Logique d'interaction pour OneDesktopItem.xaml
    /// </summary>
    public partial class OneDesktopItem : UserControl
    {
        public OneDesktopItem()
        {
            InitializeComponent();
        }

        internal OneDesktopItemViewModel MyDataContext
        {
            get { return (OneDesktopItemViewModel)DataContext; }
            set { DataContext = value; }
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
                MyDataContext.ExecuteCommand.Execute(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyDataContext.SelectItCommand.Execute(null);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && MyDataContext.CurrentDesktop.ListSelectedItem().Length == 1)
                MyDataContext.RenameCommand.Execute(null);
        }
    }
}
