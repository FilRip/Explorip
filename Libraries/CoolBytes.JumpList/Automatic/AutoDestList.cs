using System;

using CoolBytes.JumpList.ExtensionBlocks;

using Securify.ShellLink;

namespace CoolBytes.JumpList.Automatic;

public class AutoDestList(DestListEntry destEntry, Shortcut? lnk, int interactionCount)
{
    public string Hostname { get; } = destEntry.Hostname;
    public Guid VolumeDroid { get; } = destEntry.VolumeDroid;
    public Guid VolumeBirthDroid { get; } = destEntry.VolumeBirthDroid;
    public Guid FileDroid { get; } = destEntry.FileDroid;
    public Guid FileBirthDroid { get; } = destEntry.FileBirthDroid;
    public int EntryNumber { get; } = destEntry.EntryNumber;
    public int MRUPosition { get; } = destEntry.MRUPosition;
    public int InteractionCount { get; } = interactionCount;
    public DateTimeOffset CreatedOn { get; } = destEntry.CreationTime;
    public DateTimeOffset LastModified { get; } = destEntry.LastModified;
    public bool Pinned { get; } = destEntry.PinStatus != -1;

    public PropertySheet? Sps { get; } = destEntry.Sps;

    public string Path { get; } = destEntry.Path;
    public string MacAddress { get; } = destEntry.MacAddress;

    public Shortcut? Lnk { get; } = lnk;
}
