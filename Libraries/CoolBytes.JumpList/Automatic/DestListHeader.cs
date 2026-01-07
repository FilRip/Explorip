using System;
using System.Text;

namespace CoolBytes.JumpList.Automatic;

public class DestListHeader(byte[] rawBytes)
{
    public int Version { get; } = BitConverter.ToInt32(rawBytes, 0);
    public int NumberOfEntries { get; } = BitConverter.ToInt32(rawBytes, 4);
    public int NumberOfPinnedEntries { get; } = BitConverter.ToInt32(rawBytes, 8);
    public float UnknownCounter { get; } = BitConverter.ToSingle(rawBytes, 12);
    public int LastEntryNumber { get; } = BitConverter.ToInt32(rawBytes, 16);
    public int Unknown1 { get; } = BitConverter.ToInt32(rawBytes, 20);
    public int LastRevisionNumber { get; } = BitConverter.ToInt32(rawBytes, 24);
    public int Unknown2 { get; } = BitConverter.ToInt32(rawBytes, 28);

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine($"Version: {Version}");
        sb.AppendLine($"NumberOfEntries: {NumberOfEntries}");
        sb.AppendLine($"NumberOfPinnedEntries: {NumberOfPinnedEntries}");
        sb.AppendLine($"LastEntryNumber: {LastEntryNumber}");
        sb.AppendLine($"LastRevisionNumber: {LastRevisionNumber}");
        sb.AppendLine($"Unknown0: {UnknownCounter}");
        sb.AppendLine($"AccessCount: {Unknown1}");
        sb.AppendLine($"Unknown2: {Unknown2}");

        return sb.ToString();
    }
}
