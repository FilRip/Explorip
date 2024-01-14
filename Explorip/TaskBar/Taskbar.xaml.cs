using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Explorip.Helpers;
using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Utilities;
using Explorip.TaskBar.ViewModels;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTray;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace Explorip.TaskBar;

/// <summary>
/// Interaction logic for Taskbar.xaml
/// </summary>
public partial class Taskbar : AppBarWindow
{
    private bool _isReopening;
    private readonly bool _mainScreen;

    public Taskbar(StartMenuMonitor startMenuMonitor, AppBarScreen screen, AppBarEdge edge)
        : base(MyTaskbarApp.MyShellManager.AppBarManager, MyTaskbarApp.MyShellManager.ExplorerHelper, MyTaskbarApp.MyShellManager.FullScreenHelper, screen, edge, 0)
    {
        _ = TaskbarViewModel.Instance;
        InitializeComponent();

        _mainScreen = screen.Primary;
        DataContext = MyTaskbarApp.MyShellManager;
        StartButton.StartMenuMonitor = startMenuMonitor;

        DesiredHeight = Application.Current.FindResource("TaskbarHeight") as double? ?? 0;
        DesiredWidth = Application.Current.FindResource("TaskbarWidth") as double? ?? 0;

        AllowsTransparency = Application.Current.FindResource("AllowsTransparency") as bool? ?? false;
        SetFontSmoothing();

        _explorerHelper.HideExplorerTaskbar = true;

        Settings.Instance.PropertyChanged += Settings_PropertyChanged;

        // Layout rounding causes incorrect sizing on non-integer scales
        if (DpiHelper.DpiScale % 1 != 0)
            UseLayoutRounding = false;

        if (Settings.Instance.ShowQuickLaunch)
        {
            QuickLaunchToolbar.Visibility = Visibility.Visible;
            DesiredHeight += 16;
        }

        MinHeight = DesiredHeight;
    }

    public bool MainScreen
    {
        get { return _mainScreen; }
    }

    protected override void OnSourceInitialized(object sender, EventArgs e)
    {
        base.OnSourceInitialized(sender, e);

        SetBlur(AllowsTransparency);
    }

    protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        base.WndProc(hwnd, msg, wParam, lParam, ref handled);

        if ((msg == (int)NativeMethods.WM.SYSCOLORCHANGE ||
            msg == (int)NativeMethods.WM.SETTINGCHANGE) &&
            Settings.Instance.Theme == DictionaryManager.THEME_DEFAULT)
        {
            handled = true;

            // If the color scheme changes, re-apply the current theme to get updated colors.
            ((MyTaskbarApp)Application.Current).DictionaryManager.SetThemeFromSettings();
        }

