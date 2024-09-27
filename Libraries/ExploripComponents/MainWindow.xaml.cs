using System.Windows;

namespace ExploripComponents
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ExplorerViewModel();
        }

        public ExplorerViewModel MyDataContext
        {
            get { return (ExplorerViewModel)DataContext; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyDataContext.Refresh();
        }
    }
}
