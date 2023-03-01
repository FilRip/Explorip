using System.Windows;
using System.Windows.Controls;

using Explorip.Explorer.WPF.Windows;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CloseableItem.xaml
    /// </summary>
    public partial class HeaderWithCloseButton : UserControl
    {
        public HeaderWithCloseButton()
        {
            InitializeComponent();
        }

        private TabExplorerBrowser MyTabControl
        {
            get { return (TabExplorerBrowser)MyTabItem.Parent; }
        }

        private TabItemExplorerBrowser MyTabItem
        {
            get { return (TabItemExplorerBrowser)Parent; }
        }

        #region Context menu

        private TabItemExplorerBrowser CurrentTab
        {
            get
            {
                if (MyTabControl.Items.Count == 0)
                    return null;
                return (TabItemExplorerBrowser)MyTabControl.SelectedItem;
            }
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            MyTabControl.AddNewTab(CurrentTab.ExplorerBrowser.NavigationLog[CurrentTab.ExplorerBrowser.NavigationLogIndex]);
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            TabExplorerBrowser myTabControl = MyTabControl;
            if (MyTabControl.Items.Count == 1 && !MyTabControl.AutoriseFermerDernierOnglet)
                return;
            MyTabControl.Items.Remove(MyTabControl.SelectedItem);
            myTabControl.HideTab();
        }

        private void CloseAllTab_Click(object sender, RoutedEventArgs e)
        {
            TabExplorerBrowser myTabControl = MyTabControl;
            for (int i = myTabControl.Items.Count - 1; i >= 0; i--)
                if (myTabControl.SelectedItem != myTabControl.Items[i] || myTabControl.AutoriseFermerDernierOnglet)
                    myTabControl.Items.Remove(myTabControl.Items[i]);
            myTabControl.HideTab();
        }

        private void NewTabOther_Click(object sender, RoutedEventArgs e)
        {
            WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
            if (fenetre.LeftTab == MyTabControl)
                fenetre.RightTab.AddNewTab(CurrentTab.ExplorerBrowser.NavigationLog[CurrentTab.ExplorerBrowser.NavigationLogIndex]);
            else
                fenetre.LeftTab.AddNewTab(CurrentTab.ExplorerBrowser.NavigationLog[CurrentTab.ExplorerBrowser.NavigationLogIndex]);
        }

        #endregion
    }
}
