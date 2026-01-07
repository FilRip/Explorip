namespace CoolBytes.JumpList.Models;

public class JumpListHeader
{
    public int Version { get; set; }

    public int CountEntries { get; set; }

    public int CountPinnedEntries { get; set; }

    public int LastId { get; set; }

    public int LastRevision { get; set; }
}
