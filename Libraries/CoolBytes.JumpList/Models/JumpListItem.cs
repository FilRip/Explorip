using System;

using Securify.ShellLink;

namespace CoolBytes.JumpList.Models;

public class JumpListItem
{
    public ulong Id { get; set; }

    public DateTime Timestamp { get; set; }

    public uint AccessCount { get; set; }

    public bool Pinned { get; set; }

    public ulong StreamNumber { get; set; }

    public Shortcut? Shortcut { get; set; }

    public void SetTimestamp(ulong timestamp)
    {
        Timestamp = DateTime.FromFileTimeUtc((long)timestamp);
    }

    public void AddShortcut(byte[] data)
    {
        Shortcut = Shortcut.FromByteArray(data);
    }
}
