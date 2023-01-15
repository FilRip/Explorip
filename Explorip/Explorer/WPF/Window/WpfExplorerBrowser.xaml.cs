using Microsoft.WindowsAPICodePack.Shell;

using System.Windows;

namespace Explorip.Explorer.WPF
{
    /// <summary>
    /// Logique d'interaction pour WpfExplorerBrowser.xaml
    /// </summary>
    public partial class WpfExplorerBrowser : Window
    {
        public WpfExplorerBrowser()
        {
            InitializeComponent();
            eb.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
            eb.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;
            _repCourant = "Ceci est un test";
        }

        private void ExplorerBrowserControl_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            _repCourant = e.NewLocation.GetDisplayName(DisplayNameType.Url);
        }

        private string _repCourant;

        public string RepertoireCourant
        {
            get { return _repCourant; }
        }
    }
}
