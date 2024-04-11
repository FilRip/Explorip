//Copyright (c) Microsoft Corporation.  All rights reserved.

using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Taskbar;

/// <summary>
/// Represents a jump list item.
/// </summary>
/// <remarks>
/// Creates a jump list item with the specified path.
/// </remarks>
/// <param name="path">The path to the jump list item.</param>
/// <remarks>The file type should associate the given file  
/// with the calling application.</remarks>
public class JumpListItem(string path) : ShellFile(path), IJumpListItem
{
    #region IJumpListItem Members

    /// <summary>
    /// Gets or sets the target path for this jump list item.
    /// </summary>
    public new string Path
    {
        get
        {
            return base.Path;
        }
        set
        {
            base.ParsingName = value;
        }
    }

    #endregion
}
