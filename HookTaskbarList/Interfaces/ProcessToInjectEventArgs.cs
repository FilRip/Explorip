using System;

namespace HookTaskbarList.Interfaces;

public class ProcessToInjectEventArgs : EventArgs
{
    public uint NumProcess { get; set; }
}
