using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using ManagedShell.Interop;

namespace Explorip.Helpers;

public partial class MyAdornerWindow : Window
{
    private Point _offset;

    public MyAdornerWindow()
    {
        InitializeComponent();
        IntPtr hwnd = new WindowInteropHelper(this).EnsureHandle();
        NativeMethods.SetWindowLong(hwnd, NativeMethods.EGetWindowLong.GWL_EXSTYLE, NativeMethods.GetWindowLong(hwnd, NativeMethods.EGetWindowLong.GWL_EXSTYLE) | (int)NativeMethods.ExtendedWindowStyles.WS_EX_TOOLWINDOW | (int)NativeMethods.ExtendedWindowStyles.WS_EX_TRANSPARENT | (int)NativeMethods.ExtendedWindowStyles.WS_EX_LAYERED);
    }

    public void SetImage(ImageSource img, Point offset, Pen border = null, double opacity = 1)
    {
        MyAdornerImage.Source = img;
        Width = img.Width + (border.Thickness * 2);
        Height = img.Height + (border.Thickness * 2);
        _offset = offset;
        UpdatePosition();
        if (border != null)
        {
            MyAdornerBorder.BorderThickness = new(border.Thickness);
            MyAdornerBorder.BorderBrush = border.Brush;
        }
        MyAdornerBorder.Opacity = opacity;
        MyAdornerImage.Opacity = opacity;
    }

    public void UpdatePosition()
    {
        System.Drawing.Point p = new();
        NativeMethods.GetCursorPos(ref p);
        double dpiScale = WpfScreenHelper.MouseHelper.MouseScreen.ScaleFactor;
        Left = (p.X - _offset.X * dpiScale) / dpiScale;
        Top = (p.Y - _offset.Y * dpiScale) / dpiScale;
    }
}
