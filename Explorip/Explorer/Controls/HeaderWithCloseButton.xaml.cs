using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Explorip.Explorer.ViewModels;

using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Interaction logic for CloseableItem.xaml
/// </summary>
public partial class HeaderWithCloseButton : UserControl
{
    public HeaderWithCloseButton()
    {
        InitializeComponent();
        DataContext = new HeaderWithCloseButtonViewModel(this);
    }

    #region Properties

    public new HeaderWithCloseButtonViewModel DataContext
    {
        get { return (HeaderWithCloseButtonViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    public TabExplorerBrowser MyTabControl
    {
        get { return (TabExplorerBrowser)MyTabItem.Parent; }
    }

    public TabItem MyTabItem
    {
        get { return (TabItem)Parent; }
    }

    public TabItemExplorerBrowser CurrentTabExplorer
    {
        get
        {
            if (MyTabControl.Items.Count == 0)
                return null;
            return MyTabControl.SelectedItem as TabItemExplorerBrowser;
        }
    }

    #endregion

    #region Mouse events

    private void ButtonClose_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (DataContext.PlusButton)
            ButtonNewTab.Foreground = Brushes.Lime;
        else
            ButtonClose.Foreground = Brushes.Red;
    }

    private void ButtonClose_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (DataContext.PlusButton)
            ButtonNewTab.Foreground = Brushes.White;
        else
            ButtonClose.Foreground = Brushes.Black;
    }

    private void ButtonClose_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext.PlusButton)
        {
            MyTabControl.AddNewTab((ShellObject)KnownFolders.Desktop);
            e.Handled = true;
        }
        else
        {
            if (MyTabControl.Items.Count == 2 && !MyTabControl.AllowCloseLastTab)
                return;
            TabExplorerBrowser previousTabControl = MyTabControl;
            if (MyTabItem is TabItemExplorip tab)
                tab.Dispose();
            previousTabControl.HideTab();
        }
    }

    #endregion
}
