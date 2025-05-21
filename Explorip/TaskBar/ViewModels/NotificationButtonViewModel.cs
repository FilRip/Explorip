using System;
using System.Runtime.InteropServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.ViewModels;

public partial class NotificationButtonViewModel : ObservableObject
{
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(NumberOfNotificationsVisible))]
    private int _numberOfNotifications;

    [RelayCommand()]
    private void ShowNotification()
    {
        IntPtr ptrNotifCenterWindow = NativeMethods.FindWindow("Windows.UI.Core.CoreWindow", Constants.Localization.NOTIFICATION_CENTER);
        NativeMethods.DwmGetWindowAttribute(ptrNotifCenterWindow, NativeMethods.DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out uint cloaked, Marshal.SizeOf(typeof(bool)));
        ShellHelper.ShowNotificationCenter();
        Screen screen = WpfScreenHelper.MouseHelper.MouseScreen;
        int width = (int)(400 * screen.ScaleFactor);
        int top = (int)(screen.WorkingArea.Top * screen.ScaleFactor);
        if (cloaked != 0)
        {
            NativeMethods.Rect rect = new()
            {
                Top = top,
                Left = (int)(screen.WorkingArea.Right) - width,
                Bottom = (int)(screen.WorkingArea.Bottom),
                Right = (int)(screen.WorkingArea.Right),
            };
            IntPtr ptrRect = Marshal.AllocHGlobal(Marshal.SizeOf<NativeMethods.Rect>());
            Marshal.StructureToPtr(rect, ptrRect, false);
            NativeMethods.SendMessage(ptrNotifCenterWindow, NativeMethods.WM.DPICHANGED, NativeMethods.MakeLParam((int)(screen.ScaleFactor * 96), (int)(screen.ScaleFactor * 96)), ptrRect);
            NativeMethods.MoveWindow(ptrNotifCenterWindow, rect.Left, rect.Top, rect.Width, rect.Height, true);
            Marshal.FreeHGlobal(ptrRect);
            //NativeMethods.SetWindowPos(ptrNotifCenterWindow, IntPtr.Zero, (int)(screen.WorkingArea.Right) - width, top, width, (int)(screen.WorkingArea.Height), NativeMethods.SWP.SWP_SHOWWINDOW);
        }
        NumberOfNotifications = 0;
    }

    public void IncreaseNumberOfNotifications()
    {
        NumberOfNotifications++;
    }

    public bool NumberOfNotificationsVisible
    {
        get { return NumberOfNotifications > 0; }
    }
}
