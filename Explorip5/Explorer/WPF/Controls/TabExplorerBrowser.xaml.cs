using System.Windows.Controls;

namespace Explorip.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabExplorerBrowser.xaml
    /// </summary>
    public partial class TabExplorerBrowser : TabControl
    {
        public TabExplorerBrowser()
        {
            InitializeComponent();
            TabItem tabItem = new();
            AddChild(tabItem);
        }

        public bool AutoriseFermerDernierOnglet { get; set; }
    }
}
