namespace CoolBytes.InteropWinRT.PriReader;

public class HierarchicalSchemaReference
{
    public HierarchicalSchemaVersionInfo VersionInfo { get; }

    public uint Unknown1 { get; }

    public uint Unknown2 { get; }

    public string UniqueName { get; }

    internal HierarchicalSchemaReference(HierarchicalSchemaVersionInfo versionInfo, uint unknown1, uint unknown2, string uniqueName)
    {
        VersionInfo = versionInfo;
        Unknown1 = unknown1;
        Unknown2 = unknown2;
        UniqueName = uniqueName;
    }
}
