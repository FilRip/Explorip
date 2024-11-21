using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ExploripComponents;

public class ListViewEx : ListView
{
    public bool DrawSelection { get; set; }

    public Point DrawSelectionStart { get; set; }

    private readonly Pen _selectionPen = new(ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject, 2);

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (DrawSelection)
            drawingContext.DrawRectangle(ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject, _selectionPen, new Rect(DrawSelectionStart, Mouse.GetPosition(this)));
    }
}
