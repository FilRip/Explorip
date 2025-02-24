using System;

namespace WindowsDesktop
{
    /// <summary>
    /// Provides data for the <see cref="VirtualDesktop.CurrentChanged" /> event.
    /// </summary>
    public class VirtualDesktopChangedEventArgs(VirtualDesktop oldDesktop, VirtualDesktop newDesktop) : EventArgs
    {
        public VirtualDesktop OldDesktop { get; } = oldDesktop;
        public VirtualDesktop NewDesktop { get; } = newDesktop;
    }
}
