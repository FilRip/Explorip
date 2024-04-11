//Copyright (c) Microsoft Corporation.  All rights reserved.

using System.ComponentModel;

namespace Microsoft.WindowsAPICodePack.Dialogs;

/// <summary>
/// Creates the event data associated with <see cref="CommonFileDialog.FolderChanging"/> event.
/// </summary>
/// 
/// <remarks>
/// Creates a new instance of this class.
/// </remarks>
/// <param name="folder">The name of the folder.</param>
public class CommonFileDialogFolderChangeEventArgs(string folder) : CancelEventArgs
{

    /// <summary>
    /// Gets or sets the name of the folder.
    /// </summary>
    public string Folder { get; set; } = folder;
}
