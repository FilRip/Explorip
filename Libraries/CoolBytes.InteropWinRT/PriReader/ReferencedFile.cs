namespace CoolBytes.InteropWinRT.PriReader;

public sealed class ReferencedFile : ReferencedEntry
{
    internal ReferencedFile(ReferencedFolder? parent, string name) : base(parent, name)
    {
    }
}
