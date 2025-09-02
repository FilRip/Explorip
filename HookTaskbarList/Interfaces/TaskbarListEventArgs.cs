using System;

namespace HookTaskbarList.Interfaces;

public class TaskbarListEventArgs
{
    public int ProcessId { get; set; }

    public IntPtr Handle { get; set; }
}
