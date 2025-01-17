using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Threading.Tasks;

namespace Explorip.Explorer.Behaviors;

// http://peregrinesview.uk/wpf-behaviors-part-2-treeview/
public static class PerTreeViewItemHelper
{
    public static bool GetBringSelectedItemIntoView(TreeViewItem treeViewItem)
    {
        return (bool)treeViewItem.GetValue(BringSelectedItemIntoViewProperty);
    }

    public static void SetBringSelectedItemIntoView(TreeViewItem treeViewItem, bool value)
    {
        treeViewItem.SetValue(BringSelectedItemIntoViewProperty, value);
    }

    public static readonly DependencyProperty BringSelectedItemIntoViewProperty =
        DependencyProperty.RegisterAttached(
            "BringSelectedItemIntoView",
            typeof(bool),
            typeof(PerTreeViewItemHelper),
            new UIPropertyMetadata(false, BringSelectedItemIntoViewChanged));

    private static void BringSelectedItemIntoViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is not bool)
            return;

        if (obj is not TreeViewItem item)
            return;

        if ((bool)args.NewValue)
            item.Selected += OnTreeViewItemSelected;
        else
            item.Selected -= OnTreeViewItemSelected;
    }

    private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
    {
        // prevent this event bubbling up to any parent nodes
        e.Handled = true;

        if (sender is TreeViewItem item)
        {
            // use DispatcherPriority.ApplicationIdle so this occurs after all operations related to tree item expansion
            PerDispatcherHelper.AddToQueue(() => item.BringIntoView(), DispatcherPriority.ApplicationIdle);
        }
    }

    public static bool GetBringExpandedChildrenIntoView(TreeViewItem treeViewItem)
    {
        return (bool)treeViewItem.GetValue(BringExpandedChildrenIntoViewProperty);
    }

    public static void SetBringExpandedChildrenIntoView(TreeViewItem treeViewItem, bool value)
    {
        treeViewItem.SetValue(BringExpandedChildrenIntoViewProperty, value);
    }

    public static readonly DependencyProperty BringExpandedChildrenIntoViewProperty =
        DependencyProperty.RegisterAttached(
            "BringExpandedChildrenIntoView",
            typeof(bool),
            typeof(PerTreeViewItemHelper),
            new UIPropertyMetadata(false, BringExpandedChildrenIntoViewChanged));

    private static void BringExpandedChildrenIntoViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is not bool)
            return;

        if (obj is not TreeViewItem item)
            return;

        if ((bool)args.NewValue)
            item.Expanded += OnTreeViewItemExpanded;
        else
            item.Expanded -= OnTreeViewItemExpanded;
    }

    private static void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
    {
        // prevent this event bubbling up to any parent nodes
        e.Handled = true;

        if (sender is not TreeViewItem item)
            return;

        // use DispatcherPriority.ContextIdle for all actions related to tree item expansion
        // this ensures that all UI elements for any newly visible children are created before any selection operation

        // first bring the last child into view
        Action action = () =>
        {
            TreeViewItem lastChild = item.ItemContainerGenerator.ContainerFromIndex(item.Items.Count - 1) as TreeViewItem;
            lastChild?.BringIntoView();
        };

        PerDispatcherHelper.AddToQueue(action, DispatcherPriority.ContextIdle);

        // then bring the expanded item (back) into view
        action = () => { item.BringIntoView(); };

        PerDispatcherHelper.AddToQueue(action, DispatcherPriority.ContextIdle);
    }
}

public class PerMaxHeap<T> : PerBaseHeap<T> where T : IComparable<T>
{
    public PerMaxHeap()
        : base(EHeapType.Max)
    {
    }
}

// ================================================================================

public abstract class PerBaseHeap<T> where T : IComparable<T>
{
    protected enum EHeapType
    {
        Min,
        Max,
    }

    private readonly EHeapType _heapType;
    private int _capacity = 15;

    protected PerBaseHeap(EHeapType heapType)
    {
        _heapType = heapType;
        Clear();
    }

