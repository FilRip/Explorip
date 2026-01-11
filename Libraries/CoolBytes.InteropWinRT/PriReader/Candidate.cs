using CoolBytes.InteropWinRT.PriReader.Constants;

namespace CoolBytes.InteropWinRT.PriReader;

public class Candidate
{
    public ushort QualifierSet { get; }

    public EResourceValueType Type { get; }

    public ReferencedFileRef? SourceFile { get; }

    public DataItemRef? DataItem { get; }

    public ByteSpan? Data { get; }

    internal Candidate(ushort qualifierSet, EResourceValueType type, ReferencedFileRef? sourceFile, DataItemRef dataItem)
    {
        QualifierSet = qualifierSet;
        Type = type;
        SourceFile = sourceFile;
        DataItem = dataItem;
        Data = null;
    }

    internal Candidate(ushort qualifierSet, EResourceValueType type, ByteSpan data)
    {
        QualifierSet = qualifierSet;
        Type = type;
        SourceFile = null;
        DataItem = null;
        Data = data;
    }
}
