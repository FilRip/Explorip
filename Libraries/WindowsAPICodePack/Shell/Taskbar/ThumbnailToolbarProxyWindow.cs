﻿using System;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Interop.Taskbar;
using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.Taskbar;

internal class ThumbnailToolbarProxyWindow : NativeWindow, IDisposable
{
#pragma warning disable S1450 // Private fields only used as local variables in methods should become local variables
    private ThumbnailToolBarButton[] _thumbnailButtons;
#pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables
    private readonly IntPtr _internalWindowHandle;

    internal System.Windows.UIElement WindowsControl { get; set; }

    internal IntPtr WindowToTellTaskbarAbout
    {
        get
        {
            return _internalWindowHandle != IntPtr.Zero ? _internalWindowHandle : Handle;
        }
    }

    internal TaskbarWindow TaskbarWindow { get; set; }

    internal ThumbnailToolbarProxyWindow(IntPtr windowHandle, ThumbnailToolBarButton[] buttons)
    {
        if (windowHandle == IntPtr.Zero)
        {
            throw new ArgumentException(LocalizedMessages.CommonFileDialogInvalidHandle, "windowHandle");
        }
        if (buttons != null && buttons.Length == 0)
        {
            throw new ArgumentException(LocalizedMessages.ThumbnailToolbarManagerNullEmptyArray, "buttons");
        }

        _internalWindowHandle = windowHandle;
        _thumbnailButtons = buttons;

        // Set the window handle on the buttons (for future updates)
        Array.ForEach(_thumbnailButtons, new Action<ThumbnailToolBarButton>(UpdateHandle));

        // Assign the window handle (coming from the user) to this native window
        // so we can intercept the window messages sent from the taskbar to this window.
        AssignHandle(windowHandle);
    }

    internal ThumbnailToolbarProxyWindow(System.Windows.UIElement windowsControl, ThumbnailToolBarButton[] buttons)
    {
        if (buttons != null && buttons.Length == 0)
        {
            throw new ArgumentException(LocalizedMessages.ThumbnailToolbarManagerNullEmptyArray, nameof(buttons));
        }

        _internalWindowHandle = IntPtr.Zero;
        WindowsControl = windowsControl ?? throw new ArgumentNullException(nameof(windowsControl));
        _thumbnailButtons = buttons;

        // Set the window handle on the buttons (for future updates)
        Array.ForEach(_thumbnailButtons, new Action<ThumbnailToolBarButton>(UpdateHandle));
    }

    private void UpdateHandle(ThumbnailToolBarButton button)
    {
        button.WindowHandle = _internalWindowHandle;
        button.AddedToTaskbar = false;
    }

    protected override void WndProc(ref Message m)
    {
        bool handled;

        handled = TaskbarWindowManager.DispatchMessage(ref m, TaskbarWindow);

        // If it's a WM_Destroy message, then also forward it to the base class (our native window)
        if (m.Msg == (int)WindowMessage.Destroy ||
            m.Msg == (int)WindowMessage.NCDestroy ||
            m.Msg == (int)WindowMessage.SystemCommand && (int)m.WParam == TabbedThumbnailNativeMethods.ScClose || !handled)
        {
            base.WndProc(ref m);
        }
    }

    #region IDisposable Members

    /// <summary>
    /// 
    /// </summary>
    ~ThumbnailToolbarProxyWindow()
    {
        Dispose(false);
    }

    /// <summary>
    /// Release the native objects.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _isDisposed;
    public bool IsDisposed
    {
        get { return _isDisposed; }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources

                // Don't dispose the thumbnail buttons
                // as they might be used in another window.
                // Setting them to null will indicate we don't need use anymore.
                _thumbnailButtons = null;
            }
            _isDisposed = true;
        }
    }

    #endregion

}
