﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.WindowsAPICodePack.Shell.Taskbar;

/// <summary>
/// Represents a custom category on the taskbar's jump list
/// </summary>
public class JumpListCustomCategory
{
    private string name;

    internal JumpListItemCollection<IJumpListItem> JumpListItems
    {
        get;
        private set;
    }

    /// <summary>
    /// Category name
    /// </summary>
    public string Name
    {
        get { return name; }
        set
        {
            if (value != name)
            {
                name = value;
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }


    /// <summary>
    /// Add JumpList items for this category
    /// </summary>
    /// <param name="items">The items to add to the JumpList.</param>
    public void AddJumpListItems(params IJumpListItem[] items)
    {
        if (items != null)
        {
            foreach (IJumpListItem item in items)
            {
                JumpListItems.Add(item);
            }
        }
    }

    /// <summary>
    /// Event that is triggered when the jump list collection is modified
    /// </summary>
    internal event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

    /// <summary>
    /// Creates a new custom category instance
    /// </summary>
    /// <param name="categoryName">Category name</param>
    public JumpListCustomCategory(string categoryName)
    {
        Name = categoryName;

        JumpListItems = [];
        JumpListItems.CollectionChanged += OnJumpListCollectionChanged;
    }

    internal void OnJumpListCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged(this, args);
    }


    internal void RemoveJumpListItem(string path)
    {
        List<IJumpListItem> itemsToRemove = [.. from i in JumpListItems
            where string.Equals(path, i.Path, StringComparison.OrdinalIgnoreCase)
            select i];

        // Remove matching items
        for (int i = 0; i < itemsToRemove.Count; i++)
        {
            JumpListItems.Remove(itemsToRemove[i]);
        }
    }
}
