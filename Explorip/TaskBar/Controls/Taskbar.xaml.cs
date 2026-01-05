using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.StartMenu.Window;
using Explorip.TaskBar.Utilities;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;
using ManagedShell.WindowsTray;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for Taskbar.xaml
/// </summary>
public partial class Taskbar : AppBarWindow
{
    private readonly bool _mainScreen;
    private readonly int _numScreen;

    public Popup MyPopup { get; private set; }
    public int TimeBeforeAutoCloseThumb { get; private set; }

    public Taskbar(StartMenuMonitor startMenuMonitor, AppBarScreen screen)
        : base(MyTaskbarApp.MyShellManager.AppBarManager, MyTaskbarApp.MyShellManager.ExplorerHelper, MyTaskbarApp.MyShellManager.FullScreenHelper, screen, ConfigManager.GetTaskbarConfig(screen.NumScreen).Edge, 0)
    {
        InitializeComponent();

        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        _mainScreen = screen.Primary;
        _numScreen = screen.NumScreen;

        DataContext = new TaskbarViewModel(this);
        MyDataContext.TaskbarVisible = true;

        StartButton.StartMenuMonitor = startMenuMonitor;

        AllowsTransparency = ConfigManager.GetTaskbarConfig(_numScreen).TaskbarAllowsTransparency;
        SetFontSmoothing();

        _explorerHelper.HideExplorerTaskbar = true;

        SetSize();

        MyDataContext.TabTipVisible = ConfigManager.GetTaskbarConfig(NumScreen).ShowTabTip;
        MyDataContext.KeyboardLayoutVisible = ConfigManager.GetTaskbarConfig(NumScreen).ShowKeyboardLayout;

        if (ConfigManager.GetTaskbarConfig(NumScreen).TaskbarBackground != null)
            Background = ConfigManager.GetTaskbarConfig(NumScreen).TaskbarBackground;
        else
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush;

        if (_mainScreen && ConfigManager.TaskbarReplaceStartMenu)
        {
            _ = new StartMenuWindow();
        }

        MyPopup = new Popup()
        {
            AllowsTransparency = true,
            Child = new Border()
            {
                CornerRadius = ConfigManager.PopUpCornerRadius,
                BorderThickness = new Thickness(0),
                Background = Background,
            },
            StaysOpen = false,
        };
        ((Border)MyPopup.Child).Child = new ItemsControl()
        {
            Margin = new Thickness(ConfigManager.PopUpCornerRadius.TopLeft),
            Background = Brushes.Transparent,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
        };
        MyTaskbarApp.MyShellManager.TasksService.WindowActivated += ClosePopup;
        TimeBeforeAutoCloseThumb = ConfigManager.TaskbarDelayBeforeCloseThumbnail;

        PreviewKeyUp += Taskbar_PreviewKeyUp;
    }

    private void Taskbar_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && MyPopup.IsOpen)
        {
            try
            {
                ((ItemsControl)((Border)MyPopup.Child).Child).Items.Clear();
            }
            catch (Exception) { /* Ignore errors */ }
            MyPopup.IsOpen = false;
        }
#if DEBUG
        else if (e.Key == Key.F12)
        {
            CoolBytes.ScriptInterpreter.WPF.ExecuteScriptWindow executeScriptWindow = new();
            executeScriptWindow.Show();
        }
