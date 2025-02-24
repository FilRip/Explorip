using System;

namespace WindowsDesktop
{
    public class VirtualDesktopSwitchedEventArgs(VirtualDesktop desktop) : EventArgs
    {
        public VirtualDesktop Desktop { get; } = desktop;
    }
}
