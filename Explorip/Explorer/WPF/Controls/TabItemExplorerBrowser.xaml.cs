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
        }

        public void Navigation(string repertoire)
        {
            ExplorerBrowser.ExplorerBrowserControl.Navigate(ShellObject.FromParsingName(repertoire));
        }
    }
}
