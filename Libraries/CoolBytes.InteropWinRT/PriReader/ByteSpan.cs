namespace CoolBytes.InteropWinRT.PriReader;

public struct ByteSpan(long offset, uint length)
{
    public long Offset { get; set; } = offset;
    public uint Length { get; set; } = length;
}
