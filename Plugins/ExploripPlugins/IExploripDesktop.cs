using System.Windows.Controls;

namespace ExploripPlugins;

public interface IExploripDesktop
{
    string Author { get; }

    string Name { get; }

    string Description { get; }

    Version Version { get; }

    UserControl ExploripWidget { get; }

    Guid GuidKey { get; }
}
