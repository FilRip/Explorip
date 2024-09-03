// Copyright (c) Microsoft Corporation.  All rights reserved.

using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

/// <summary>
/// Represents a Non FileSystem folder (e.g. My Computer, Control Panel)
/// </summary>
public class ShellNonFileSystemFolder : ShellFolder
{
    #region Internal Constructors

    internal ShellNonFileSystemFolder()
    {
        // Empty            
    }

    internal ShellNonFileSystemFolder(IShellItem2 shellItem)
    {
        nativeShellItem = shellItem;
    }

    #endregion

}
