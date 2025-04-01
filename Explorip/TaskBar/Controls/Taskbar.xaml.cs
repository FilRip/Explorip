using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Explorip.Helpers;
using Explorip.StartMenu.Window;
using Explorip.TaskBar.Utilities;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTray;

using Microsoft.WindowsAPICodePack.Shell.CommonFileDialogs;

using WindowsDesktop;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for Taskbar.xaml
/// </summary>
public partial class Taskbar : AppBarWindow
{
    private readonly bool _mainScreen;
    private readonly string _screenName;

    public Taskbar(StartMenuMonitor startMenuMonitor, AppBarScreen screen, AppBarEdge edge)
        : base(MyTaskbarApp.MyShellManager.AppBarManager, MyTaskbarApp.MyShellManager.ExplorerHelper, MyTaskbarApp.MyShellManager.FullScreenHelper, screen, edge, 0)
    {
        InitializeComponent();

        _mainScreen = screen.Primary;
        _screenName = screen.DeviceName.TrimStart('.', '\\');

        DataContext = new TaskbarViewModel(this);

        StartButton.StartMenuMonitor = startMenuMonitor;

        AllowsTransparency = ConfigManager.GetTaskbarConfig(ScreenName).TaskbarAllowsTransparency;
        SetFontSmoothing();

        _explorerHelper.HideExplorerTaskbar = true;

        // Layout rounding causes incorrect sizing on non-integer scales
        if (DpiHelper.DpiScale % 1 != 0)
            UseLayoutRounding = false;

        MinHeight = Math.Max(52, DesiredHeight);

        if (ConfigManager.GetTaskbarConfig(ScreenName).TaskbarHeight > 0)
            DesiredHeight = ConfigManager.GetTaskbarConfig(ScreenName).TaskbarHeight;
        if (ConfigManager.GetTaskbarConfig(ScreenName).TaskbarWidth > 0)
            DesiredWidth = ConfigManager.GetTaskbarConfig(ScreenName).TaskbarWidth;

        if (!ConfigManager.GetTaskbarConfig(ScreenName).ShowTaskManButton)
            SetShowTaskMan(false);
        if (!ConfigManager.GetTaskbarConfig(ScreenName).ShowWidgetButton)
            SetShowWidget(false);

        MyDataContext.TabTipVisible = ConfigManager.GetTaskbarConfig(ScreenName).ShowTabTip;
        MyDataContext.KeyboardLayoutVisible = ConfigManager.GetTaskbarConfig(ScreenName).ShowKeyboardLayout;

        if (ConfigManager.GetTaskbarConfig(ScreenName).TaskbarBackground != null)
            Background = ConfigManager.GetTaskbarConfig(ScreenName).TaskbarBackground;
        else
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;

        if (_mainScreen && ConfigManager.TaskbarReplaceStartMenu)
        {
            _ = new StartMenuWindow();
        }
    }

    public TaskbarViewModel MyDataContext
    {
        get { return (TaskbarViewModel)DataContext; }
    }

    public bool MainScreen
    {
        get { return _mainScreen; }
    }

    public string ScreenName
    {
        get { return _screenName; }
    }

