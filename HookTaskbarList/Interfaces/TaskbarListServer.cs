using System;

using HookTaskbarList.TaskbarList.Interfaces;

namespace HookTaskbarList.Interfaces;

public class TaskbarListServer : MarshalByRefObject
{
    public event EventHandler<AddButtonsEventArgs>? AddButtonsEvent;

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public void IsInstalled(int clientPID)
    {
        Console.WriteLine("Explorip has injected HookFileOperations into process {0}.", clientPID);
    }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

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
}
