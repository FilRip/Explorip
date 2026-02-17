using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploripCopy.Controls;

public class ProcessDropEventArgs<ItemType> : EventArgs where ItemType : class
{
    private readonly ObservableCollection<ItemType> itemsSource;
    private readonly ItemType dataItem;
    private readonly int oldIndex;
    private readonly int newIndex;
    private readonly DragDropEffects allowedEffects;

    internal ProcessDropEventArgs(
        ObservableCollection<ItemType> itemsSource,
        ItemType dataItem,
        int oldIndex,
        int newIndex,
        DragDropEffects allowedEffects)
    {
        this.itemsSource = itemsSource;
        this.dataItem = dataItem;
        this.oldIndex = oldIndex;
        this.newIndex = newIndex;
        this.allowedEffects = allowedEffects;
    }

    /// <summary>
    /// The items source of the ListView where the drop occurred.
    /// </summary>
    public ObservableCollection<ItemType> ItemsSource
    {
        get { return itemsSource; }
    }

    /// <summary>
    /// The data object which was dropped.
    /// </summary>
    public ItemType DataItem
    {
        get { return dataItem; }
    }

    /// <summary>
    /// The current index of the data item being dropped, in the ItemsSource collection.
    /// </summary>
    public int OldIndex
    {
        get { return oldIndex; }
    }

    /// <summary>
    /// The target index of the data item being dropped, in the ItemsSource collection.
    /// </summary>
    public int NewIndex
    {
        get { return newIndex; }
    }

    /// <summary>
    /// The drag drop effects allowed to be performed.
    /// </summary>
    public DragDropEffects AllowedEffects
    {
        get { return allowedEffects; }
    }

    /// <summary>
    /// The drag drop effect(s) performed on the dropped item.
    /// </summary>
    public DragDropEffects Effects { get; set; }
}
