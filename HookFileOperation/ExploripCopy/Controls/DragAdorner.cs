using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ExploripCopy.Controls;

public class DragAdorner : Adorner
{
    private readonly Grid child = null;
    private readonly int _visualChildren;
    private Point _offset;
    private Size _size;

    public DragAdorner(UIElement adornedElement, Size size, Brush brush, double opacity, int visualChildren = 1)
        : base(adornedElement)
    {
        child = new()
        {
            Width = size.Width,
            Height = size.Height,
            IsHitTestVisible = false,
            Margin = new Thickness(0),
        };
        child.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(size.Height) });
        child.Children.Add(new Rectangle() { Fill = brush, Opacity = opacity });

        _visualChildren = visualChildren;
        _size = size;
    }

    public void AddElement(Brush elementToAdd, double opacity)
    {
        child.Height += _size.Height;
        child.RowDefinitions.Add(new RowDefinition() { Height = child.RowDefinitions[0].Height });
        Rectangle rect = new() { Fill = elementToAdd, Opacity = opacity };
        child.Children.Add(rect);
        Grid.SetRow(rect, child.RowDefinitions.Count - 1);
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
