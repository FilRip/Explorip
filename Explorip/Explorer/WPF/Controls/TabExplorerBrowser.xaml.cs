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
    }
}
