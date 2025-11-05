using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ManagedShell.Common.Logging;
using ManagedShell.Interop;

namespace Explorip.Helpers;

public class DragGhostAdorner : Adorner
{
    private readonly VisualBrush _vBrush;
    private readonly Pen _border;
    private Point _offset;
    private Point _location;

    public DragGhostAdorner(UIElement adornedElement, Point offset, Pen border = null, double opacity = 1)
        : base(adornedElement)
    {
        IsHitTestVisible = false;
        _offset = offset;
        _border = border;
        _vBrush = new(adornedElement)
        {
            Opacity = opacity,
        };
    }

    public void UpdatePosition(Point mousePosition)
    {
        _location = mousePosition;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ShellLogger.Debug("Call OnRender");
        Point p = _location;
        p.Offset(-_offset.X, -_offset.Y);
        drawingContext.DrawRectangle(_vBrush, _border, new Rect(p, RenderSize));
    }

    #region Static

    public static UIElement CreateImageOfWpfControl(UIElement source)
    {
        Rect bounds = VisualTreeHelper.GetDescendantBounds(source);
        RenderTargetBitmap rtb = new((int)bounds.Width,
                                     (int)bounds.Height,
                                     96,
                                     96,
                                     PixelFormats.Pbgra32);

        DrawingVisual dv = new();
        using (DrawingContext ctx = dv.RenderOpen())
        {
            VisualBrush vb = new(source);
            ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
        }

        rtb.Render(dv);

        Image image = new()
        {
            Source = rtb,
            Width = bounds.Width,
            Height = bounds.Height,
            Opacity = 1,
            IsHitTestVisible = false,
        };
        return image;
    }

    private static DragGhostAdorner _ghostAdorner;
    private static UIElement _relativeElement;

    public static void StartDragGhost(UIElement source, MouseEventArgs e, UIElement relativeElement)
    {
        if (source == null)
            return;

        _relativeElement = relativeElement;
        Point offset = e.GetPosition(_relativeElement);
        //_ghostAdorner = new(source, offset, new Pen(Brushes.Red, 1));
        _ghostAdorner = new(CreateImageOfWpfControl(source), offset, new Pen(Brushes.Red, 1));
        AdornerLayer.GetAdornerLayer(_relativeElement).Add(_ghostAdorner);
        ShellLogger.Debug("Start Ghost Adorner");
    }

    public static void UpdateDragGhost()
    {
        if (_ghostAdorner != null && _relativeElement != null)
        {
            try
            {
                System.Drawing.Point p = new();
                NativeMethods.GetCursorPos(ref p);
                Point location = _relativeElement.PointFromScreen(new Point(p.X, p.Y));
                _ghostAdorner.UpdatePosition(location);
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }

    public static void StopDragGhost()
    {
        if (_ghostAdorner != null && _relativeElement != null)
        {
            ShellLogger.Debug("Stop Ghost Adorner");
            AdornerLayer.GetAdornerLayer(_relativeElement).Remove(_ghostAdorner);
            _ghostAdorner = null;
            _relativeElement = null;
        }
    }

    #endregion
}
