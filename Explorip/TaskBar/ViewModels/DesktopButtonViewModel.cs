using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.ViewModels;

public partial class DesktopButtonViewModel : ObservableObject
{
    private Timer _timer;
    private IntPtr _thumbPtr = IntPtr.Zero;
    private PreviewDesktopWindow _windowPreview = null;

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
        if (ConfigManager.ShowDesktopPreviewAllMonitors)
        {
            foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
                tb.DesktopButton.MyDataContext.HidePreviewWindow();
        }
        else
            HidePreviewWindow();
    }

    public void HidePreviewWindow()
    {
        if (_thumbPtr != IntPtr.Zero)
        {
            NativeMethods.DwmUnregisterThumbnail(_thumbPtr);
            _thumbPtr = IntPtr.Zero;
        }
        _windowPreview?.Close();
        _windowPreview = null;
    }

    public void ShowPreviewDesktop()
    {
        _windowPreview?.Close();
        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
        _windowPreview = new PreviewDesktopWindow()
        {
            Height = screen.WorkingArea.Height / screen.ScaleFactor,
            Width = screen.WorkingArea.Width / screen.ScaleFactor,
            Left = screen.WpfWorkingArea.Left,
            Top = screen.WpfWorkingArea.Top,
        };
        int lowerX = 0;
        int lowerY = 0;
        foreach (Rect srcRect in Screen.AllScreens.Select(s => s.WorkingArea))
        {
            lowerX = (int)Math.Min(lowerX, srcRect.X);
            lowerY = (int)Math.Min(lowerY, srcRect.Y);
        }
        lowerX = Math.Abs(lowerX);
        lowerY = Math.Abs(lowerY);
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
                rcSource = new NativeMethods.Rect(lowerX + (int)screen.WorkingArea.X, lowerY + (int)screen.WorkingArea.Y, lowerX + (int)screen.WorkingArea.Right, lowerY + (int)screen.WorkingArea.Bottom),
                rcDestination = new NativeMethods.Rect(0, 0, (int)screen.WorkingArea.Width, (int)screen.WorkingArea.Height),
            };
            NativeMethods.DwmUpdateThumbnailProperties(_thumbPtr, ref preview);
        }
    }

    private void PreviewDesktop(object userData)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ShowPreviewDesktop();
            if (ConfigManager.ShowDesktopPreviewAllMonitors && Screen.AllScreens.Count() > 1)
            {
                foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar().Where(tb => tb.Screen.NumScreen != WpfScreenHelper.MouseHelper.MouseScreen.DisplayNumber))
                    tb.DesktopButton.MyDataContext.ShowPreviewDesktop();
            }
        });
    }

    [RelayCommand()]
    private void ClickDesktop()
    {
        ShellHelper.ShellKeyCombo(NativeMethods.VK.LWIN, NativeMethods.VK.KEY_D);
    }
}
