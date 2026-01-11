using System.IO;
using System.Text;

namespace CoolBytes.InteropWinRT.PriReader.Helpers;

internal static class ExtensionMethods
{
    private const string UnexpectedValueRead = "Unexpected value read.";

    public static string ReadString(this BinaryReader reader, Encoding encoding, int length)
    {
        using BinaryReader r = new(reader.BaseStream, encoding, true);

        return new string(r.ReadChars(length));
    }

    public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
    {
        using BinaryReader r = new(reader.BaseStream, encoding, true);

        StringBuilder result = new();
        char c;
        while ((c = r.ReadChar()) != '\0')
            result.Append(c);
        return result.ToString();
    }

    public static void ExpectByte(this BinaryReader reader, byte expectedValue)
    {
        if (reader.ReadByte() != expectedValue)
            throw new InvalidDataException(UnexpectedValueRead);
    }

    public static void ExpectUInt16(this BinaryReader reader, ushort expectedValue)
    {
        if (reader.ReadUInt16() != expectedValue)
            throw new InvalidDataException(UnexpectedValueRead);
    }

    public static void ExpectUInt32(this BinaryReader reader, uint expectedValue)
    {
        if (reader.ReadUInt32() != expectedValue)
            throw new InvalidDataException(UnexpectedValueRead);
    }

    public static void ExpectString(this BinaryReader reader, string s)
    {
        if (new string(reader.ReadChars(s.Length)) != s)
            throw new InvalidDataException(UnexpectedValueRead);
    }
}
