using System;

namespace WindowsDesktop
{
    /// <summary>
    /// Provides data for the <see cref="VirtualDesktop.Renamed" /> event.
    /// </summary>
    public class VirtualDesktopRenamedEventArgs(VirtualDesktop source, string oldName, string newName) : EventArgs
    {
        /// <summary>
        /// Gets the virtual desktop that was renamed.
        /// </summary>
        public VirtualDesktop Source { get; } = source;

        /// <summary>
        /// Gets the old name of the virtual desktop.
        /// </summary>
        public string OldName { get; } = oldName;

        /// <summary>
        /// Gets the new name of the virtual desktop.
        /// </summary>
        public string NewName { get; } = newName;
    }
}
