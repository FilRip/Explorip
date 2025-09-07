using System;

using HookTaskbarList.TaskbarList.Interfaces;

namespace HookTaskbarList.Interfaces;

public class TaskbarListServer : MarshalByRefObject
{
    public event EventHandler<AddButtonsEventArgs>? AddButtonsEvent;
    public event EventHandler<ProcessToInjectEventArgs>? ProcessToInjectEvent;

    /// <summary>
    /// Called to confirm that the IPC channel is still open / host application has not closed
    /// </summary>
    public void Ping()
    {
        // Just to test if server still exist
    }

    /// <summary>
    /// A window send buttons to thumbnail of taskbar
    /// </summary>
    /// <param name="processId">Id of the process who send buttons</param>
    /// <param name="hWndWindow">Pointer to the window who send buttons to taskbar</param>
    /// <param name="cButtons">Number of buttons</param>
    /// <param name="pButtons">Array of details for each button</param>
    public void AddButtons(int processId, IntPtr hWndWindow, uint cButtons, ThumbButton[] pButtons)
    {
        AddButtonsEvent?.Invoke(this, new AddButtonsEventArgs()
        {
            ProcessId = processId,
            Handle = hWndWindow,
            NbButtons = cButtons,
            Buttons = pButtons
        });
    }

    public void InjectInProcess(uint processId)
    {
        ProcessToInjectEvent?.Invoke(this, new ProcessToInjectEventArgs()
        {
            NumProcess = processId,
        });
    }
}
