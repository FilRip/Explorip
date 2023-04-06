using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Explorip.Explorer.WPF.Windows;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CloseableItem.xaml
    /// </summary>
    public partial class HeaderWithCloseButton : UserControl, INotifyPropertyChanged
    {
        public HeaderWithCloseButton()
        {
            DataContext = this;
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

        private bool _plusButton;
        public bool PlusButton
        {
            get { return _plusButton; }
            set
            {
                _plusButton = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            if (MyTabControl.Items.Count == 1 && !MyTabControl.AllowCloseLastTab)
                return;
            MyTabControl.Items.Remove(MyTabControl.SelectedItem);
            myTabControl.HideTab();
        }

        private void CloseAllTab_Click(object sender, RoutedEventArgs e)
        {
            TabExplorerBrowser myTabControl = MyTabControl;
            for (int i = myTabControl.Items.Count - 1; i >= 0; i--)
                if (myTabControl.SelectedItem != myTabControl.Items[i] || myTabControl.AllowCloseLastTab)
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

        private void ButtonClose_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (PlusButton)
                ButtonNewTab.Foreground = Brushes.Lime;
            else
                ButtonClose.Foreground = Brushes.Red;
        }

        private void ButtonClose_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (PlusButton)
                ButtonNewTab.Foreground = Brushes.White;
            else
                ButtonClose.Foreground = Brushes.Black;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (PlusButton)
            {
                MyTabControl.AddNewTab((ShellObject)KnownFolders.Desktop);
                e.Handled = true;
            }
            else
            {
                if (MyTabControl.Items.Count == 2 && !MyTabControl.AllowCloseLastTab)
                    return;
                TabExplorerBrowser previousTabControl = MyTabControl;
                MyTabItem.Dispose();
                previousTabControl.HideTab();
            }
        }
    }
}
