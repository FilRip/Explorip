using System;

using VirtualDesktop.Interop.Proxy;

namespace VirtualDesktop;

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.Renamed" /> event.
/// </summary>
public class VirtualDesktopRenamedEventArgs(VirtualDesktop desktop, string name) : EventArgs
{
    public VirtualDesktop Desktop { get; } = desktop;

    public string Name { get; } = name;
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.WallpaperChanged" /> event.
/// </summary>
public class VirtualDesktopWallpaperChangedEventArgs(VirtualDesktop desktop, string path) : EventArgs
{
    public VirtualDesktop Desktop { get; } = desktop;

    public string Path { get; } = path;
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.CurrentChanged" /> event.
/// </summary>
public class VirtualDesktopChangedEventArgs(VirtualDesktop oldDesktop, VirtualDesktop newDesktop) : EventArgs
{
    public VirtualDesktop OldDesktop { get; } = oldDesktop;

    public VirtualDesktop NewDesktop { get; } = newDesktop;

    internal VirtualDesktopChangedEventArgs(IVirtualDesktop oldDesktop, IVirtualDesktop newDesktop)
        : this(oldDesktop.ToVirtualDesktop(), newDesktop.ToVirtualDesktop())
    {
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.CurrentChanged" /> event.
/// </summary>
public class VirtualDesktopMovedEventArgs(VirtualDesktop desktop, int oldIndex, int newIndex) : EventArgs
{
    public VirtualDesktop Desktop { get; } = desktop;

    public int OldIndex { get; } = oldIndex;

    public int NewIndex { get; } = newIndex;

    internal VirtualDesktopMovedEventArgs(IVirtualDesktop desktop, int oldIndex, int newIndex)
        : this(desktop.ToVirtualDesktop(), oldIndex, newIndex)
    {
    }
}

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

    internal VirtualDesktopDestroyEventArgs(IVirtualDesktop destroyed, IVirtualDesktop fallback)
        : this(destroyed.ToVirtualDesktop(), fallback.ToVirtualDesktop())
    {
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.Switched" /> event.
/// </summary>
public class VirtualDesktopSwitchedEventArgs(VirtualDesktop desktop) : EventArgs
{
    public VirtualDesktop Desktop { get; } = desktop;

    internal VirtualDesktopSwitchedEventArgs(IVirtualDesktop desktop)
        : this(desktop.ToVirtualDesktop())
    {
    }
}

/// <summary>
/// Provides data for the <see cref="VirtualDesktop.RemoteConnected" /> event.
/// </summary>
public class RemoteVirtualDesktopConnectedEventArgs(VirtualDesktop desktop) : EventArgs
{
    public VirtualDesktop Desktop { get; } = desktop;

    internal RemoteVirtualDesktopConnectedEventArgs(IVirtualDesktop desktop)
        : this(desktop.ToVirtualDesktop())
    {
    }
}
