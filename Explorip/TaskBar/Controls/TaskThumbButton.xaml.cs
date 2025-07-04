using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour TaskThumbButton.xaml
/// </summary>
public partial class TaskThumbButton : Window
{
    private readonly List<IntPtr> _thumbPtr;
    private readonly List<Button> _listThumbnailButtons;

    public TaskThumbButton(TaskButton parent)
    {
        InitializeComponent();

        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        _thumbPtr = [];
        if (parent?.TaskbarParent == null)
        {
            Close();
            return;
        }

        // Set default value
        MyDataContext.ParentTask = parent;
        MyDataContext.CloseThumbnail = Close;

        _listThumbnailButtons = [];
        MyBorder.Background = ConfigManager.GetTaskbarConfig(parent.TaskbarParent.ScreenName).TaskbarBackground;
        MyBorder.CornerRadius = ConfigManager.ThumbnailCornerRadius;
        MyDataContext.ThumbWidth = ConfigManager.GetTaskbarConfig(parent.TaskbarParent.ScreenName).TaskbarThumbWidth;
        MyDataContext.ThumbHeight = ConfigManager.GetTaskbarConfig(parent.TaskbarParent.ScreenName).TaskbarThumbHeight;
        MainGrid.RowDefinitions[1].Height = new GridLength(MyDataContext.ThumbHeight, GridUnitType.Pixel);
        MainGrid.RowDefinitions[2].Height = new GridLength(ConfigManager.SpaceBetweenThumbnail, GridUnitType.Pixel);
        MainGrid.Margin = new Thickness(ConfigManager.SpaceBetweenThumbnail);
        Owner = parent.TaskbarParent;
        MyDataContext.WindowHandle = new WindowInteropHelper(this).EnsureHandle();

        // Build controls
        if (parent.ApplicationWindow.ListWindows.Count > 0)
        {
            Width = (MyDataContext.ThumbWidth + ConfigManager.SpaceBetweenThumbnail * 2) * parent.ApplicationWindow.ListWindows.Count;
            for (int i = 0; i < parent.ApplicationWindow.ListWindows.Count; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(MyDataContext.ThumbWidth, GridUnitType.Pixel) });

                StringBuilder sb = new(255);
                NativeMethods.GetWindowText(MyDataContext.ParentTask.ApplicationWindow.ListWindows[i], sb, 255);

                TextBlock txtTitle = new()
                {
                    Text = sb.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = Brushes.Transparent,
                    Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                    MaxWidth = MyDataContext.ThumbWidth - 16,
                };
                Button closeButton = new()
                {
                    Style = (Style)FindResource("CloseButtonStyle"),
                    Tag = i,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                };
                closeButton.Click += CloseButton_Click;
                Button thumbnailButton = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.Transparent,
                    Margin = new Thickness(0),
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.White,
                    Tag = i,
                    Style = (Style)FindResource("ButtonWithoutMouseOver"),
                };
                thumbnailButton.MouseEnter += ThumbnailButton_MouseEnter;
                thumbnailButton.MouseLeftButtonUp += ThumbnailButton_MouseLeftButtonUp;
                thumbnailButton.MouseRightButtonDown += ThumbnailButton_MouseRightButtonDown;

                MainGrid.Children.Add(txtTitle);
                MainGrid.Children.Add(closeButton);
                MainGrid.Children.Add(thumbnailButton);

                Grid.SetColumn(txtTitle, i);
                Grid.SetColumn(closeButton, i);
                Grid.SetColumn(thumbnailButton, i);
                Grid.SetRow(thumbnailButton, 1);

                _listThumbnailButtons.Add(thumbnailButton);
            }
        }

        // Calculate size
        Height = MainGrid.RowDefinitions.Sum(row => row.Height.Value) + ConfigManager.SpaceBetweenThumbnail;
        Screen screen = Screen.AllScreens.FirstOrDefault(s => s.DeviceName.EndsWith(parent.TaskbarParent.ScreenName));
        Point positionParent = MyDataContext.ParentTask.PointToScreen(Mouse.GetPosition(this));
        Left = (int)((positionParent.X - (Width / 2)) / screen.ScaleFactor);
        if (parent.ApplicationWindow.ListWindows.Count == 1)
            Left += Width / screen.ScaleFactor / 2;
        Top = parent.TaskbarParent.Top - Height;
    }

    private void ThumbnailButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        MyDataContext.MouseRightButtonDown();
    }

    private void ThumbnailButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        MyDataContext.MouseLeftButtonUp();
    }

    private void ThumbnailButton_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Button btn &&
            btn.Tag is int numWindow)
        {
            MyDataContext.CurrentWindow = numWindow;
        }
    }

    public TaskThumbButtonViewModel MyDataContext
    {
        get { return (TaskThumbButtonViewModel)DataContext; }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (MyDataContext.ShowContextMenu)
        {
            e.Cancel = true;
            return;
        }
        foreach (IntPtr thumb in _thumbPtr)
            NativeMethods.DwmUnregisterThumbnail(thumb);
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        try
        {
            WindowHelper.ExcludeWindowFromPeek(MyDataContext.WindowHandle);
            if (MyDataContext.ParentTask.ApplicationWindow.ListWindows.Count > 0)
            {
                double currentLeft = 0;
                for (int i = 0; i < MyDataContext.ParentTask.ApplicationWindow.ListWindows.Count; i++)
                {
                    currentLeft += ConfigManager.SpaceBetweenThumbnail;
                    int result = NativeMethods.DwmRegisterThumbnail(MyDataContext.WindowHandle, MyDataContext.ParentTask.ApplicationWindow.ListWindows[i], out IntPtr thumbPtr);
                    if (result == (int)NativeMethods.HResult.SUCCESS)
                    {
                        Point buttonPosition = _listThumbnailButtons[i].TransformToAncestor(this).Transform(new Point(0, 0));

                        NativeMethods.DwmThumbnailProperties thumbProp = new()
                        {
                            dwFlags = NativeMethods.DWM_TNP.VISIBLE | NativeMethods.DWM_TNP.RECTDESTINATION | NativeMethods.DWM_TNP.OPACITY,
                            fVisible = true,
                            opacity = 255,
                            rcDestination = new NativeMethods.Rect()
                            {
                                Left = (int)(currentLeft * VisualTreeHelper.GetDpi(this).DpiScaleX),
                                Top = (int)(buttonPosition.Y * VisualTreeHelper.GetDpi(this).DpiScaleY),
                                Right = (int)(MyDataContext.ThumbWidth * VisualTreeHelper.GetDpi(this).DpiScaleX) + (int)(currentLeft * VisualTreeHelper.GetDpi(this).DpiScaleX),
                                Bottom = (int)(MyDataContext.ThumbHeight * VisualTreeHelper.GetDpi(this).DpiScaleY) + (int)(buttonPosition.Y * VisualTreeHelper.GetDpi(this).DpiScaleY),
                            }
                        };

                        currentLeft += MyDataContext.ThumbWidth + ConfigManager.SpaceBetweenThumbnail;

                        NativeMethods.DwmUpdateThumbnailProperties(thumbPtr, ref thumbProp);
                        _thumbPtr.Add(thumbPtr);
                    }
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
        MyDataContext.Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn &&
            btn.Tag is int numWindow)
        {
            IntPtr windowHandle = MyDataContext.ParentTask.ApplicationWindow.ListWindows[numWindow];
            if (windowHandle == IntPtr.Zero)
                return;
            MyDataContext.UnPeek();
            NativeMethods.SendMessage(windowHandle, NativeMethods.WM.CLOSE, 0, 0);
            Close();
        }
    }
}
