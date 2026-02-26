using System.Windows;

using Explorip.TaskBar.ViewModels;

using ExploripPlugins;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour ToolbarPlugin.xaml
/// </summary>
public partial class ToolbarPlugin : BaseToolbar
{
    public ToolbarPlugin()
    {
        InitializeComponent();
    }

    public new ToolbarPluginViewModel DataContext
    {
        get { return (ToolbarPluginViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    public IExploripToolbar PluginLinked { get; set; }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded || !_ignoreReload)
        {
            _isLoaded = true;
            DataContext.Init(this);
        }
    }
}
