using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemPlusExplorerBrowser.xaml
    /// </summary>
    public partial class TabItemPlusExplorerBrowser : TabItem
    {
        public TabItemPlusExplorerBrowser()
        {
            InitializeComponent();
            HeaderWithCloseButton closableTabHeader = new();
            Header = closableTabHeader;
            MyHeader.PlusButton = true;
            closableTabHeader.Label_TabTitle.SizeChanged += TabTitle_SizeChanged;
            MyHeader.Label_TabTitle.Content = "";
        }

        public TabExplorerBrowser MyTabControl
        {
            get { return (TabExplorerBrowser)Parent; }
        }

        public HeaderWithCloseButton MyHeader
        {
            get { return (HeaderWithCloseButton)Header; }
        }

        private void TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyHeader.ButtonNewTab.Margin = new Thickness(MyHeader.Label_TabTitle.ActualWidth + 5, 3, 0, 0);
        }
    }
}
