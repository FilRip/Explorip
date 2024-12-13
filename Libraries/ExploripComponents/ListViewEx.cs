using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.Helpers;

namespace ExploripComponents;

public class ListViewEx : ListView
{
    public ListViewEx()
    {
        this.PreviewMouseLeftButtonUp += ListView_PreviewMouseLeftButtonUp;
        this.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
        this.PreviewMouseMove += ListView_PreviewMouseMove;
    }

    public WpfExplorerViewModel MainViewModel
    {
        get { return this.FindVisualParent<MainWindow>().MyDataContext; }
    }

    private void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DrawSelection && SelectInRectangle())
            return;

        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneFileSystem)
            return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            return;
        UnselectAll();
    }

    private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (DrawSelection)
            InvalidateVisual();
    }

    private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ((Mouse.DirectlyOver is not FrameworkElement element || element.DataContext is not OneFileSystem) && !DrawSelection)
        {
            DrawSelection = true;
            DrawSelectionStart = e.GetPosition(this);
            Mouse.Capture(this);
        }
    }

    private bool SelectInRectangle()
    {
        DrawSelection = false;
        Mouse.Capture(null);
        InvalidateVisual();
        Rect rect = new(DrawSelectionStart, Mouse.GetPosition(this));
        if (rect.Width == 0 && rect.Height == 0)
            return false;
        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            UnselectAll();
        SelectItems(rect);
        return true;
    }

    private void SelectItems(Rect rect)
    {
        foreach (OneFileSystem item in MainViewModel.FileListView.Where(i => i.IsItemVisible))
        {
            if (ItemContainerGenerator.ContainerFromItem(item) is Control control)
            {
                GeneralTransform transform = control.TransformToAncestor(this);
                Rect bounds = transform.TransformBounds(new Rect(new Point(0, 0), control.RenderSize));
                if (rect.IntersectsWith(bounds))
                    SelectedItems.Add(item);
            }
        }
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
}
