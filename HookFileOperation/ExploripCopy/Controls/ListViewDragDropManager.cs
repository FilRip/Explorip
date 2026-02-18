using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ExploripCopy.Helpers;

namespace ExploripCopy.Controls;

public class ListViewDragDropManager<T> where T : class
{
    private bool _canInitiateDrag;
    private DragAdorner _dragAdorner;
    private int _indexToSelect;
    private T _itemUnderDragCursor;
    private readonly ListView _listView;
    private Point _ptMouseDown;
    private int _firstVisibleIndex, _nbVisibleItem;
    private Thread _threadScrollTo;
    private readonly int _speedScrolling;
    private readonly int _stepScrolling;

    private ListViewDragDropManager()
    {
        _canInitiateDrag = false;
        _indexToSelect = -1;
    }

    public ListViewDragDropManager(ListView listView, double dragAdornerOpacity = 0.7, int speedScrolling = 500, int stepScrolling = 50)
        : this()
    {
        _listView = listView;
        _listView.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
        _listView.PreviewMouseMove += ListView_PreviewMouseMove;
        _listView.DragOver += ListView_DragOver;
        _listView.DragLeave += ListView_DragLeave;
        _listView.DragEnter += ListView_DragEnter;
        _listView.Drop += ListView_Drop;

        ScrollViewer sv = _listView.GetScrollViewer();
        if (sv != null)
            sv.ScrollChanged += ScrollViewer_ScrollChanged;
        else
            _listView.Loaded += ListView_Loaded;

        DragAdornerOpacity = dragAdornerOpacity;
        _speedScrolling = speedScrolling;
        _stepScrolling = stepScrolling;
    }

    private void ListView_Loaded(object sender, RoutedEventArgs e)
    {
        _listView.Loaded -= ListView_Loaded;
        ScrollViewer sv = _listView.GetScrollViewer();
        if (sv != null)
            sv.ScrollChanged += ScrollViewer_ScrollChanged;
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        _firstVisibleIndex = (int)e.VerticalOffset;
        _nbVisibleItem = (int)e.ViewportHeight;
    }

    public double DragAdornerOpacity { get; set; }

    public bool IsDragInProgress { get; private set; }

    public ListView ListView
    {
        get { return _listView; }
    }

    public event EventHandler<ProcessDropEventArgs<T>> ProcessDrop;

