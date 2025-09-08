using System;

using HookTaskbarList.TaskbarList.Interfaces;

namespace HookTaskbarList.Interfaces;

[Serializable()]
public class AddButtonsEventArgs : TaskbarListEventArgs
{
    public uint NbButtons { get; set; }

    public ThumbButton[]? Buttons { get; set; }
}
