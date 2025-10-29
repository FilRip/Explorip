using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Explorip.Helpers;

public class DragGhostAdorner : Adorner
{
    private readonly UIElement _visual;
    private Point _offset;
    private readonly AdornerLayer _layer;

    public DragGhostAdorner(UIElement adornedElement, UIElement visual)
        : base(adornedElement)
    {
        _visual = visual;
        IsHitTestVisible = false;
        _layer = AdornerLayer.GetAdornerLayer(adornedElement);
        _layer?.Add(this);
    }

    public void UpdatePosition(Point mousePosition)
    {
        _offset = mousePosition;
        _layer?.Update(AdornedElement);
    }

    public void Detach()
    {
        _layer?.Remove(this);
    }

    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int index) => _visual;

    protected override Size MeasureOverride(Size constraint)
    {
        _visual.Measure(constraint);
        return _visual.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _visual.Arrange(new Rect(_visual.DesiredSize));
        return finalSize;
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        GeneralTransform baseTransform = base.GetDesiredTransform(transform);
        TranslateTransform offsetTransform = new(_offset.X, _offset.Y);

        GeneralTransformGroup group = new();
        group.Children.Add(baseTransform);
        group.Children.Add(offsetTransform);
        return group;
    }

    #region Static

    private static DragGhostAdorner _ghost;

    public static void StartDragGhost(UIElement source, UIElement reference, Point mousePosition)
    {
        Border ghostVisual = new()
        {
            Width = source.RenderSize.Width,
            Height = source.RenderSize.Height,
            Background = new VisualBrush(source),
            Opacity = 0.5,
            IsHitTestVisible = false,
        };

        if (_ghost != null)
        {
            EndDragGhost();
        }
        _ghost = new(reference, ghostVisual);
        _ghost.UpdatePosition(mousePosition);
    }

    public static void UpdateDragGhost(Point mousePosition)
    {
        _ghost?.UpdatePosition(mousePosition);
    }

    public static void EndDragGhost()
    {
        _ghost?.Detach();
        _ghost = null;
    }

    #endregion
}
