﻿using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Microsoft.WindowsAPICodePack.Shell.DesktopWindowManager;

/// <summary>
/// WPF Glass Window
/// Inherit from this window class to enable glass on a WPF window
/// </summary>
public class GlassWindow : Window
{
    #region properties

    /// <summary>
    /// Get determines if AeroGlass is enabled on the desktop. Set enables/disables AreoGlass on the desktop.
    /// </summary>
    public static bool AeroGlassCompositionEnabled
    {
        set
        {
            DesktopWindowManagerNativeMethods.DwmEnableComposition(
                value ? CompositionEnable.Enable : CompositionEnable.Disable);
        }
        get
        {
            return DesktopWindowManagerNativeMethods.DwmIsCompositionEnabled();
        }
    }

    #endregion

    #region events

    /// <summary>
    /// Fires when the availability of Glass effect changes.
    /// </summary>
    public event EventHandler<AeroGlassCompositionChangedEventArgs> AeroGlassCompositionChanged;

    #endregion

    #region operations

    /// <summary>
    /// Makes the background of current window transparent from both Wpf and Windows Perspective
    /// </summary>
    public void SetAeroGlassTransparency()
    {
        // Set the Background to transparent from Win32 perpective 
        HwndSource.FromHwnd(windowHandle).CompositionTarget.BackgroundColor = Colors.Transparent;

        // Set the Background to transparent from WPF perpective 
        Background = Brushes.Transparent;
    }

    /// <summary>
    /// Excludes a UI element from the AeroGlass frame.
    /// </summary>
    /// <param name="element">The element to exclude.</param>
    /// <remarks>Many non-WPF rendered controls (i.e., the ExplorerBrowser control) will not 
    /// render properly on top of an AeroGlass frame. </remarks>
    public void ExcludeElementFromAeroGlass(FrameworkElement element)
    {
        if (AeroGlassCompositionEnabled && element != null)
        {
            // calculate total size of window nonclient area
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            DesktopWindowManagerNativeMethods.GetWindowRect(hwndSource.Handle, out NativeRect windowRect);
            DesktopWindowManagerNativeMethods.GetClientRect(hwndSource.Handle, out NativeRect clientRect);
            Size nonClientSize = new(
                    windowRect.Right - windowRect.Left - (double)(clientRect.Right - clientRect.Left),
                    windowRect.Bottom - windowRect.Top - (double)(clientRect.Bottom - clientRect.Top));

            // calculate size of element relative to nonclient area
            GeneralTransform transform = element.TransformToAncestor(this);
            Point topLeftFrame = transform.Transform(new Point(0, 0));
            Point bottomRightFrame = transform.Transform(new Point(
                        element.ActualWidth + nonClientSize.Width,
                        element.ActualHeight + nonClientSize.Height));

            // Create a margin structure
            Margins margins = new()
            {
                LeftWidth = (int)topLeftFrame.X,
                RightWidth = (int)(ActualWidth - bottomRightFrame.X),
                TopHeight = (int)topLeftFrame.Y,
                BottomHeight = (int)(ActualHeight - bottomRightFrame.Y)
            };

            // Extend the Frame into client area
            DesktopWindowManagerNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }
    }

    /// <summary>
    /// Resets the AeroGlass exclusion area.
    /// </summary>
    public void ResetAeroGlass()
    {
        Margins margins = new(true);
        DesktopWindowManagerNativeMethods.DwmExtendFrameIntoClientArea(windowHandle, ref margins);
    }

    #endregion

    #region implementation
    private IntPtr windowHandle;

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == DwmMessages.WM_DWMCOMPOSITIONCHANGED
            || msg == DwmMessages.WM_DWMNCRENDERINGCHANGED)
        {
            AeroGlassCompositionChanged?.Invoke(this,
                    new AeroGlassCompositionChangedEventArgs(AeroGlassCompositionEnabled));

            handled = true;
        }
        return IntPtr.Zero;
    }

    /// <summary>
    /// OnSourceInitialized
    /// Override SourceInitialized to initialize windowHandle for this window.
    /// A valid windowHandle is available only after the sourceInitialized is completed
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        WindowInteropHelper interopHelper = new(this);
        windowHandle = interopHelper.EnsureHandle();

        // add Window Proc hook to capture DWM messages
        HwndSource source = HwndSource.FromHwnd(windowHandle);
        source.AddHook(new HwndSourceHook(WndProc));

        ResetAeroGlass();
    }

    #endregion
}
