using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.Helpers;

using ExploripComponents.Models;
using ExploripComponents.ViewModels;

namespace ExploripComponents.Controls;

public class ListViewEx : ListView
{
    private List<string> _itemsSelectedBefore = [];
    private int _numFirstItem, _numLastItem;

    public ListViewEx()
    {
        PreviewMouseLeftButtonUp += ListView_PreviewMouseLeftButtonUp;
        PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
        PreviewMouseMove += ListView_PreviewMouseMove;
    }

    public WpfExplorerViewModel MainViewModel
    {
        get { return this.FindVisualParent<MainWindow>()!.MyDataContext; }
    }

    private void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (ClickOnScrollStep(e))
            return;
        if (DrawSelection)
        {
            MainViewModel.CurrentlyRenaming?.Rename();
            DisableSelectInRectangle();
            e.Handled = true;
            MainViewModel.ModeEditPath = false;
            return;
        }

        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneFileSystem)
            return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            return;
        UnselectAll();
    }

    private bool ClickOnScrollStep(MouseEventArgs e)
    {
        Point mousePos = e.GetPosition(this);
        ScrollViewer? sv = this.FindVisualChild<ScrollViewer>();
        if ((sv != null && sv.Visibility == Visibility.Visible) &&
            (mousePos.X > ActualWidth - SystemParameters.ScrollWidth ||
            mousePos.Y > ActualHeight - SystemParameters.ScrollHeight))
        {
            return true;
        }
        return false;
    }

    private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (DrawSelection)
        {
            InvalidateVisual();
            SelectItems();
        }
    }

    private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (ClickOnScrollStep(e))
            return;
        if ((Mouse.DirectlyOver is not FrameworkElement element || element.DataContext is not OneFileSystem) && !DrawSelection && Mouse.DirectlyOver is not System.Windows.Shapes.Rectangle)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                UnselectAll();
            _itemsSelectedBefore = SelectedItems.OfType<OneFileSystem>().Select(i => i.DisplayText).ToList();
            DrawSelection = true;
            DrawSelectionStart = e.GetPosition(this);
            Mouse.Capture(this);
        }
    }

    private void DisableSelectInRectangle()
    {
        if (DrawSelection)
        {
            DrawSelection = false;
            Mouse.Capture(null);
            InvalidateVisual();
            _scrolledSelectedElements.Clear();
        }
    }

    public void ResetCurrentSelection()
    {
        DisableSelectInRectangle();
        SelectedItems.Clear();
        foreach (OneFileSystem fs in MainViewModel.FileListView.Where(i => _itemsSelectedBefore.Contains(i.DisplayText)))
            SelectedItems.Add(fs);
    }

    private void SelectItems()
    {
        Rect rect = new(DrawSelectionStart, Mouse.GetPosition(this));
        SelectedItems.Clear();
        foreach (OneFileSystem item in MainViewModel.FileListView.Where(i => i.IsItemVisible && !_itemsSelectedBefore.Contains(i.DisplayText)))
        {
            if (ItemContainerGenerator.ContainerFromItem(item) is Control control)
            {
                GeneralTransform transform = control.TransformToAncestor(this);
                Rect bounds = transform.TransformBounds(new Rect(new Point(0, 0), control.RenderSize));
                if (rect.IntersectsWith(bounds))
                    SelectedItems.Add(item);
            }
        }
        foreach (OneFileSystem fs in MainViewModel.FileListView.Where(i => _itemsSelectedBefore.Contains(i.DisplayText)))
            SelectedItems.Add(fs);
        foreach (OneFileSystem fs in _scrolledSelectedElements)
            SelectedItems.Add(fs);
    }

    public bool DrawSelection { get; set; }

    public Point DrawSelectionStart { get; set; }

    private readonly Pen _selectionPen = new(ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject, 2);

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (DrawSelection)
        {
            Point dest = Mouse.GetPosition(this);
            dest.X = Math.Max(dest.X, 0);
            dest.Y = Math.Max(dest.Y, 0);
            drawingContext.DrawRectangle(ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject, _selectionPen, new Rect(DrawSelectionStart, dest));
        }
    }

    private readonly List<OneFileSystem> _scrolledSelectedElements = [];
    public void Scrolling(ScrollChangedEventArgs e)
    {
        SetVisibleItem(e);
        if (DrawSelection && e.VerticalChange != 0)
        {
            foreach (OneFileSystem fs in SelectedItems.OfType<OneFileSystem>())
                if (!_scrolledSelectedElements.Contains(fs))
                    _scrolledSelectedElements.Add(fs);
            InvalidateVisual();
            SelectItems();
        }
    }

    public void SetVisibleItem(ScrollChangedEventArgs e)
    {
        _numFirstItem = (int)e.VerticalOffset;
        _numLastItem = (int)e.VerticalOffset + (int)e.ViewportHeight;
    }
}
