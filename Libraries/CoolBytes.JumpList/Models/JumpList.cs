using System.Collections.Generic;

namespace CoolBytes.JumpList.Models;

public class JumpList
{
    public JumpListHeader Header { get; set; } = new();

    public List<JumpListItem> Items { get; set; } = [];
}
