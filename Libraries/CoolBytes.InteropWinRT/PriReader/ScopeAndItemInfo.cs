namespace CoolBytes.InteropWinRT.PriReader;

internal struct ScopeAndItemInfo(ushort parent, ushort fullPathLength, byte flags, uint nameOffset, ushort index)
{
    public ushort Parent = parent;
    public ushort FullPathLength = fullPathLength;
    public byte Flags = flags;
    public uint NameOffset = nameOffset;
    public ushort Index = index;

    public readonly bool IsScope => (Flags & 0x10) != 0;
    public readonly bool NameInAscii => (Flags & 0x20) != 0;
}
