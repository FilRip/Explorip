using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Explorer.WPF.Window
{
    /// <summary>
    /// Logique d'interaction pour WpfExplorerBrowser.xaml
    /// </summary>
    public partial class WpfExplorerBrowser : System.Windows.Window
    {
        public WpfExplorerBrowser()
        {
            InitializeComponent();
            LeftTab.FirstTab.ExplorerBrowser.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
            RightTab.FirstTab.ExplorerBrowser.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
        }
    }
}
