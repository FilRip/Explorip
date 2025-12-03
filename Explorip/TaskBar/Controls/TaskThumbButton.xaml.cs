using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour TaskThumbButton.xaml
/// </summary>
public partial class TaskThumbButton : Window
{
    private readonly List<Button> _listCloseButton = [];
    private readonly List<Button> _listThumbnailButton = [];

    public TaskThumbButton(TaskButton parent)
    {
        InitializeComponent();

        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if (parent?.TaskbarParent == null)
        {
            Close();
            return;
        }

        // Set default value
        MyDataContext.ParentControl = this;
        MyDataContext.ParentTask = parent;

        MyDataContext.SpaceBetweenThumbnail = ConfigManager.SpaceBetweenThumbnail;
        MyBorder.Background = ConfigManager.GetTaskbarConfig(parent.TaskbarParent.NumScreen).TaskbarBackground;
        MyBorder.CornerRadius = ConfigManager.ThumbnailCornerRadius;
        MyDataContext.ThumbWidth = ConfigManager.GetTaskbarConfig(parent.TaskbarParent.NumScreen).TaskbarThumbWidth;
        MyDataContext.ThumbHeight = ConfigManager.GetTaskbarConfig(parent.TaskbarParent.NumScreen).TaskbarThumbHeight;
        MainGrid.RowDefinitions[1].Height = new GridLength(MyDataContext.ThumbHeight, GridUnitType.Pixel);
        MainGrid.RowDefinitions[2].Height = new GridLength(MyDataContext.SpaceBetweenThumbnail, GridUnitType.Pixel);
        MainGrid.Margin = new Thickness(MyDataContext.SpaceBetweenThumbnail);
        Owner = parent.TaskbarParent;
        MyDataContext.WindowHandle = new WindowInteropHelper(this).EnsureHandle();

        // Build controls
        if (parent.ApplicationWindow.ListWindows.Count > 0)
        {
            Width = (MyDataContext.ThumbWidth + MyDataContext.SpaceBetweenThumbnail * 2) * parent.ApplicationWindow.ListWindows.Count;
            int numColumn = -1;
            for (int i = 0; i < parent.ApplicationWindow.ListWindows.Count; i++)
            {
                numColumn++;
                if (i > 0 && i < parent.ApplicationWindow.ListWindows.Count)
                {
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(MyDataContext.SpaceBetweenThumbnail * 2, GridUnitType.Pixel) });
                    numColumn++;
                }
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
                Image icon = null;
                if (ConfigManager.ShowIconOnThumbnailWindow)
                {
                    txtTitle.Margin = new Thickness(17, 0, 0, 0);
                    txtTitle.MaxWidth -= 17;
                    icon = new()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Height = 16,
                        Width = 16,
                        Source = MyDataContext.ParentTask.ApplicationWindow.Icon,
                    };
                }
                Button closeButton = new()
                {
                    Style = (Style)FindResource("CloseButtonStyle"),
                    Tag = i,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                };
                closeButton.Click += CloseWindowButton_Click;
                _listCloseButton.Add(closeButton);

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
                thumbnailButton.SetBinding(Button.CommandProperty, new Binding(nameof(MyDataContext.ClickWindowCommand)));
                thumbnailButton.MouseRightButtonDown += ThumbnailButton_MouseRightButtonDown;
                _listThumbnailButton.Add(thumbnailButton);

                if (icon != null)
                {
                    MainGrid.Children.Add(icon);
                    Grid.SetColumn(icon, numColumn);
                }
                MainGrid.Children.Add(txtTitle);
                MainGrid.Children.Add(closeButton);
                MainGrid.Children.Add(thumbnailButton);

                Grid.SetColumn(txtTitle, numColumn);
                Grid.SetColumn(closeButton, numColumn);
                Grid.SetColumn(thumbnailButton, numColumn);
                Grid.SetRow(thumbnailButton, 1);

                MyDataContext.ListThumbnailButtons.Add(thumbnailButton);
            }
        }

        // Calculate size and position
        Height = MainGrid.RowDefinitions.Sum(row => row.Height.Value) + MyDataContext.SpaceBetweenThumbnail;
        Screen screen = Screen.AllScreens.FirstOrDefault(s => s.DisplayNumber == parent.TaskbarParent.NumScreen);
        Point positionParent = MyDataContext.ParentTask.PointToScreen(Mouse.GetPosition(this));
        Left = (int)((positionParent.X - (Width / 2)) / screen.ScaleFactor);
        if (parent.ApplicationWindow.ListWindows.Count == 1)
        {
            Left += Width / screen.ScaleFactor / 2;
            Left -= (Width - parent.ActualWidth) / 2;
        }

        // Position
        switch (parent.TaskbarParent.AppBarEdge)
        {
            case ManagedShell.AppBar.AppBarEdge.Left:
                Left = parent.TaskbarParent.Left + parent.TaskbarParent.ActualWidth;
                break;
            case ManagedShell.AppBar.AppBarEdge.Top:
                Top = parent.TaskbarParent.Top + parent.TaskbarParent.ActualHeight;
                break;
            case ManagedShell.AppBar.AppBarEdge.Right:
                Top = parent.TaskbarParent.Left - Width;
                break;
            case ManagedShell.AppBar.AppBarEdge.Bottom:
                Top = parent.TaskbarParent.Top - Height;
                break;
        }

        // Cancel System menu
        HwndSource.FromHwnd(MyDataContext.WindowHandle).AddHook(WndProc);
    }

    private void ThumbnailButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        MyDataContext.MouseRightButtonDown();
    }

    private void ThumbnailButton_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Button btn &&
            btn.Tag is int numWindow)
        {
            MyDataContext.MouseEnter(numWindow);
        }
    }

    public TaskThumbButtonViewModel MyDataContext
    {
        get { return (TaskThumbButtonViewModel)DataContext; }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = MyDataContext.ShowContextMenu;
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
        foreach (Button closeButton in _listCloseButton)
            closeButton.Click -= CloseWindowButton_Click;
        foreach (Button thumbnailButton in _listThumbnailButton)
        {
            thumbnailButton.MouseRightButtonDown -= ThumbnailButton_MouseRightButtonDown;
            thumbnailButton.MouseEnter -= ThumbnailButton_MouseEnter;
        }
        MyDataContext.Dispose();
    }

    private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn &&
            btn.Tag is int numWindow)
        {
            MyDataContext.CloseWindow(numWindow);
        }
    }

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == (int)NativeMethods.WM.SYSCOMMAND)
            handled = true;
        return IntPtr.Zero;
    }
}
