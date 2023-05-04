using Explorip.Explorer.WPF.Windows;
using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Shell;

using System.Drawing;
using System.Threading;
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

        public bool AllowCloseLastTab { get; set; }

        #region tabcontrol when add/remove tabitem

        public void HideTab()
        {
            if (MyTabControl.Items.Count == 1)
            {
                MyTabControl.Visibility = Visibility.Collapsed;
                WpfExplorerBrowser fenetre = (WpfExplorerBrowser)Window.GetWindow(this);
                fenetre.HideRightTab();
            }
        }

        public void CloseAllTabs()
        {
            if (MyTabControl.Items.Count > 0)
                for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
                    if (MyTabControl.Items[i] is TabItemExplorip tabItem)
                        tabItem.Dispose();
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

        #endregion

        #region Properties

        public Color AccentColor
        {
            get { return WindowsSettings.GetWindowsAccentColor(); }
        }

        public TabItemExplorip CurrentTab
        {
            get { return (TabItemExplorip)SelectedItem; }
        }

        public TabItemExplorerBrowser CurrentTabExplorer
        {
            get { return (TabItemExplorerBrowser)SelectedItem; }
        }

        public WpfExplorerBrowser MyWindow
        {
            get { return (WpfExplorerBrowser)Window.GetWindow(this); }
        }

        #endregion

        #region Shortcuts

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.F4 && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (Items.Count > 2 || AllowCloseLastTab)
                {
                    Items.Remove(SelectedItem);
                    HideTab();
                }
            }
            else if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control && CurrentTab is TabItemExplorerBrowser tieb)
            {
                AddNewTab(tieb.CurrentDirectory);
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

        #endregion

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

        #endregion

        private void MyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem is TabItemPlusExplorerBrowser && SelectedIndex > 0)
            {
                SelectedIndex--;
                e.Handled = true;
            }
            if (e.AddedItems?.Count > 0 && e.AddedItems[0] is TabItemExplorip tabSelecting)
                tabSelecting.RaiseOnSelecting();
            if (e.RemovedItems?.Count > 0 && e.RemovedItems[0] is TabItemExplorip tabDeselecting)
                tabDeselecting.RaiseOnDeSelecting();
        }
    }
}
