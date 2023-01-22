using Explorip.Explorer.WPF.Windows;

using Microsoft.WindowsAPICodePack.Shell;

using System.Windows;
using System.Windows.Controls;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabExplorerBrowser.xaml
    /// </summary>
    public partial class TabExplorerBrowser : TabControl
    {
        public TabExplorerBrowser()
        {
            InitializeComponent();
        }

        public bool AutoriseFermerDernierOnglet { get; set; }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            TabItemExplorerBrowser item = new();
            item.Navigation(CurrentTab.ExplorerBrowser.NavigationLog[CurrentTab.ExplorerBrowser.NavigationLogIndex]);
            MyTabControl.Items.Add(item);
            MyTabControl.SelectedItem = item;
        }

        private TabItemExplorerBrowser CurrentTab
        {
            get
            {
                if (MyTabControl.Items.Count == 0)
                    return null;
                return (TabItemExplorerBrowser)MyTabControl.SelectedItem;
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (MyTabControl.Items.Count == 1 && !AutoriseFermerDernierOnglet)
                return;
            MyTabControl.Items.Remove(MyTabControl.SelectedItem);
            if (MyTabControl.Items.Count == 0)
                MyTabControl.Visibility = Visibility.Collapsed;
        }

        private void CloseAllTab_Click(object sender, RoutedEventArgs e)
        {
            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
                if (MyTabControl.SelectedItem != MyTabControl.Items[i] || AutoriseFermerDernierOnglet)
                    MyTabControl.Items.Remove(MyTabControl.Items[i]);
        }

        private void NewTabOther_Click(object sender, RoutedEventArgs e)
        {
            WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
            if (fenetre.LeftTab == MyTabControl)
                fenetre.RightTab.AddNewTab(CurrentTab.ExplorerBrowser.NavigationLog[CurrentTab.ExplorerBrowser.NavigationLogIndex]);
            else
                fenetre.LeftTab.AddNewTab(CurrentTab.ExplorerBrowser.NavigationLog[CurrentTab.ExplorerBrowser.NavigationLogIndex]);
        }

        public void AddNewTab(ShellObject location)
        {
            TabItemExplorerBrowser item = new();
            item.Navigation(location);
            MyTabControl.Items.Add(item);
            MyTabControl.SelectedItem = item;
        }
    }
}
