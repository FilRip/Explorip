using System.Collections.Generic;

namespace CoolBytes.InteropWinRT.PriReader;

public sealed class ReferencedFolder : ReferencedEntry
{
#pragma warning disable CS8618
    internal ReferencedFolder(ReferencedFolder? parent, string name) : base(parent, name)
#pragma warning restore CS8618
    {
    }

    public IReadOnlyList<ReferencedEntry> Children { get; internal set; }
}
