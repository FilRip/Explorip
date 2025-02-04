using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemWindowEmbedded.xaml
    /// </summary>
    public partial class TabItemWindowEmbedded : TabItemExplorip
    {
        public TabItemWindowEmbedded()
        {
            InitializeComponent();
            InitializeExplorip();
            OnSelecting += TabItemWindowEmbedded_OnSelecting;
            OnDeSelecting += TabItemWindowEmbedded_OnDeSelecting;
            DataContext = new TabItemWindowEmbeddedViewModel(this);
        }

        public TabItemWindowEmbeddedViewModel MyDataContext
        {
            get { return (TabItemWindowEmbeddedViewModel)DataContext; }
        }

        private void TabItemWindowEmbedded_OnDeSelecting()
        {
            EmbeddedWindow.Hide();
        }

        private void TabItemWindowEmbedded_OnSelecting()
        {
            EmbeddedWindow.Show();
        }

        private void TabItemExplorip_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetTitle("Embedded window");
        }

        public void Reset()
        {
            MyDataContext.Enabled = false;
        }
    }
}
