using System.Collections.Generic;

namespace CoolBytes.InteropWinRT.PriReader;

public sealed class ResourceMapScope : ResourceMapEntry
{
#pragma warning disable CS8618
    internal ResourceMapScope(ushort index, ResourceMapScope? parent, string name) : base(index, parent, name)
#pragma warning restore CS8618
    {
    }

    public IReadOnlyList<ResourceMapEntry> Children { get; internal set; }

    public override string ToString()
    {
        return $"Scope {Index} {FullName} ({Children.Count} children)";
    }
}
