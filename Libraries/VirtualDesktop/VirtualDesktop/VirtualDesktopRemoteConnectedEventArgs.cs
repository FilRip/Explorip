using System;

namespace WindowsDesktop
{
    public class VirtualDesktopRemoteConnectedEventArgs(VirtualDesktop desktop) : EventArgs
    {
        public VirtualDesktop Desktop { get; } = desktop;
    }
}
