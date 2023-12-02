using System;

namespace WindowsDesktop
{
    public class VirtualDesktopRemoteConnectedEventArgs : EventArgs
    {
        public VirtualDesktop Desktop { get; }

        public VirtualDesktopRemoteConnectedEventArgs(VirtualDesktop desktop)
        {
            Desktop = desktop;
        }
    }
}
