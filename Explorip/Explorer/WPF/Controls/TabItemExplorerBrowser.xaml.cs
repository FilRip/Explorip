using Explorip.Explorer.WPF.ViewModels;

using Microsoft.WindowsAPICodePack.Shell;

using System.Windows.Controls;

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
        }

        public TabItemExplorerBrowserViewModel MyDataContext
        {
            get { return (TabItemExplorerBrowserViewModel)DataContext; }
        }
        private void ExplorerBrowserControl_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            MyDataContext.TabTitle = e.NewLocation.Name;
        }

        public void Navigation(string repertoire)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(repertoire));
        }
    }
}
