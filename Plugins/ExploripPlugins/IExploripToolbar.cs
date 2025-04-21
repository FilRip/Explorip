using System.Windows.Controls;

namespace ExploripPlugins;

public interface IExploripToolbar
{
    string Author { get; }

    string Name { get; }

    string Description { get; }

    Version Version { get; }

    UserControl ExploripToolbar { get; }

    void SpecifyTaskbarSize(string screenName, int width, int height);

    double MinHeight { get; }

    double MinWidth { get; }
}
