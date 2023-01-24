using Explorip.Explorer.WPF.Windows;
using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Shell;

using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabExplorerBrowser.xaml
    /// </summary>
    public partial class TabExplorerBrowser : TabControl
    {
        public TabExplorerBrowser()
        {
            DataContext = this;
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
            HideTab();
        }

        private void CloseAllTab_Click(object sender, RoutedEventArgs e)
        {
            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
                if (MyTabControl.SelectedItem != MyTabControl.Items[i] || AutoriseFermerDernierOnglet)
                    MyTabControl.Items.Remove(MyTabControl.Items[i]);
            HideTab();
        }

        public void HideTab()
        {
            if (MyTabControl.Items.Count == 0)
            {
                MyTabControl.Visibility = Visibility.Collapsed;
                WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
                fenetre.HideRightTab();
            }
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
            WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
            if (fenetre.RightTab == MyTabControl && MyTabControl.Items.Count == 1)
                fenetre.ShowRightTab();
        }

        public Color AccentColor
        {
            get { return WindowsSettings.GetWindowsAccentColor(); }
        }

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is not TabItem tabItem)
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Source is TabItem tabItemTarget &&
                e.Data.GetData(typeof(TabItem)) is TabItem tabItemSource &&
                !tabItemTarget.Equals(tabItemSource) &&
                tabItemTarget.Parent is TabControl tabControl)
            {
                int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                tabControl.Items.Remove(tabItemSource);
                tabControl.Items.Insert(targetIndex, tabItemSource);
                tabItemSource.IsSelected = true;
            }
        }
    }
}
