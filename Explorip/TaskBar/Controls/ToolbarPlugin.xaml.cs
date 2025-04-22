using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

using ExploripPlugins;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Logique d'interaction pour ToolbarPlugin.xaml
    /// </summary>
    public partial class ToolbarPlugin : UserControl
    {
        private bool _isLoaded;

        public ToolbarPlugin()
        {
            InitializeComponent();
        }

        public ToolbarPluginViewModel MyDataContext
        {
            get { return (ToolbarPluginViewModel)DataContext; }
        }

        public IExploripToolbar PluginLinked { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                MyDataContext.Init(this);
            }
        }
    }
}
