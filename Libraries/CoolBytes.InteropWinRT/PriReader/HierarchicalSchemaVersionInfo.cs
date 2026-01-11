namespace CoolBytes.InteropWinRT.PriReader;

public class HierarchicalSchemaVersionInfo(ushort majorVersion, ushort minorVersion, uint checksum, uint numScopes, uint numItems)
{
    public ushort MajorVersion { get; set; } = majorVersion;

    public ushort MinorVersion { get; set; } = minorVersion;

    public uint Checksum { get; set; } = checksum;

    public uint NumScopes { get; set; } = numScopes;

    public uint NumItems { get; set; } = numItems;
}