    private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMouseOverScrollbar)
        {
            _canInitiateDrag = false;
            return;
        }

        int index = IndexUnderDragCursor;
        _canInitiateDrag = index > -1;

        if (_canInitiateDrag)
        {
            _ptMouseDown = ExtensionsWpf.GetMousePosition(_listView);
            _indexToSelect = index;
        }
        else
        {
            _ptMouseDown = new Point(short.MinValue, short.MinValue);
            _indexToSelect = -1;
        }
    }

    private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!CanStartDragOperation)
            return;

        if (_listView.SelectedIndex != _indexToSelect)
            _listView.SelectedIndex = _indexToSelect;

        if (_listView.SelectedItem == null)
            return;

        ListViewItem itemToDrag = GetListViewItem(_listView.SelectedIndex);
        if (itemToDrag == null)
            return;

        AdornerLayer adornerLayer = InitializeAdornerLayer(itemToDrag);

        IsDragInProgress = true;
        _canInitiateDrag = false;
        ListViewItemDragState.SetIsBeingDragged(itemToDrag, true);

        T selectedItem = _listView.SelectedItem as T;
        DragDropEffects allowedEffects = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        if (DragDrop.DoDragDrop(_listView, selectedItem, allowedEffects) != DragDropEffects.None)
            _listView.SelectedItem = selectedItem;

        FinishDragOperation(itemToDrag, adornerLayer);
    }

    private void ListView_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Move;

        UpdateDragAdornerLocation();

        int index = IndexUnderDragCursor;
        ItemUnderDragCursor = index < 0 ? null : ListView.Items[index] as T;

        if (index >= _firstVisibleIndex + _nbVisibleItem - 1)
            CreateLoopScroll(AutoScroll.Down);
        else if (index == _firstVisibleIndex && index > 0)
            CreateLoopScroll(AutoScroll.Up);
        else if (_threadScrollTo != null && _threadScrollTo.ThreadState != System.Threading.ThreadState.Aborted && _threadScrollTo.ThreadState != System.Threading.ThreadState.Stopped)
            _threadScrollTo.Abort();
    }

    private enum AutoScroll
    {
        Up = 0,
        Down = 1,
    }

    private void CreateLoopScroll(AutoScroll autoScroll)
    {
        if (_threadScrollTo == null)
            CreateThread();
        else if (_threadScrollTo.Name != "AutoScroll=" + autoScroll.ToString())
        {
            if (_threadScrollTo.ThreadState == System.Threading.ThreadState.Running)
                _threadScrollTo.Abort();
            CreateThread();
        }
        else if (_threadScrollTo.ThreadState == System.Threading.ThreadState.Stopped || _threadScrollTo.ThreadState == System.Threading.ThreadState.Aborted)
            CreateThread();

        void CreateThread()
        {
            _threadScrollTo = new Thread(new ParameterizedThreadStart(LoopScroll))
            {
                Name = "AutoScroll=" + autoScroll.ToString(),
            };
            _threadScrollTo.Start(autoScroll);
        }
    }

    private void LoopScroll(object userState)
    {
        try
        {
            int currentSpeed = _speedScrolling;
            Thread.Sleep(3000);
            if (userState is AutoScroll direction)
            {
                while (true)
                {
                    int indexItem = 0;
                    switch (direction)
                    {
                        case AutoScroll.Up:
                            indexItem = Math.Max(_firstVisibleIndex - 1, 0);
                            break;
                        case AutoScroll.Down:
                            indexItem = Math.Min(_firstVisibleIndex + _nbVisibleItem + 1, _listView.Items.Count - 1);
                            break;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            _listView.ScrollIntoView(_listView.Items[indexItem]);
                        }
                        catch (Exception) { /* Ignore errors */ }
                    });

                    Thread.Sleep(currentSpeed);
                    currentSpeed = Math.Max(50, currentSpeed - _stepScrolling);
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private void ListView_DragLeave(object sender, DragEventArgs e)
    {
        if (!IsMouseOver(_listView))
        {
            if (ItemUnderDragCursor != null)
                ItemUnderDragCursor = null;

            _dragAdorner?.Visibility = Visibility.Collapsed;
        }
    }

    private void ListView_DragEnter(object sender, DragEventArgs e)
    {
        if (_dragAdorner != null && _dragAdorner.Visibility != Visibility.Visible)
        {
            UpdateDragAdornerLocation();
            _dragAdorner.Visibility = Visibility.Visible;
        }
    }

    private void ListView_Drop(object sender, DragEventArgs e)
    {
        if (ItemUnderDragCursor != null)
            ItemUnderDragCursor = null;

        e.Effects = DragDropEffects.None;

        if (!e.Data.GetDataPresent(typeof(T)))
            return;

        if (e.Data.GetData(typeof(T)) is not T data)
            return;

        if (_listView.ItemsSource is not ObservableCollection<T> itemsSource)
            return;

        int oldIndex = itemsSource.IndexOf(data);
        int newIndex = IndexUnderDragCursor;

        if (newIndex < 0)
        {
            if (itemsSource.Count == 0)
                newIndex = 0;

            else if (oldIndex < 0)
                newIndex = itemsSource.Count;

            else
                return;
        }

        if (oldIndex == newIndex)
            return;

        if (ProcessDrop != null)
        {
            ProcessDropEventArgs<T> args = new(itemsSource, data, oldIndex, newIndex, e.AllowedEffects);
            ProcessDrop(this, args);
            e.Effects = args.Effects;
        }
        else
        {
            if (oldIndex > -1)
                itemsSource.Move(oldIndex, newIndex);
            else
                itemsSource.Insert(newIndex, data);

            e.Effects = DragDropEffects.Move;
        }
    }

    private bool CanStartDragOperation
    {
        get
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed || !_canInitiateDrag || _indexToSelect == -1 || !HasCursorLeftDragThreshold)
                return false;

            return true;
        }
    }

    private void FinishDragOperation(ListViewItem draggedItem, AdornerLayer adornerLayer)
    {
        ListViewItemDragState.SetIsBeingDragged(draggedItem, false);

        adornerLayer?.Remove(_dragAdorner);

        Stop(false);
    }

    private ListViewItem GetListViewItem(int index)
    {
        if (_listView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            return null;

        return _listView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
    }

    private ListViewItem GetListViewItem(T dataItem)
    {
        if (_listView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            return null;

        return _listView.ItemContainerGenerator.ContainerFromItem(dataItem) as ListViewItem;
    }

    private bool HasCursorLeftDragThreshold
    {
        get
        {
            if (_indexToSelect < 0)
                return false;

            ListViewItem item = GetListViewItem(_indexToSelect);
            Rect bounds = VisualTreeHelper.GetDescendantBounds(item);
            Point ptInItem = _listView.TranslatePoint(_ptMouseDown, item);

            double topOffset = Math.Abs(ptInItem.Y);
            double btmOffset = Math.Abs(bounds.Height - ptInItem.Y);
            double vertOffset = Math.Min(topOffset, btmOffset);

            double width = SystemParameters.MinimumHorizontalDragDistance * 2;
            double height = Math.Min(SystemParameters.MinimumVerticalDragDistance, vertOffset) * 2;
            Size szThreshold = new(width, height);

            Rect rect = new(_ptMouseDown, szThreshold);
            rect.Offset(szThreshold.Width / -2, szThreshold.Height / -2);
            Point ptInListView = ExtensionsWpf.GetMousePosition(_listView);
            return !rect.Contains(ptInListView);
        }
    }

    private int IndexUnderDragCursor
    {
        get
        {
            int index = -1;
            for (int i = _firstVisibleIndex; i < _firstVisibleIndex + _nbVisibleItem; ++i)
            {
                ListViewItem item = GetListViewItem(i);
                if (IsMouseOver(item))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }

    private AdornerLayer InitializeAdornerLayer(ListViewItem itemToDrag)
    {
        VisualBrush brush = new(itemToDrag);

        _dragAdorner = new DragAdorner(_listView, itemToDrag.RenderSize, brush)
        {
            Opacity = DragAdornerOpacity
        };

        AdornerLayer layer = AdornerLayer.GetAdornerLayer(_listView);
        layer.Add(_dragAdorner);

        _ptMouseDown = ExtensionsWpf.GetMousePosition(_listView);

        return layer;
    }

    private static bool IsMouseOver(Visual target)
    {
        Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
        Point mousePos = ExtensionsWpf.GetMousePosition(target);
        return bounds.Contains(mousePos);
    }

    private bool IsMouseOverScrollbar
    {
        get
        {
            Point ptMouse = ExtensionsWpf.GetMousePosition(_listView);
            HitTestResult res = VisualTreeHelper.HitTest(_listView, ptMouse);
            if (res == null)
                return false;

            DependencyObject depObj = res.VisualHit;
            while (depObj != null)
            {
                if (depObj is ScrollBar)
                    return true;

                if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else
                    depObj = LogicalTreeHelper.GetParent(depObj);
            }

            return false;
        }
    }

    private T ItemUnderDragCursor
    {
        get { return _itemUnderDragCursor; }
        set
        {
            if (_itemUnderDragCursor == value)
                return;

            ChangeState(false);

            _itemUnderDragCursor = value;

            ChangeState(true);
        }
    }

    private void ChangeState(bool state)
    {
        if (_itemUnderDragCursor != null)
        {
            ListViewItem currentItem = GetListViewItem(_itemUnderDragCursor);
            if (currentItem != null)
                ListViewItemDragState.SetIsUnderDragCursor(currentItem, state);
        }
    }

    private void UpdateDragAdornerLocation()
    {
        if (_dragAdorner != null)
        {
            Point ptCursor = ExtensionsWpf.GetMousePosition(ListView);

            double left = ptCursor.X - _ptMouseDown.X;

            ListViewItem itemBeingDragged = GetListViewItem(_indexToSelect);
            Point itemLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), ListView);
            double top = itemLoc.Y + ptCursor.Y - _ptMouseDown.Y;

            _dragAdorner.SetOffset(new Point(left, top));
        }
    }

    public void Stop(bool unregisterEvent = true)
    {
        if (unregisterEvent)
        {
            _listView.PreviewMouseLeftButtonDown -= ListView_PreviewMouseLeftButtonDown;
            _listView.PreviewMouseMove -= ListView_PreviewMouseMove;
            _listView.DragOver -= ListView_DragOver;
            _listView.DragLeave -= ListView_DragLeave;
            _listView.DragEnter -= ListView_DragEnter;
            _listView.Drop -= ListView_Drop;
        }

        IsDragInProgress = false;

        if (ItemUnderDragCursor != null)
            ItemUnderDragCursor = null;

        _dragAdorner = null;

        _threadScrollTo?.Abort();
    }
}
