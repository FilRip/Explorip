using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.WindowsTray;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for NotifyBalloon.xaml
/// </summary>
public partial class NotifyBalloon : UserControl
{
    private DispatcherTimer _closeTimer;
    private NotificationBalloon _balloonInfo = new();

    public NotifyBalloon()
    {
        DataContext = _balloonInfo;
        InitializeComponent();
    }

    public void Show(NotificationBalloon balloonInfo, UIElement placementTarget)
    {
        _balloonInfo = balloonInfo;
        DataContext = _balloonInfo;

        PlaySound(_balloonInfo);

        BalloonPopup.PlacementTarget = placementTarget;
        BalloonPopup.Placement = PlacementMode.Custom;
        BalloonPopup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PlacePopup);
        BalloonPopup.IsOpen = true;

        _balloonInfo.SetVisibility(BalloonVisibility.Visible);

        StartTimer(balloonInfo.Timeout);
    }

    public CustomPopupPlacement[] PlacePopup(Size popupSize, Size targetSize, Point offset)
    {
        DpiScale dpiScale = VisualTreeHelper.GetDpi(this);
        CustomPopupPlacement placement = (AppBarEdge)ConfigManager.Edge switch
        {
            AppBarEdge.Top => new CustomPopupPlacement(new Point((popupSize.Width * -1) + (offset.X * dpiScale.DpiScaleX),
                                   targetSize.Height + (offset.Y * dpiScale.DpiScaleY)),
                                   PopupPrimaryAxis.Horizontal),
            AppBarEdge.Left => new CustomPopupPlacement(new Point(offset.X * dpiScale.DpiScaleX,
                                    (popupSize.Height * -1) + (offset.Y * dpiScale.DpiScaleY)),
                                    PopupPrimaryAxis.Horizontal),
            _ => new CustomPopupPlacement(new Point((popupSize.Width * -1) + (offset.X * dpiScale.DpiScaleX),
                        (popupSize.Height * -1) + (offset.Y * dpiScale.DpiScaleY)),
                        PopupPrimaryAxis.Horizontal),// bottom or right taskbar
        };
        return [placement];
    }

    private void PlaySound(NotificationBalloon balloonInfo)
    {
        if (BalloonPopup.IsOpen)
        {
            return;
        }

        if ((balloonInfo.Flags & ManagedShell.Interop.NativeMethods.NIIF.NOSOUND) != 0)
        {
            return;
        }

        SoundHelper.PlayNotificationSound();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        CloseBalloon();
        _balloonInfo.SetVisibility(BalloonVisibility.Hidden);
        e.Handled = true;
    }

    private void CloseBalloon()
    {
        _closeTimer?.Stop();
        BalloonPopup.IsOpen = false;
    }

    private void StartTimer(int timeout)
    {
        _closeTimer?.Stop();

        _closeTimer = new DispatcherTimer
        {
            Interval = System.TimeSpan.FromMilliseconds(timeout)
        };

        _closeTimer.Tick += CloseTimer_Tick;
        _closeTimer.Start();
    }

    private void CloseTimer_Tick(object sender, System.EventArgs e)
    {
        CloseBalloon();
        _balloonInfo.SetVisibility(BalloonVisibility.TimedOut);
    }

    private void ContentControl_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // Prevent taskbar context menu from appearing
        e.Handled = true;
    }

    private void ContentControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _balloonInfo.Click();

        CloseBalloon();
        e.Handled = true;
    }
}
