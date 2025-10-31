using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ManagedShell.Common.Logging;
using ManagedShell.Interop;

namespace Explorip.Helpers;

public class DragGhostAdorner : Adorner
{
    private readonly VisualBrush _vBrush;
    private Point _offset;
    private Point _location;

    public DragGhostAdorner(UIElement adornedElement, Point? offset)
        : base(adornedElement)
    {
        IsHitTestVisible = false;
        if (offset.HasValue)
            _offset = offset.Value;
        _vBrush = new(adornedElement)
        {
            Opacity = 0.5,
        };
    }

    public void UpdatePosition(Point mousePosition)
    {
        _location = mousePosition;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Point p = _location;
        p.Offset(-_offset.X, -_offset.Y);
        drawingContext.DrawRectangle(_vBrush, null, new Rect(p, RenderSize));
    }

    #region Static

    private static DragGhostAdorner _ghostAdorner;
    private static UIElement _source;

    public static void StartDragGhost(UIElement source, MouseEventArgs e)
    {
        if (source == null)
            return;

        _source = source;
        _ghostAdorner = new(source, (e?.GetPosition(source)));
        AdornerLayer.GetAdornerLayer(source).Add(_ghostAdorner);
        ShellLogger.Debug("Start Ghost Adorner");
    }

    public static void UpdateDragGhost()
    {
        if (_ghostAdorner != null && _source != null)
        {
            try
            {
                System.Drawing.Point p = new();
                NativeMethods.GetCursorPos(ref p);
                Point mousePos = new(p.X, p.Y);
                _ghostAdorner.UpdatePosition(_source.PointFromScreen(mousePos));
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }

    public static void StopDragGhost()
    {
        ShellLogger.Debug("Stop Ghost Adorner");
        if (_ghostAdorner != null && _source != null)
        {
            AdornerLayer.GetAdornerLayer(_source)?.Remove(_ghostAdorner);
            _ghostAdorner = null;
            _source = null;
        }
    }

    #endregion
}
