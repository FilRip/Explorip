using Explorip.Explorer.WPF.Windows;
using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Shell;

using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        public void HideTab()
        {
            if (MyTabControl.Items.Count == 1)
            {
                MyTabControl.Visibility = Visibility.Collapsed;
                WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
                fenetre.HideRightTab();
            }
        }

        public void AddNewTab(ShellObject location)
        {
            TabItemExplorerBrowser item = new();
            item.Navigation(location);
            Items.Insert(Items.Count - 1, item);
            SelectedItem = item;
            WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
            if (fenetre.RightTab == MyTabControl && MyTabControl.Items.Count > 1)
                fenetre.ShowRightTab();
        }

        public Color AccentColor
        {
            get { return WindowsSettings.GetWindowsAccentColor(); }
        }

        public TabItemExplorerBrowser CurrentTab
        {
            get { return (TabItemExplorerBrowser)SelectedItem; }
        }

        public WpfExplorerBrowser MyWindow
        {
            get { return (WpfExplorerBrowser)Window.GetWindow(this); }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.F4 && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (Items.Count > 1 || AutoriseFermerDernierOnglet)
                {
                    Items.Remove(SelectedItem);
                    HideTab();
                }
            }
            else if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                AddNewTab(CurrentTab.CurrentDirectory);
            }
            else if (e.Key == Key.Right && e.KeyboardDevice.Modifiers == ModifierKeys.Control &&
                MyWindow.MyDataContext.SelectionLeft)
            {
                MyWindow.CopyLeft.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else if (e.Key == Key.Left && e.KeyboardDevice.Modifiers == ModifierKeys.Control &&
                MyWindow.MyDataContext.SelectionRight)
            {
                MyWindow.CopyRight.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            base.OnKeyUp(e);
        }

        #region Drag'n Drop tab item in tab control

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

        #endregion

        private void MyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem is TabItemPlusExplorerBrowser && SelectedIndex > 0)
            {
                SelectedIndex--;
                e.Handled = true;
            }
        }
    }
}
