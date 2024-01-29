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
    }
}
