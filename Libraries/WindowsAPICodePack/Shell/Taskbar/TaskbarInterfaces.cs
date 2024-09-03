using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Taskbar;

/// <summary>
/// Interface for jump list items
/// </summary>
public interface IJumpListItem
{
    /// <summary>
    /// Gets or sets this item's path
    /// </summary>
    string Path { get; set; }
}

/// <summary>
/// Interface for jump list tasks
/// </summary>
#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods
public abstract class JumpListTask
{
    internal abstract IShellLinkW NativeShellLink { get; }
}
#pragma warning restore S1694 // An abstract class should have both abstract and concrete methods
