using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray;

public class NotificationBalloon
{
    public string Info { get; internal set; }

    public string Title { get; internal set; }

    public ENotifyIconInfos Flags { get; internal set; }

    public ImageSource Icon { get; internal set; }

    public int Timeout { get; internal set; }

    public DateTime Received { get; internal set; }

    public IntPtr HandleWindow { get; internal set; }

    public readonly NotifyIcon NotifyIcon;

    public NotificationBalloon() { }

    public NotificationBalloon(SafeNotifyIconData nicData, NotifyIcon notifyIcon)
    {
        NotifyIcon = notifyIcon;

        Title = nicData.InfoTitle;
        Info = nicData.Info;
        Flags = nicData.InfoFlags;
        Timeout = (int)nicData.Version;
        HandleWindow = nicData.Hwnd;

        Received = DateTime.Now;

        if (string.IsNullOrWhiteSpace(Title))
            Title = notifyIcon.Title;
        if (string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(notifyIcon.Path))
            Title = Path.GetFileNameWithoutExtension(notifyIcon.Path);

        if (Flags.HasFlag(ENotifyIconInfos.ERROR))
        {
            Icon = GetSystemIcon(SystemIcons.Error.Handle);
        }
        else if (Flags.HasFlag(ENotifyIconInfos.INFO))
        {
            Icon = GetSystemIcon(SystemIcons.Information.Handle);
        }
        else if (Flags.HasFlag(ENotifyIconInfos.WARNING))
        {
            Icon = GetSystemIcon(SystemIcons.Warning.Handle);
        }
        else if (Flags.HasFlag(ENotifyIconInfos.USER))
        {
            if (nicData.BalloonIconHandle != 0)
            {
                SetIconFromHIcon((IntPtr)nicData.BalloonIconHandle);
            }
            else if (nicData.IconHandle != IntPtr.Zero)
            {
                SetIconFromHIcon(nicData.IconHandle);
            }
        }
    }

    public void SetVisibility(BalloonVisibility visibility)
    {
        if (NotifyIcon == null)
        {
            ShellLogger.Error("NotificationBalloon: NotifyIcon is null");
            return;
        }

        switch (visibility)
        {
            case BalloonVisibility.Visible:
                NotifyIcon.SendMessage((uint)NotifyIconNotification.BALLOONSHOW, 0);
                break;
            case BalloonVisibility.Hidden:
                NotifyIcon.SendMessage((uint)NotifyIconNotification.BALLOONHIDE, 0);
                break;
            case BalloonVisibility.TimedOut:
                NotifyIcon.SendMessage((uint)NotifyIconNotification.BALLOONTIMEOUT, 0);
                break;
            default:
                break;
        }
    }

    public void Click()
    {
        if (NotifyIcon == null)
        {
            ShellLogger.Error("NotificationBalloon: NotifyIcon is null");
            return;
        }

        NotifyIcon.SendMessage((uint)NotifyIconNotification.BALLOONUSERCLICK, 0);
    }

    private void SetIconFromHIcon(IntPtr hIcon)
    {
        if (hIcon == IntPtr.Zero)
        {
            // Use default only if we don't have a valid icon already
            Icon ??= IconImageConverter.GetDefaultIcon();

            return;
        }

        ImageSource icon = IconImageConverter.GetImageFromHIcon(hIcon, false);

        if (icon != null)
        {
            Icon = icon;
        }
        else                 // Use default only if we don't have a valid icon already
            Icon ??= IconImageConverter.GetDefaultIcon();
    }

    private static BitmapSource GetSystemIcon(IntPtr hIcon)
    {
        BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        bs.Freeze();

        return bs;
    }
}
