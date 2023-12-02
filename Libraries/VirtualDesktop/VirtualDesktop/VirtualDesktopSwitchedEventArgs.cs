using System;

namespace WindowsDesktop
{
    public class VirtualDesktopSwitchedEventArgs : EventArgs
    {
        public VirtualDesktop Desktop { get; }

        public VirtualDesktopSwitchedEventArgs(VirtualDesktop desktop)
        {
            Desktop = desktop;
        }
    }
}
