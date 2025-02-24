using System;

namespace WindowsDesktop
{
    /// <summary>
    /// Provides data for the <see cref="VirtualDesktop.WallpaperChanged" /> event.
    /// </summary>
    public class VirtualDesktopMovedEventArgs(VirtualDesktop source, int oldIndex, int newIndex) : EventArgs
    {
        /// <summary>
        /// Gets the virtual desktop that was moved.
        /// </summary>
        public VirtualDesktop Source { get; } = source;

        /// <summary>
        /// Gets the old index of the virtual desktop.
        /// </summary>
        public int OldIndex { get; } = oldIndex;

        /// <summary>
        /// Gets the new index of the virtual desktop.
        /// </summary>
        public int NewIndex { get; } = newIndex;
    }
}
