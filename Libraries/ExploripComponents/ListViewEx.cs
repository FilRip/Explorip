﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.Helpers;

namespace ExploripComponents;

public class ListViewEx : ListView
{
    private IEnumerable<OneFileSystem> _itemsSelectedBefore = [];

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
        if (DrawSelection)
        {
            DisableSelectInRectangle();
            return;
        }

        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneFileSystem)
            return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            return;
        UnselectAll();
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
        if ((Mouse.DirectlyOver is not FrameworkElement element || element.DataContext is not OneFileSystem) && !DrawSelection)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                UnselectAll();
            _itemsSelectedBefore = SelectedItems.OfType<OneFileSystem>();
            DrawSelection = true;
            DrawSelectionStart = e.GetPosition(this);
            Mouse.Capture(this);
        }
    }

    private void DisableSelectInRectangle()
    {
        DrawSelection = false;
        Mouse.Capture(null);
        InvalidateVisual();
    }

    private void SelectItems()
    {
        Rect rect = new(DrawSelectionStart, Mouse.GetPosition(this));
        foreach (OneFileSystem item in MainViewModel.FileListView.Where(i => i.IsItemVisible && !_itemsSelectedBefore.Contains(i)))
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
