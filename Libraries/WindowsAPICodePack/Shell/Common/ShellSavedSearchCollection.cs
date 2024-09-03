using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

/// <summary>
/// Represents a saved search
/// </summary>
public class ShellSavedSearchCollection : ShellSearchCollection
{
    internal ShellSavedSearchCollection(IShellItem2 shellItem)
        : base(shellItem)
    {
        CoreHelpers.ThrowIfNotVista();
    }
}
