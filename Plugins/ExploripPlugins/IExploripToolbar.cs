using System.Windows.Controls;
using System.Windows.Media;

using ManagedShell.AppBar;

namespace ExploripPlugins;

public interface IExploripToolbar
{
    string Author { get; }

    string Name { get; }

    string Description { get; }

    Version Version { get; }

    UserControl ExploripToolbar { get; }

    Guid GuidKey { get; }

    void UpdateTaskbar(int numScreen, double width, double height, Brush backgroundColor, AppBarEdge edge);

    double MinHeight { get; }

    double MinWidth { get; }

    void SetGlobalColors(SolidColorBrush background, SolidColorBrush foreground, SolidColorBrush accent);
}
