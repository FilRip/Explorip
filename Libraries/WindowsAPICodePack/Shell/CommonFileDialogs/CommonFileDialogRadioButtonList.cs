﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Markup;

using Microsoft.WindowsAPICodePack.Shell.Interop.Dialogs;
using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.CommonFileDialogs;

/// <summary>
/// Represents a radio button list for the Common File Dialog.
/// </summary>
[ContentProperty("Items")]
public class CommonFileDialogRadioButtonList : CommonFileDialogControl, ICommonFileDialogIndexedControls
{
    private readonly Collection<CommonFileDialogRadioButtonListItem> items = [];
    /// <summary>
    /// Gets the collection of CommonFileDialogRadioButtonListItem objects
    /// </summary>
    public Collection<CommonFileDialogRadioButtonListItem> Items
    {
        get { return items; }
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    public CommonFileDialogRadioButtonList() { }

    /// <summary>
    /// Creates a new instance of this class with the specified name.
    /// </summary>
    /// <param name="name">The name of this control.</param>
    public CommonFileDialogRadioButtonList(string name) : base(name, string.Empty) { }

    #region ICommonFileDialogIndexedControls Members

    private int selectedIndex = -1;
    /// <summary>
    /// Gets or sets the current index of the selected item.
    /// </summary>
    public int SelectedIndex
    {
        get { return selectedIndex; }
        set
        {
            // Don't update this property if it hasn't changed
            if (selectedIndex == value) { return; }

            // If the native dialog has not been created yet
            if (HostingDialog == null)
            {
                selectedIndex = value;
            }
            else if (value >= 0 && value < items.Count)
            {
                selectedIndex = value;
                ApplyPropertyChange("SelectedIndex");
            }
            else
            {
#pragma warning disable S112
                throw new IndexOutOfRangeException(LocalizedMessages.RadioButtonListIndexOutOfBounds);
#pragma warning restore S112
            }
        }
    }

    /// <summary>
    /// Occurs when the user changes the SelectedIndex.
    /// </summary>
    /// 
    /// <remarks>
    /// By initializing the SelectedIndexChanged event with an empty
    /// delegate, we can skip the test to determine
    /// if the SelectedIndexChanged is null.
    /// test.
    /// </remarks>
    public event EventHandler SelectedIndexChanged = delegate { };

    /// <summary>
    /// Occurs when the user changes the SelectedIndex.
    /// </summary>
    /// <remarks>Because this method is defined in an interface, we can either
    /// have it as public, or make it private and explicitly implement (like below).
    /// Making it public doesn't really help as its only internal (but can't have this 
    /// internal because of the interface)
    /// </remarks>
    void ICommonFileDialogIndexedControls.RaiseSelectedIndexChangedEvent()
    {
        // Make sure that this control is enabled and has a specified delegate
        if (Enabled) { SelectedIndexChanged(this, EventArgs.Empty); }
    }

    #endregion

    /// <summary>
    /// Attach the RadioButtonList control to the dialog object
    /// </summary>
    /// <param name="dialog">The target dialog</param>
    internal override void Attach(IFileDialogCustomize dialog)
    {
        Debug.Assert(dialog != null, "CommonFileDialogRadioButtonList.Attach: dialog parameter can not be null");

        // Add the radio button list control
        dialog.AddRadioButtonList(Id);

        // Add the radio button list items
        for (int index = 0; index < items.Count; index++)
        {
            dialog.AddControlItem(Id, index, items[index].Text);
        }

        // Set the currently selected item
        if (selectedIndex >= 0 && selectedIndex < items.Count)
        {
            dialog.SetSelectedControlItem(Id, selectedIndex);
        }
        else if (selectedIndex != -1)
        {
#pragma warning disable S112
            throw new IndexOutOfRangeException(LocalizedMessages.RadioButtonListIndexOutOfBounds);
#pragma warning restore S112
        }

        // Sync unmanaged properties with managed properties
        SyncUnmanagedProperties();
    }
}

/// <summary>
/// Represents a list item for the CommonFileDialogRadioButtonList object.
/// </summary>
/// <remarks>
/// Creates a new instance of this class with the specified text.
/// </remarks>
/// <param name="text">The string that you want to display for this list item.</param>
public class CommonFileDialogRadioButtonListItem(string text)
{
    /// <summary>
    /// Gets or sets the string that will be displayed for this list item.
    /// </summary>
    public string Text { get; set; } = text;

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    public CommonFileDialogRadioButtonListItem() : this(string.Empty) { }
}
