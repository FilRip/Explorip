using ExploripPlugins.Models;

namespace ExploripPlugins;

public interface IExploripTab
{
    ITabExplorip Tab { get; }

    string Author { get; }

    string Name { get; }

    string Description { get; }

    Version Version { get; }

    Guid GuidKey { get; }
}