#endif
        else if (e.Key == Key.F11)
        {
            MyDataContext.ExpandCollapseTaskbar(!MyDataContext.TaskbarVisible);
        }
    }

    private void ClosePopup(object sender, WindowEventArgs e)
    {
        MyPopup.IsOpen = false;
    }

    #region Properties

    public TaskbarViewModel MyDataContext
    {
        get { return (TaskbarViewModel)DataContext; }
    }

    public bool MainScreen
    {
        get { return _mainScreen; }
    }

    public int NumScreen
    {
        get { return _numScreen; }
    }

    public bool IsReopening { get; set; }

    #endregion

    protected override void OnSourceInitialized(object sender, EventArgs e)
    {
        base.OnSourceInitialized(sender, e);

        SetBlur(AllowsTransparency);
    }

    protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        base.WndProc(hwnd, msg, wParam, lParam, ref handled);

        if (!handled)
        {
            if ((msg == (int)NativeMethods.WM.SYSCOLORCHANGE ||
                msg == (int)NativeMethods.WM.SETTINGCHANGE) &&
                ConfigManager.Theme == DictionaryManager.THEME_DEFAULT)
            {
                handled = true;

                // If the color scheme changes, re-apply the current theme to get updated colors.
                ((MyTaskbarApp)Application.Current).DictionaryManager.SetThemeFromSettings();
            }
            else if (msg == (int)NativeMethods.WM.SYSCOMMAND)
            {
                handled = true;
            }
        }

        return IntPtr.Zero;
    }

    #region Position and size

    public void SetPositionAndSize()
    {
        double previousDpi = Screen.DpiScale;
        AppBarScreen newScreen = AppBarScreen.FromAllScreens().FirstOrDefault(s => s.NumScreen == _numScreen);
        if (newScreen != null)
            Screen = newScreen;
        DpiScale = Screen.DpiScale;

        if (DpiScale != previousDpi)
        {
            if (ConfigManager.GetTaskbarConfig(NumScreen)?.TaskbarHeight > 0)
            {
                double newHeight = ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight;
                newHeight = newHeight / previousDpi * DpiScale;
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = newHeight;
            }
            if (ConfigManager.GetTaskbarConfig(NumScreen)?.TaskbarMinHeight > 0)
            {
                double newMinHeight = ConfigManager.GetTaskbarConfig(NumScreen).TaskbarMinHeight;
                newMinHeight = newMinHeight / previousDpi * DpiScale;
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarMinHeight = newMinHeight;
            }
            if (ConfigManager.GetTaskbarConfig(NumScreen)?.TaskbarWidth > 0)
            {
                double newWidth = ConfigManager.GetTaskbarConfig(NumScreen).TaskbarWidth;
                newWidth = newWidth / previousDpi * DpiScale;
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarWidth = newWidth;
            }
        }

        SetPosition();
        DelaySetPosition();

        Thread.Sleep(200);

        SetSize();
    }

    public void SetSize()
    {
        // Layout rounding causes incorrect sizing on non-integer scales
        if (DpiHelper.DpiScale % 1 != 0)
            UseLayoutRounding = false;

        MinHeight = Math.Max(ConfigManager.GetTaskbarConfig(NumScreen)?.TaskbarMinHeight ?? 0, DesiredHeight);

        if (ConfigManager.GetTaskbarConfig(NumScreen)?.TaskbarHeight > 0)
            DesiredHeight = ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight;
        if (ConfigManager.GetTaskbarConfig(NumScreen)?.TaskbarWidth > 0)
            DesiredWidth = ConfigManager.GetTaskbarConfig(NumScreen).TaskbarWidth;
        FloatingButton.Height = DesiredHeight;
        FloatingButton.FloatingButtonImage.Height = DesiredHeight;
    }

    public override void SetPosition()
    {
        if (!MyDataContext.TaskbarVisible)
            return;

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
            },
        });
    }

    private void Taskbar_OnLocationChanged(object sender, EventArgs e)
    {
        if (!IsRegistered)
            return;

        // primarily for win7/8, they will set up the appbar correctly but then put it in the wrong place
        if (Orientation == Orientation.Vertical)
        {
            double desiredLeft = 0;

            if (AppBarEdge == AppBarEdge.Right)
                desiredLeft = Screen.Bounds.Right / DpiScale - Width;

            if (Left != desiredLeft)
                Left = desiredLeft;
        }
        else
        {
            double desiredTop = 0;

            if (AppBarEdge == AppBarEdge.Bottom)
                desiredTop = Screen.Bounds.Bottom / DpiScale - Height;

            if (Top != desiredTop)
                Top = desiredTop;
        }
    }

    #endregion

    private void SetFontSmoothing()
    {
        VisualTextRenderingMode = ConfigManager.GetTaskbarConfig(NumScreen).AllowFontSmoothing ? TextRenderingMode.Auto : TextRenderingMode.Aliased;
    }

    protected override void CustomClosing()
    {
        if (AllowClose && !IsReopening)
        {
            PreviewKeyUp -= Taskbar_PreviewKeyUp;
            MyTaskbarApp.MyShellManager.TasksService.WindowActivated -= ClosePopup;
            if (_mainScreen)
            {
                _explorerHelper.HideExplorerTaskbar = false;
                MySystray.MyDataContext.Unload();
            }
        }
    }

    private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        ShellLogger.Debug("OnLoaded on Taskbar " + NumScreen);
        MyDataContext.ChangeEdge(AppBarEdge);
        if (_mainScreen)
        {
            MyTaskbarApp.MyShellManager.Tasks.Initialize(new TaskCategoryProvider());
            MyTaskbarApp.MyShellManager.TasksService.FullScreenChanged += TasksService_FullScreenChanged;
        }
        AddToolbars();

        FloatingButton.MyDataContext.SetParentTaskbar(this);
        // TODO : Start in floatinng mode, not working yet
        /*if (ConfigManager.GetTaskbarConfig(_numScreen).StartFloating)
            MyDataContext.ExpandCollapseTaskbar(false.ToString());*/
    }

    private void TasksService_FullScreenChanged(object sender, FullScreenEventArgs e)
    {
        MyTaskbarApp.MyShellManager.NotificationArea.Disable = e.IsEntering;
    }

    public void SetBackground(SolidColorBrush newBackground)
    {
        Background = newBackground;
        UpdatePlugins();
    }

    #region Move/stretch

    public void ChangeDesiredSize(double height, double width)
    {
        Height = height;
        Width = width;
        DesiredHeight = height;
        DesiredWidth = width;
        _appBarManager.SetWorkArea(Screen);
        ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = DesiredHeight;
        ConfigManager.GetTaskbarConfig(NumScreen).TaskbarWidth = DesiredWidth;
        UpdatePlugins();
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
            UpdatePlugins();
            foreach (ToolbarViewModel tb in ToolsBars.Children.OfType<Toolbar>().Select(tb => tb.MyDataContext))
            {
                tb.RefreshMyCollectionView();
                tb.UpdateInvisibleIcons();
            }
        }
    }

    private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _lastMousePosition = e.GetPosition(ToolsBars);
    }

    #endregion

    private void MenuToolbars_MouseEnter(object sender, MouseEventArgs e)
    {
        MyDataContext.BuildToolbarsMenu();
    }

    private void AppBarWindow_Unloaded(object sender, RoutedEventArgs e)
    {
        ToolsBars.Children.Clear();
        ShellLogger.Debug("OnUnloaded Taskbar " + NumScreen);
        if (_mainScreen)
            MyTaskbarApp.MyShellManager.TasksService.FullScreenChanged -= TasksService_FullScreenChanged;
    }

    public void FloatingTaskbar()
    {
        if (MyDataContext.TaskbarVisible)
            RegisterAppBar();
        else
            UnregisterAppBar();
        _appBarManager.SetWorkArea(Screen);
    }
}
