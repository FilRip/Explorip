using System.IO;

namespace CoolBytes.InteropWinRT.PriReader;

public class TocEntry(string sectionIdentifier, ushort flags, ushort sectionFlags, uint sectionQualifier, uint sectionOffset, uint sectionLength)
{
    public string SectionIdentifier { get; set; } = sectionIdentifier;
    public ushort Flags { get; set; } = flags;
    public ushort SectionFlags { get; set; } = sectionFlags;
    public uint SectionQualifier { get; set; } = sectionQualifier;
    public uint SectionOffset { get; set; } = sectionOffset;
    public uint SectionLength { get; set; } = sectionLength;

    internal static TocEntry Parse(BinaryReader binaryReader)
    {
        return new TocEntry(
            new string(binaryReader.ReadChars(16)),
            binaryReader.ReadUInt16(),
            binaryReader.ReadUInt16(),
            binaryReader.ReadUInt32(),
            binaryReader.ReadUInt32(),
            binaryReader.ReadUInt32());
    }

    public override string ToString()
    {
        return $"{SectionIdentifier.TrimEnd('\0', ' ')} length: {SectionLength}";
    }
}
