using System.IO;
using System.Text;

namespace CoolBytes.InteropWinRT.PriReader;

public struct ByteSpan(long offset, uint length)
{
    public long Offset { get; set; } = offset;

    public uint Length { get; set; } = length;

    public readonly string ReadString(Stream stream, Encoding? encoding = null)
    {
        byte[] buffer = new byte[Length];
        stream.Seek(Offset, SeekOrigin.Begin);
        _ = stream.Read(buffer, 0, (int)Length);
        encoding ??= Encoding.ASCII;
        string result = encoding.GetString(buffer).TrimEnd((char)0);
        return result;
    }
}
