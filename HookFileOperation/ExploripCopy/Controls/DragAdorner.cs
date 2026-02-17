using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ExploripCopy.Controls;

public class DragAdorner : Adorner
{
    private readonly Rectangle child = null;
    private readonly int _visualChildren;
    private Point _offset;

    public DragAdorner(UIElement adornedElement, Size size, Brush brush, int visualChildren = 1)
        : base(adornedElement)
    {
        Rectangle rect = new()
        {
            Fill = brush,
            Width = size.Width,
            Height = size.Height,
            IsHitTestVisible = false
        };
        child = rect;
        _visualChildren = visualChildren;
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        GeneralTransformGroup result = new();
        result.Children.Add(base.GetDesiredTransform(transform));
        result.Children.Add(new TranslateTransform(_offset.X, _offset.Y));
        return result;
    }

    public void SetOffset(Point offset)
    {
        _offset = offset;
        UpdateLocation();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        child.Measure(constraint);
        return child.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        child.Arrange(new Rect(finalSize));
        return finalSize;
    }

    protected override Visual GetVisualChild(int index)
    {
        return child;
    }

    protected override int VisualChildrenCount
    {
        get { return _visualChildren; }
    }

    private void UpdateLocation()
    {
        AdornerLayer adornerLayer = Parent as AdornerLayer;
        adornerLayer?.Update(AdornedElement);
    }
}