    /// <summary>
    ///  Clear all items from the heap, and reset status
    /// </summary>
    public void Clear()
    {
        Count = 0;
        Heap = new T[_capacity];
    }

    private T[] Heap { get; set; }

    /// <summary>
    /// How many items currently in the heap
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Are there any items in the heap?
    /// </summary>
    /// <returns></returns>
    public bool Any()
    {
        return Count > 0;
    }

    /// <summary>
    /// Add a new item to the heap and re-order so the best item is at the top of the heap
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(T item)
    {
        // grow the heap array if necessary
        if (Count == _capacity)
        {
            _capacity = _capacity * 2 + 1;
            T[] newHeap = new T[_capacity];
            Array.Copy(Heap, 0, newHeap, 0, Count);
            Heap = newHeap;
        }

        Heap[Count] = item;
        Count++;

        // clean up the heap, moving the new item to the appropriate level
        int currentIndex = Count - 1;
        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (RequiresSwap(currentIndex, parentIndex))
            {
                SwapItems(parentIndex, currentIndex);
                currentIndex = parentIndex;
            }
            else
                break;
        }
    }

    /// <summary>
    /// Add multiple items to the heap
    /// </summary>
    /// <param name="items"></param>
    public void Enqueue(params T[] items)
    {
        foreach (T item in items)
            Enqueue(item);
    }

    /// <summary>
    /// Remove an item from the heap and re-order so the best remaining item is at the top of the heap
    /// </summary>
    /// <remarks>
    /// Note that by rule, the two items in level 1 (array indexes 1 & 2), if they exist, each rank better than all of their children,
    /// and so one of these will be the new best item in the heap.
    /// </remarks>
    /// <returns></returns>
    public T Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException($"{nameof(Dequeue)}() called on an empty heap");

        T result = Heap[0];

        Count--;
        if (Count > 0)
            SwapItems(0, Count);

        Heap[Count] = default;

        // re-sort the heap as the previous last item in the array was swapped with the removed item (in index 0)
        int currentIndex = 0;

        // Keep walking the heap, placing best of three items (current + its two children) in highest position,
        // until no further swaps are required
        while (true)
        {
            int bestItemIndex = currentIndex;
            int leftChildIndex = currentIndex * 2 + 1;
            int rightChildIndex = currentIndex * 2 + 2;

            if (leftChildIndex < Count && RequiresSwap(leftChildIndex, currentIndex))
                bestItemIndex = leftChildIndex;

            if (rightChildIndex < Count && RequiresSwap(rightChildIndex, bestItemIndex))
                bestItemIndex = rightChildIndex;

            if (bestItemIndex == currentIndex)
                break;

            SwapItems(currentIndex, bestItemIndex);
            currentIndex = bestItemIndex;
        }

        return result;
    }

    /// <summary>
    /// Returns the item at the top of the heap without removing it
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException($"{nameof(Peek)}() called on an empty heap");

        return Heap[0];
    }

    /// <summary>
    /// Swap the two items at indexA and indexB
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    private void SwapItems(int indexA, int indexB)
    {
        if (indexA == indexB)
            return;

        (Heap[indexB], Heap[indexA]) = (Heap[indexA], Heap[indexB]);
    }

    /// <summary>
    /// returns true if the item in indexA ranks worse than the item in indexB
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    /// <returns></returns>
    private bool RequiresSwap(int indexA, int indexB)
    {
        if (indexA >= Count || indexB >= Count)
            return false;

        int compare = Heap[indexA].CompareTo(Heap[indexB]);

        return (_heapType == EHeapType.Min && compare < 0) || (_heapType == EHeapType.Max && compare > 0);
    }

    public override string ToString()
    {
        return string.Join(" ", Heap.Take(Count));
    }
}
public static class PerDispatcherHelper
{
    private static Dispatcher UIDispatcher { get; set; }

    public static void Initialise()
    {
        if (UIDispatcher != null && UIDispatcher.Thread.IsAlive)
            return;

        UIDispatcher = Dispatcher.CurrentDispatcher;
    }

