namespace CoolBytes.InteropWinRT.PriReader;

public struct ReferencedFileRef
{
    internal int fileIndex;

    internal ReferencedFileRef(int fileIndex)
    {
        this.fileIndex = fileIndex;
    }
}
