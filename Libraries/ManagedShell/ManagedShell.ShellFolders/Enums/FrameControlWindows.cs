namespace ManagedShell.ShellFolders.Enums;

/// <summary>
/// A value that indicates the frame control to show or hide or -1 for fullscreen/kiosk mode
/// </summary>
public enum FrameControlWindows : uint
{
    /// <summary>The browser's media bar.</summary>
    FCW_INTERNETBAR = 0x0006,

    /// <summary>The browser's progress bar.</summary>
    FCW_PROGRESS = 0x0008,

    /// <summary>The browser's status bar.</summary>
    FCW_STATUS = 0x0001,

    /// <summary>The browser's toolbar.</summary>
    FCW_TOOLBAR = 0x0002,

    /// <summary>The browser's tree view.</summary>
    FCW_TREE = 0x0003,
}
