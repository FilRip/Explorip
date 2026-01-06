namespace CoolBytes.JumpList.Models;

public class JumpListHeader
{
    public ulong Version { get; set; }

    public ulong CountEntries { get; set; }

    public ulong LastId { get; set; }

    public ulong Reserved { get; set; }
}