        return IntPtr.Zero;
    }

    public override void SetPosition()
    {
        base.SetPosition();

        MyTaskbarApp.MyShellManager.NotificationArea.SetTrayHostSizeData(new TrayHostSizeData()
        {
            edge = (NativeMethods.ABEdge)AppBarEdge,
            rc = new NativeMethods.Rect()
            {
                Top = (int)(Top * DpiScale),
                Left = (int)(Left * DpiScale),
                Bottom = (int)((Top + Height) * DpiScale),
                Right = (int)((Left + Width) * DpiScale),
            }
        });
    }

    private void SetFontSmoothing()
    {
        VisualTextRenderingMode = Settings.Instance.AllowFontSmoothing ? TextRenderingMode.Auto : TextRenderingMode.Aliased;
    }

    private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Theme")
        {
            bool newTransparency = Application.Current.FindResource("AllowsTransparency") as bool? ?? false;
            double newHeight = Application.Current.FindResource("TaskbarHeight") as double? ?? 0;
            double newWidth = Application.Current.FindResource("TaskbarWidth") as double? ?? 0;
            bool heightChanged = newHeight != DesiredHeight;
            bool widthChanged = newWidth != DesiredWidth;

            if (AllowsTransparency != newTransparency)
            {
                // Transparency cannot be changed on an open window.
                _isReopening = true;
                ((MyTaskbarApp)Application.Current).ReopenTaskbar();
                return;
            }

            DesiredHeight = newHeight;
            DesiredWidth = newWidth;

            if (Orientation == Orientation.Horizontal && heightChanged)
            {
                Height = DesiredHeight;
                SetScreenPosition();
            }
            else if (Orientation == Orientation.Vertical && widthChanged)
            {
                Width = DesiredWidth;
                SetScreenPosition();
            }
        }
        else if (e.PropertyName == "AllowFontSmoothing")
        {
            SetFontSmoothing();
        }
        else if (e.PropertyName == "ShowQuickLaunch")
        {
            if (Settings.Instance.ShowQuickLaunch)
            {
                QuickLaunchToolbar.Visibility = Visibility.Visible;
                DesiredHeight += 16;
            }
            else
            {
                QuickLaunchToolbar.Visibility = Visibility.Collapsed;
                DesiredHeight -= 16;
            }
        }
        else if (e.PropertyName == "Edge")
        {
            AppBarEdge = (AppBarEdge)Settings.Instance.Edge;
            SetScreenPosition();
        }
    }

    private void Taskbar_OnLocationChanged(object sender, EventArgs e)
    {
        // primarily for win7/8, they will set up the appbar correctly but then put it in the wrong place
        if (Orientation == Orientation.Vertical)
        {
            double desiredLeft = 0;

            if (AppBarEdge == AppBarEdge.Right)
            {
                desiredLeft = Screen.Bounds.Right / DpiScale - Width;
            }

            if (Left != desiredLeft)
                Left = desiredLeft;
        }
        else
        {
            double desiredTop = 0;

            if (AppBarEdge == AppBarEdge.Bottom)
            {
                desiredTop = Screen.Bounds.Bottom / DpiScale - Height;
            }

            if (Top != desiredTop)
                Top = desiredTop;
        }
    }

    #region Context menu

    private void TaskManagerMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        ShellHelper.StartTaskManager();
    }

    private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        ((MyTaskbarApp)Application.Current).ExitGracefully();
    }

    private void TaskbarAllScreenMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        ((MyTaskbarApp)Program.MyCurrentApp).ShowTaskbarOnAllOthersScreen();
    }

    private void MenuShowTabTip_Click(object sender, RoutedEventArgs e)
    {
        TaskbarViewModel.Instance.ShowTabTip = TaskbarViewModel.Instance.ShowTabTip == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        MenuShowTabTip.IsChecked = TaskbarViewModel.Instance.ShowTabTip == Visibility.Visible;
        ColumnVirtualKeyboard.Width = TaskbarViewModel.Instance.ShowTabTip == Visibility.Visible ? GridLength.Auto : new GridLength(0);
    }

    #endregion

    protected override void CustomClosing()
    {
        if (AllowClose)
        {
            if (!_isReopening)
                _explorerHelper.HideExplorerTaskbar = false;
            QuickLaunchToolbar.Visibility = Visibility.Collapsed;

            Settings.Instance.PropertyChanged -= Settings_PropertyChanged;
        }
    }

    private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_mainScreen)
            WindowsDesktop.VirtualDesktopProvider.Default.Initialize().Wait();
        MyTaskbarApp.MyShellManager.Tasks.Initialize(new TaskCategoryProvider());
    }

    #region Move/stretch

    public void ChangeDesiredSize(double height, double width)
    {
        Height = height;
        Width = width;
        DesiredHeight = height;
        DesiredWidth = width;
        _appBarManager.SetWorkArea(Screen);
    }

    private void UnlockTaskbar_Click(object sender, RoutedEventArgs e)
    {
        if (ResizeMode == ResizeMode.NoResize)
        {
            ResizeMode = ResizeMode.CanResizeWithGrip;
            TaskbarViewModel.Instance.ResizeOn = true;
        }
        else
        {
            ResizeMode = ResizeMode.NoResize;
            TaskbarViewModel.Instance.ResizeOn = false;
            DesiredHeight = Height;
            _appBarManager.SetWorkArea(Screen);
        }
    }

    private void Taskbar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _lastMousePosition = e.GetPosition(ToolsBars);
    }

    #endregion

    #region Manage toolbar

    private void AddToolbar_Click(object sender, RoutedEventArgs e)
    {
        CommonOpenFileDialog dialog = new()
        {
            IsFolderPicker = true,
        };
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Toolbar newToolbar = new()
            {
                Path = dialog.FileName,
            };
            Grid.SetRow(newToolbar, ToolsBars.RowDefinitions.Count - 1);
            Grid.SetColumn(newToolbar, 0);
            ToolsBars.Children.Add(newToolbar);
            Height += 22;
            DesiredHeight = Height;
            _appBarManager.SetWorkArea(Screen);
        }
    }

    private Point _lastMousePosition;
    private void ShowHideTitleToolbar_Click(object sender, RoutedEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(ToolsBars, _lastMousePosition);
        if (result?.VisualHit != null)
        {
            Toolbar toolbar = result.VisualHit.FindVisualParent<Toolbar>();
            toolbar?.ShowHideTitle_Click(sender, e);
        }
    }

    private void ShowSmallLargeIcon_Click(object sender, RoutedEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(ToolsBars, _lastMousePosition);
        if (result?.VisualHit != null)
        {
            Toolbar toolbar = result.VisualHit.FindVisualParent<Toolbar>();
            toolbar?.ShowLargeIcon_Click(sender, e);
        }
    }

    #endregion
}
