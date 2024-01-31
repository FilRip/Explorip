using System.Windows.Controls;

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

        private void Button_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && e.RightButton == System.Windows.Input.MouseButtonState.Released)
                MyDataContext.ExecuteCommand.Execute(null);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDataContext.SelectItCommand.Execute(null);
        }
    }
}
