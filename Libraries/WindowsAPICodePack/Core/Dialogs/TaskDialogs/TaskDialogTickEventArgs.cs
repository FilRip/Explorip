//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;

namespace Microsoft.WindowsAPICodePack.Dialogs;

/// <summary>
/// The event data for a TaskDialogTick event.
/// </summary>
/// <remarks>
/// Initializes the data associated with the TaskDialog tick event.
/// </remarks>
/// <param name="ticks">The total number of ticks since the control was activated.</param>
public class TaskDialogTickEventArgs(int ticks) : EventArgs
{

    /// <summary>
    /// Gets a value that determines the current number of ticks.
    /// </summary>
    public int Ticks { get; private set; } = ticks;
}
