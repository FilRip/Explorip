using System.Windows.Controls;

namespace ExploripPlugins;

public interface IExploripToolbar
{
    string Author { get; set; }

    string Name { get; set; }

    string Description { get; set; }

    Version Version { get; set; }

    UserControl ExploripToolbar { get; set; }

    void SpecifyTaskbarSize(string screenName, int width, int height);
}
