using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.ViewModels;

public partial class DesktopButtonViewModel : ObservableObject
{
    private Timer _timer;
    private IntPtr _thumbPtr = IntPtr.Zero;
    private Window _windowPreview = null;

    [RelayCommand()]
    private void MouseEnter()
    {
        _timer?.Dispose();
        _timer = new Timer(PreviewDesktop, null, ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        _timer?.Dispose();
        if (_thumbPtr != IntPtr.Zero)
        {
            NativeMethods.DwmUnregisterThumbnail(_thumbPtr);
            _thumbPtr = IntPtr.Zero;
        }
        _windowPreview?.Close();
        _windowPreview = null;
    }

    private void PreviewDesktop(object userData)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _windowPreview?.Close();
            Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
            _windowPreview = new()
            {
                WindowStyle = WindowStyle.None,
                Topmost = true,
                //Background = System.Windows.Media.Brushes.Transparent,
                Height = screen.WorkingArea.Height / screen.ScaleFactor,
                Width = screen.WorkingArea.Width / screen.ScaleFactor,
                AllowsTransparency = true,
                Left = screen.WpfWorkingArea.Left,
                Top = screen.WpfWorkingArea.Top,
                ShowInTaskbar = false,
            };
            _windowPreview.Show();
            IntPtr destPtr = new WindowInteropHelper(_windowPreview).EnsureHandle();
            IntPtr desktopPtr = NativeMethods.FindWindow("Progman", "Program Manager");
            NativeMethods.DwmRegisterThumbnail(destPtr, desktopPtr, out _thumbPtr);
            if (_thumbPtr != IntPtr.Zero)
            {
                NativeMethods.DwmThumbnailProperties preview = new()
                {
                    dwFlags = NativeMethods.DWM_TNP.VISIBLE | NativeMethods.DWM_TNP.OPACITY | NativeMethods.DWM_TNP.RECTDESTINATION | NativeMethods.DWM_TNP.RECTSOURCE,
                    fVisible = true,
                    opacity = 255,
                    rcSource = new NativeMethods.Rect((int)screen.WorkingArea.X, (int)screen.WorkingArea.Y, (int)screen.WorkingArea.Right, (int)screen.WorkingArea.Bottom),
                    rcDestination = new NativeMethods.Rect(0, 0, (int)screen.WorkingArea.Width, (int)screen.WorkingArea.Height),
                };
                NativeMethods.DwmUpdateThumbnailProperties(_thumbPtr, ref preview);
            }
        });
    }

    [RelayCommand()]
    private void ClickDesktop()
    {
        ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_D);
    }
}
