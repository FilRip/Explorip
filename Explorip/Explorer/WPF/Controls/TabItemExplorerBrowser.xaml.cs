using Explorip.Explorer.WPF.ViewModels;

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemExplorerBrowser.xaml
    /// </summary>
    public partial class TabItemExplorerBrowser : TabItem
    {
        public TabItemExplorerBrowser()
        {
            InitializeComponent();
            ExplorerBrowser.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;

            // Create an instance of the usercontrol
            HeaderWithCloseButton closableTabHeader = new();
            // Assign the usercontrol to the tab header
            Header = closableTabHeader;
            // Attach to the CloseableHeader events (Mouse Enter/Leave, Button Click, and Label resize)
            closableTabHeader.ButtonClose.MouseEnter += ButtonClose_MouseEnter;
            closableTabHeader.ButtonClose.MouseLeave += ButtonClose_MouseLeave;
            closableTabHeader.ButtonClose.Click += ButtonClose_Click;
            closableTabHeader.Label_TabTitle.SizeChanged += TabTitle_SizeChanged;
        }

        public HeaderWithCloseButton MyHeader
        {
            get { return (HeaderWithCloseButton)Header; }
        }

        public TabExplorerBrowser MyTabControl
        {
            get { return (TabExplorerBrowser)Parent; }
        }

        /// <summary>
        /// Property - Set the Title of the Tab
        /// </summary>
        public void SetTitle(string newTitle)
        {
            MyHeader.Label_TabTitle.Content = newTitle;
        }

        public TabItemExplorerBrowserViewModel MyDataContext
        {
            get { return (TabItemExplorerBrowserViewModel)DataContext; }
        }

        #region Navigation file explorer

        private void ExplorerBrowserControl_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            SetTitle(e.NewLocation.Name);
            CurrentPath.Inlines?.Clear();

            string pathLink = e.NewLocation.GetDisplayName(DisplayNameType.FileSystemPath);
            StringBuilder partialPath = new();
            foreach (string path in pathLink.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries))
            {
                partialPath.Append(path + @"\");
                Hyperlink lb = new()
                {
                    Foreground = Brushes.Yellow,
                    NavigateUri = new Uri(partialPath.ToString()),
                };
                lb.RequestNavigate += Lb_RequestNavigate;
                lb.Inlines.Add(path);
                CurrentPath.Inlines.Add(lb);
                CurrentPath.Inlines.Add(" \\ ");
            }

            MyDataContext.AllowNavigatePrevious = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CanNavigateBackward;
            MyDataContext.AllowNavigateNext = ExplorerBrowser.ExplorerBrowserControl.NavigationLog.CanNavigateForward;
        }

        private void Lb_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(e.Uri.ToString()));
        }

        public void Navigation(string repertoire)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(repertoire));
        }

        public void Navigation(ShellObject repertoire)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(repertoire);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.NavigateLogLocation(Microsoft.WindowsAPICodePack.Controls.NavigationLogDirection.Forward);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            ExplorerBrowser.ExplorerBrowserControl.NavigateLogLocation(Microsoft.WindowsAPICodePack.Controls.NavigationLogDirection.Backward);
        }

        #endregion

        #region Support Close button on Header

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            MyHeader.ButtonClose.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            MyHeader.ButtonClose.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            MyHeader.ButtonClose.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                MyHeader.ButtonClose.Visibility = Visibility.Hidden;
            }
        }

        private void ButtonClose_MouseEnter(object sender, MouseEventArgs e)
        {
            MyHeader.ButtonClose.Foreground = Brushes.Red;
        }

        private void ButtonClose_MouseLeave(object sender, MouseEventArgs e)
        {
            MyHeader.ButtonClose.Foreground = Brushes.Black;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MyTabControl.Items.Count == 1 && !MyTabControl.AutoriseFermerDernierOnglet)
                return;
            TabExplorerBrowser previousTabControl = MyTabControl;
            MyTabControl.Items.Remove(MyTabControl.SelectedItem);
            previousTabControl.HideTab();
        }

        private void TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyHeader.ButtonClose.Margin = new Thickness(MyHeader.Label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }

        #endregion
    }
}
