using System;
using System.Windows;
using System.Windows.Interop;

using WpfScreenHelper.Enum;

namespace WpfScreenHelper;

/// <summary>
/// Provides helper functions for window class.
/// </summary>
public static class WindowScreenHelper
{
    /// <summary>
    /// Moves window to desired position on screen.
    /// </summary>
    /// <param name="window">Window to move.</param>
    /// <param name="x">X coordinate for moving.</param>
    /// <param name="y">Y coordinate for moving.</param>
    /// <param name="width">New width of the window.</param>
    /// <param name="height">New height of the window.</param>
    public static void SetWindowPosition(this Window window, int x, int y, int width, int height)
    {
        // The first move puts it on the correct monitor, which triggers WM_DPICHANGED
        // The +1/-1 coerces WPF to update Window.Top/Left/Width/Height in the second move
        NativeMethods.MoveWindow(new WindowInteropHelper(window).EnsureHandle(), x - 1, y, width + 1, height, false);
        NativeMethods.MoveWindow(new WindowInteropHelper(window).EnsureHandle(), x, y, width, height, true);
    }

    /// <summary>
    /// Moves window to desired position on screen.
    /// </summary>
    /// <param name="window">Window to move.</param>
    /// <param name="pos">Desired position.</param>
    /// <param name="screen">The screen to which we move.</param>
    public static void SetWindowPosition(this Window window, WindowPositions pos, Screen screen)
    {
        Rect coordinates = CalculateWindowCoordinates(window, pos, screen);

        window.SetWindowPosition((int)coordinates.X, (int)coordinates.Y, (int)coordinates.Width, (int)coordinates.Height);
    }

    /// <summary>
    /// Gets window position on screen with respect of screen scale factor.
    /// </summary>
    public static Rect GetWindowAbsolutePlacement(this Window window)
    {
        Rect placement = GetWindowPlacement(window);

        return new Rect(
            Math.Abs(placement.Left),
            Math.Abs(placement.Top),
            placement.Width,
            placement.Height);
    }

    public static Rect GetWindowPlacement(this Window window)
    {
        Screen screen = Screen.FromWindow(window);

        double left = (screen.WpfBounds.Left - window.Left) * screen.ScaleFactor;
        double top = (screen.WpfBounds.Top - window.Top) * screen.ScaleFactor;
        double width = window.Width * screen.ScaleFactor;
        double height = window.Height * screen.ScaleFactor;

        return new Rect(left, top, width, height);
    }

    /// <summary>
    /// Center a window on a specified screen
    /// </summary>
    /// <param name="window">Window to centered</param>
    /// <param name="screen">Screen/Monitor on which the window must be centered</param>
    public static void SetCenterOnScreen(this Window window, Screen screen)
    {
        if (window != null && screen != null)
        {
            window.Left = (screen.WpfWorkingArea.Left + (screen.WpfWorkingArea.Width / 2) - (window.Width / 2));
            window.Top = (screen.WpfWorkingArea.Top + (screen.WpfWorkingArea.Height / 2) - (window.Height / 2));
        }
    }

    /// <summary>
    /// Calculates window end position.
    /// </summary>
    private static Rect CalculateWindowCoordinates(FrameworkElement window, WindowPositions pos, Screen screen)
    {
        switch (pos)
        {
            case WindowPositions.Center:
                {
                    double x = screen.WpfBounds.X + (screen.WpfBounds.Width - window.Width) / 2.0;
                    double y = screen.WpfBounds.Y + (screen.WpfBounds.Height - window.Height) / 2.0;

                    return new Rect(x * screen.ScaleFactor, y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.Left:
                {
                    double y = screen.WpfBounds.Y + (screen.WpfBounds.Height - window.Height) / 2.0;

                    return new Rect(screen.WpfBounds.X * screen.ScaleFactor, y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.Top:
                {
                    double x = screen.WpfBounds.X + (screen.WpfBounds.Width - window.Width) / 2.0;

                    return new Rect(x * screen.ScaleFactor, screen.WpfBounds.Y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.Right:
                {
                    double x = screen.WpfBounds.X + (screen.WpfBounds.Width - window.Width);
                    double y = screen.WpfBounds.Y + (screen.WpfBounds.Height - window.Height) / 2.0;

                    return new Rect(x * screen.ScaleFactor, y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.Bottom:
                {
                    double x = screen.WpfBounds.X + (screen.WpfBounds.Width - window.Width) / 2.0;
                    double y = screen.WpfBounds.Y + (screen.WpfBounds.Height - window.Height);

                    return new Rect(x * screen.ScaleFactor, y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.TopLeft:
                return new Rect(screen.WpfBounds.X * screen.ScaleFactor, screen.WpfBounds.Y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);

            case WindowPositions.TopRight:
                {
                    double x = screen.WpfBounds.X + (screen.WpfBounds.Width - window.Width);

                    return new Rect(x * screen.ScaleFactor, screen.WpfBounds.Y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.BottomRight:
                {
                    double x = screen.WpfBounds.X + (screen.WpfBounds.Width - window.Width);
                    double y = screen.WpfBounds.Y + (screen.WpfBounds.Height - window.Height);

                    return new Rect(x * screen.ScaleFactor, y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.BottomLeft:
                {
                    double y = screen.WpfBounds.Y + (screen.WpfBounds.Height - window.Height);

                    return new Rect(screen.WpfBounds.X * screen.ScaleFactor, y * screen.ScaleFactor, window.Width * screen.ScaleFactor, window.Height * screen.ScaleFactor);
                }

            case WindowPositions.Maximize:
                return new Rect(screen.WpfBounds.X * screen.ScaleFactor, screen.WpfBounds.Y * screen.ScaleFactor, screen.WpfBounds.Width * screen.ScaleFactor, screen.WpfBounds.Height * screen.ScaleFactor);

            default:
                return Rect.Empty;
        }
    }
}