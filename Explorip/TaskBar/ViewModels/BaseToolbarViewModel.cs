using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;
using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

namespace Explorip.TaskBar.ViewModels;

public abstract partial class BaseToolbarViewModel : ObservableObject
{
    private double _startX, _startY;
    protected BaseToolbar _parentControl;

    public Taskbar ParentTaskbar { get; set; }

    public abstract string Id { get; }

    [ObservableProperty()]
    private Thickness _margin;

    [RelayCommand()]
    private void MouseDown()
    {
        if (!ParentTaskbar.MyDataContext.ResizeOn)
            return;

        Grid myGrid = _parentControl.FindVisualParent<Grid>();
        _startX = Mouse.GetPosition(myGrid).X - Margin.Left;
        _startY = Mouse.GetPosition(myGrid).Y - Margin.Top;

        Mouse.OverrideCursor = Cursors.ScrollAll;
        _parentControl.CaptureMouse();
    }

    [RelayCommand()]
    private void MouseUp()
    {
        if (_parentControl.IsMouseCaptured)
        {
            _parentControl.ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
            ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarPosition(Id, new Point(Margin.Left, Margin.Top));
        }
    }

    [RelayCommand()]
    private void MouseMove(MouseEventArgs e)
    {
        if (!_parentControl.IsMouseCaptured)
            return;

        Grid myGrid = _parentControl.FindVisualParent<Grid>();
        Margin = new Thickness(Math.Max(0, Mouse.GetPosition(myGrid).X - _startX), Margin.Top, Margin.Right, Margin.Bottom);
        HitTestResult result = VisualTreeHelper.HitTest(myGrid, e.GetPosition(myGrid));
        if (result?.VisualHit != null)
        {
            if (Margin.Left == 0)
            {
                BaseToolbar parent = result.VisualHit.FindVisualParent<BaseToolbar>();
                if (parent != null)
                {
                    int previousRow = Grid.GetRow(_parentControl);
                    int newRow = Grid.GetRow(parent);
                    if (previousRow != newRow)
                    {
                        Grid.SetRow(parent, previousRow);
                        Grid.SetRow(_parentControl, newRow);
                    }
                }
            }
            else
            {
                Margin = new Thickness(Margin.Left, Mouse.GetPosition(myGrid).Y - _startY, Margin.Right, Margin.Bottom);
                Toolbar parent = result.VisualHit.FindVisualParent<Toolbar>();
                if (parent != null)
                {
                    int previousRow = Grid.GetRow(_parentControl);
                    int newRow = Grid.GetRow(parent);
                    if (previousRow != newRow)
                    {
                        int previousColumn = Grid.GetColumn(_parentControl);
                        int newColumn = Grid.GetColumn(parent);
                        if (previousColumn == newColumn)
                        {
                            Grid.SetRow(_parentControl, newRow);
                            myGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            Grid.SetColumn(_parentControl, myGrid.ColumnDefinitions.Count - 1);
                            Margin = new Thickness(0, Margin.Top, Margin.Right, Margin.Bottom);
                            _startX = 0;
                        }
                        else
                        {
                            Grid.SetColumn(_parentControl, newColumn);
                            Grid.SetColumn(parent, previousColumn);
                        }
                    }
                }
            }
        }
        UpdateAfterMove();
    }

    protected virtual void UpdateAfterMove()
    {
        // Noting to do in base class
    }

    public virtual void Init(BaseToolbar parentControl)
    {
        _parentControl = parentControl;
        ParentTaskbar = parentControl.FindVisualParent<Taskbar>();
    }

    protected void DefaultSavedPosition()
    {
        Point point = ConfigManager.GetTaskbarConfig(ParentTaskbar.ScreenName).ToolbarPosition(Id);
        Margin = new Thickness(point.X, point.Y, Margin.Right, Margin.Bottom);
    }
}
