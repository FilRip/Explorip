﻿using System;

namespace WindowsDesktop
{
    /// <summary>
    /// Provides data for the <see cref="VirtualDesktop.DestroyBegin" />, <see cref="VirtualDesktop.DestroyFailed" />, and <see cref="VirtualDesktop.Destroyed" /> events.
    /// </summary>
    public class VirtualDesktopDestroyEventArgs(VirtualDesktop destroyed, VirtualDesktop fallback) : EventArgs
    {
        /// <summary>
        /// Gets the virtual desktop that was destroyed.
        /// </summary>
        public VirtualDesktop Destroyed { get; } = destroyed;

        /// <summary>
        /// Gets the virtual desktop to be displayed after <see cref="Destroyed" /> is destroyed.
        /// </summary>
        public VirtualDesktop Fallback { get; } = fallback;
    }
}
