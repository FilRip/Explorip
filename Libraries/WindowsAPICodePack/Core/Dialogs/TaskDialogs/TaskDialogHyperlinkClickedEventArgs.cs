using System;

namespace Microsoft.WindowsAPICodePack.Dialogs.TaskDialogs;

/// <summary>
/// Defines event data associated with a HyperlinkClick event.
/// </summary>
/// <remarks>
/// Creates a new instance of this class with the specified link text.
/// </remarks>
/// <param name="linkText">The text of the hyperlink that was clicked.</param>
public class TaskDialogHyperlinkClickedEventArgs(string linkText) : EventArgs
{

    /// <summary>
    /// Gets or sets the text of the hyperlink that was clicked.
    /// </summary>
    public string LinkText { get; set; } = linkText;
}
