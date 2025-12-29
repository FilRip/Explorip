using System;
using System.Windows;

namespace VirtualDesktop.WPF;

public static class ApplicationExtensions
{
    /// <summary>
    /// Determines whether this application is pinned.
    /// </summary>
    /// <returns><see langword="true" /> if pinned, <see langword="false" /> otherwise.</returns>
    public static bool IsPinned(this Application app)
    {
        return VirtualDesktopManager.TryGetAppUserModelId(app.GetWindowHandle(), out string? appId)
            && VirtualDesktopManager.IsPinnedApplication(appId);
    }

    /// <summary>
    /// Pins an application, showing it on all virtual desktops.
    /// </summary>
    /// <returns><see langword="true" /> if already pinned or successfully pinned, <see langword="false" /> otherwise (most of the time, main window is not found).</returns>
    public static bool Pin(this Application app)
    {
        return VirtualDesktopManager.TryGetAppUserModelId(app.GetWindowHandle(), out string? appId)
            && VirtualDesktopManager.PinApplication(appId);
    }

    /// <summary>
    /// Unpins an application.
    /// </summary>
    /// <returns><see langword="true" /> if already unpinned or successfully unpinned, <see langword="false" /> otherwise (most of the time, main window is not found).</returns>
    public static bool Unpin(this Application app)
    {
        return VirtualDesktopManager.TryGetAppUserModelId(app.GetWindowHandle(), out string? appId)
            && VirtualDesktopManager.UnpinApplication(appId);
    }

    /// <summary>
    /// Toggles an application between being pinned and unpinned.
    /// </summary>
    /// <returns><see langword="true" /> if successfully toggled, <see langword="false" /> otherwise (most of the time, main window is not found).</returns>
    public static bool TogglePin(this Application app)
    {
        if (!VirtualDesktopManager.TryGetAppUserModelId(app.GetWindowHandle(), out string? appId))
            return false;

        return VirtualDesktopManager.IsPinnedApplication(appId) 
            ? VirtualDesktopManager.UnpinApplication(appId) 
            : VirtualDesktopManager.PinApplication(appId);
    }

    private static IntPtr GetWindowHandle(this Application app)
        => app.MainWindow?.GetHandle()
            ?? throw new InvalidOperationException();
}