    private static void CheckDispatcher()
    {
        if (UIDispatcher != null)
        {
            return;
        }

        const string errorMessage = "The Dispatcher Helper is not initialized.\r\n\r\n" +
                                    "Call perDispatcherHelper.Initialize() in the static App constructor.";

        throw new InvalidOperationException(errorMessage);
    }

    /// <summary>
    /// Invoke the action on the captured UI dispatcher, using the default priority
    /// </summary>
    /// <param name="action"></param>
    public static void InvokeOnUIContext(Action action)
    {
        InvokeOnUIContext(action, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Invoke the action on the captured UI dispatcher, using the specified priority
    /// </summary>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    public static void InvokeOnUIContext(Action action, DispatcherPriority priority)
    {
        _ = RunAsync(action, priority);
    }

    /// <summary>
    /// Invoke the action on the captured UI dispatcher, using the default priority
    /// and return an awaitable object
    /// </summary>
    /// <param name="action"></param>
    public static DispatcherOperation RunAsync(Action action)
    {
        return RunAsync(action, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Invoke the action on the captured UI dispatcher, using the specified priority
    /// and return an awaitable object
    /// </summary>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    public static DispatcherOperation RunAsync(Action action, DispatcherPriority priority)
    {
        CheckDispatcher();
        return UIDispatcher.BeginInvoke(action, priority);
    }

    // max heap keeps the highest sorting item at the top of the heap, without any requirement to exactly sort the remaining items
    private static readonly PerMaxHeap<PerDispatcherItemPair> PriorityQueue = new();

    /// <summary>
    /// Add a new operation to the queue, with the default priority 
    /// </summary>
    /// <remarks>
    /// Operations are executed in DispatcherPriority order (highest value executes first)
    /// </remarks>
    /// <param name="action"></param>
    public static void AddToQueue(Action action)
    {
        AddToQueue(action, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Add a new operation to the queue, with the specified priority 
    /// </summary>
    /// <remarks>
    /// Operations are executed in DispatcherPriority order (highest value executes first)
    /// </remarks>
    /// <param name="action"></param>
    /// <param name="dispatcherPriority"></param>
    public static void AddToQueue(Action action, DispatcherPriority dispatcherPriority)
    {
        PriorityQueue.Enqueue(new PerDispatcherItemPair(action, dispatcherPriority));
    }

    // stops the queue being processed more than once at a time
    private static bool IsProcessingQueue { get; set; }

    /// <summary>
    /// Execute each operation from the operations queue in order.
    /// </summary>
    /// <remarks>
    /// An operation may result in more items being added to the queue, which will be
    /// executed in the appropriate order.
    /// </remarks>
    /// <returns></returns>
    public static async Task ProcessQueueAsync()
    {
        if (IsProcessingQueue)
            return;

        IsProcessingQueue = true;

        try
        {
            while (PriorityQueue.Any())
            {
                // remove brings the next highest item to the top of the heap
                PerDispatcherItemPair pair = PriorityQueue.Dequeue();
                await RunAsync(pair.Action, pair.DispatcherPriority);
            }
        }
        finally
        {
            IsProcessingQueue = false;
        }
    }

    /// <summary>
    /// Clear any outstanding operations from the queue
    /// </summary>
    public static void ResetQueue()
    {
        PriorityQueue.Clear();
    }

    // ================================================================================

    /// <summary>
    /// Internal class representing a queued dispatcher operation.
    /// </summary>
#pragma warning disable S1210, S3260
    private class PerDispatcherItemPair(Action action, DispatcherPriority dispatcherPriority) : IComparable<PerDispatcherItemPair>
#pragma warning restore S1210, S3260
    {
        public Action Action { get; } = action;

        public DispatcherPriority DispatcherPriority { get; } = dispatcherPriority;

        // keep items with the same DispatcherPriority in the order they were added to the queue
        private long ItemIndex { get; } = long.MaxValue - DateTime.UtcNow.Ticks;

        public int CompareTo(PerDispatcherItemPair other)
        {
            int result = DispatcherPriority.CompareTo(other.DispatcherPriority);

            if (result == 0)
                result = ItemIndex.CompareTo(other.ItemIndex);

            return result;
        }
    }
}
