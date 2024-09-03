using System;

namespace Microsoft.WindowsAPICodePack.Shell.ExplorerBrowser;

/// <summary>
/// The event argument for NavigationLogChangedEvent
/// </summary>
public class NavigationLogEventArgs : EventArgs
{
    /// <summary>
    /// Indicates CanNavigateForward has changed
    /// </summary>
    public bool CanNavigateForwardChanged { get; set; }

    /// <summary>
    /// Indicates CanNavigateBackward has changed
    /// </summary>
    public bool CanNavigateBackwardChanged { get; set; }

    /// <summary>
    /// Indicates the Locations collection has changed
    /// </summary>
    public bool LocationsChanged { get; set; }
}