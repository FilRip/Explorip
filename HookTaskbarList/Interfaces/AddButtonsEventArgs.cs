using HookTaskbarList.TaskbarList.Interfaces;

namespace HookTaskbarList.Interfaces;

public class AddButtonsEventArgs : TaskbarListEventArgs
{
    public uint NbButtons { get; set; }

    public ThumbButton[]? Buttons { get; set; }
}
