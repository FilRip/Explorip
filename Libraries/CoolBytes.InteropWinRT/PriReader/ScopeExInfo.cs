namespace CoolBytes.InteropWinRT.PriReader;

internal struct ScopeExInfo(ushort scopeIndex, ushort childCount, ushort firstChildIndex)
{
    public ushort ScopeIndex = scopeIndex;
    public ushort ChildCount = childCount;
    public ushort FirstChildIndex = firstChildIndex;
}
