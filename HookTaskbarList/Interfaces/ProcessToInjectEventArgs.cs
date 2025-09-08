using System;

namespace HookTaskbarList.Interfaces;

[Serializable()]
public class ProcessToInjectEventArgs : EventArgs
{
    public uint NumProcess { get; set; }
}
