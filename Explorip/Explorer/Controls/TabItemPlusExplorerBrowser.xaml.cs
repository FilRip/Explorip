using System.Windows;
using System.Windows.Controls;

namespace Explorip.Explorer.Controls;

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
        MyHeader.MyDataContext.PlusButton = true;
        closableTabHeader.lblTitle.SizeChanged += TabTitle_SizeChanged;
        MyHeader.lblTitle.Content = "";
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
        MyHeader.ButtonNewTab.Margin = new Thickness(MyHeader.lblTitle.ActualWidth + 5, 3, 0, 0);
    }
}