    public bool IsReopening { get; set; }

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
            ConfigManager.Theme == DictionaryManager.THEME_DEFAULT)
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
        VisualTextRenderingMode = ConfigManager.GetTaskbarConfig(ScreenName).AllowFontSmoothing ? TextRenderingMode.Auto : TextRenderingMode.Aliased;
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

    #endregion

    protected override void CustomClosing()
    {
        if (AllowClose && !IsReopening)
        {
            _explorerHelper.HideExplorerTaskbar = false;
        }
    }

    private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
    {
        MyDataContext.ChangeEdge(AppBarEdge);
        if (_mainScreen)
        {
            MyTaskbarApp.MyShellManager.Tasks.Initialize(new TaskCategoryProvider());
            try
            {
                VirtualDesktopProvider.Default.Initialize().Wait();
                VirtualDesktopProvider.Default.TerminatedInitializedTask.Wait();
                if (!VirtualDesktopProvider.Default.Initialized)
                    throw new Exceptions.ExploripException("VirtualDesktop not initialized");
            }
            catch (Exception)
            {
                MessageBox.Show("Error during initialization of VirtualDesktop support." + Environment.NewLine + "VirtualDesktop will not be supported");
            }
        }
        string[] listToolbars = ConfigManager.ToolbarsPath;
        if (listToolbars?.Length > 0)
        {
            double newHeight = 52;
            foreach (string path in listToolbars.Where(p => ConfigManager.GetTaskbarConfig(ScreenName).ToolbarVisible(p)))
            {
                AddToolbar(path, false);
                newHeight += (ConfigManager.GetTaskbarConfig(ScreenName).ToolbarSmallSizeIcon(path) ? 16 : 32);
            }
        }
    }

    #region Move/stretch

    public void ChangeDesiredSize(double height, double width)
    {
        Height = height;
        Width = width;
        DesiredHeight = height;
        DesiredWidth = width;
        _appBarManager.SetWorkArea(Screen);
        ConfigManager.GetTaskbarConfig(ScreenName).TaskbarHeight = DesiredHeight;
        ConfigManager.GetTaskbarConfig(ScreenName).TaskbarWidth = DesiredWidth;
    }

    private void UnlockTaskbar_Click(object sender, RoutedEventArgs e)
    {
        if (ResizeMode == ResizeMode.NoResize)
        {
            ResizeMode = ResizeMode.CanResizeWithGrip;
            MyDataContext.ResizeOn = true;
        }
        else
        {
            ResizeMode = ResizeMode.NoResize;
            MyDataContext.ResizeOn = false;
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

    private Toolbar AddToolbar(string path, bool resize = true)
    {
        if (!Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
            return null;
        ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        Toolbar newToolbar = new();
        newToolbar.MyDataContext.Path = path;
        Grid.SetRow(newToolbar, ToolsBars.RowDefinitions.Count - 1);
        Grid.SetColumn(newToolbar, 0);
        ToolsBars.Children.Add(newToolbar);
        if (resize)
        {
            Height += 22;
            DesiredHeight = Height;
        }
        _appBarManager.SetWorkArea(Screen);
        return newToolbar;
    }

    private void AddToolbar_Click(object sender, RoutedEventArgs e)
    {
        CommonOpenFileDialog dialog = new()
        {
            IsFolderPicker = true,
        };
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            Toolbar newTb = AddToolbar(dialog.FileName);
            if (newTb != null)
            {
                if (MainScreen)
                {
                    ConfigManager.GetTaskbarConfig(ScreenName).TaskbarHeight = DesiredHeight;
                    ConfigManager.ToolbarsPath = [.. ToolsBars.Children.OfType<Toolbar>().Select(tb => tb.MyDataContext.Path)];
                }
                newTb.MyDataContext.ShowHideTitle();
                newTb.MyDataContext.CurrentShowLargeIcon = true;
                newTb.MyDataContext.ShowLargeIcon();
            }
        }
    }

    private Point _lastMousePosition;
    private void ShowHideTitleToolbar_Click(object sender, RoutedEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(ToolsBars, _lastMousePosition);
        if (result?.VisualHit != null)
        {
            Toolbar toolbar = result.VisualHit.FindVisualParent<Toolbar>();
            toolbar?.MyDataContext.ShowHideTitle();
        }
    }

    private void ShowSmallLargeIcon_Click(object sender, RoutedEventArgs e)
    {
        HitTestResult result = VisualTreeHelper.HitTest(ToolsBars, _lastMousePosition);
        if (result?.VisualHit != null)
        {
            Toolbar toolbar = result.VisualHit.FindVisualParent<Toolbar>();
            toolbar?.MyDataContext.ShowLargeIcon();
        }
    }

    #endregion

    private void SetShowTaskMan(bool state)
    {
        TaskManButton.Visibility = (state ? Visibility.Visible : Visibility.Collapsed);
        MenuShowTaskmgr.IsChecked = state;
    }

    private void MenuShowTaskmgr_Click(object sender, RoutedEventArgs e)
    {
        ConfigManager.GetTaskbarConfig(ScreenName).ShowTaskManButton = !ConfigManager.GetTaskbarConfig(ScreenName).ShowTaskManButton;
        SetShowTaskMan(ConfigManager.GetTaskbarConfig(ScreenName).ShowTaskManButton);
    }

    private void SetShowWidget(bool state)
    {
        WidgetsButton.Visibility = (state ? Visibility.Visible : Visibility.Collapsed);
        MenuShowWidget.IsChecked = state;
    }

    private void MenuShowWidget_Click(object sender, RoutedEventArgs e)
    {
        ConfigManager.GetTaskbarConfig(ScreenName).ShowWidgetButton = !ConfigManager.GetTaskbarConfig(ScreenName).ShowWidgetButton;
        SetShowWidget(ConfigManager.GetTaskbarConfig(ScreenName).ShowWidgetButton);
    }
}
