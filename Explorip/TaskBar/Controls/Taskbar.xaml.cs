using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
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
    private IntPtr _windowHandle = IntPtr.Zero;

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

        base.DataContext = new TaskbarViewModel(this)
        {
            TaskbarVisible = true,
        };

        StartButton.StartMenuMonitor = startMenuMonitor;

        AllowsTransparency = ConfigManager.GetTaskbarConfig(_numScreen).TaskbarAllowsTransparency;
        SetFontSmoothing();

        _explorerHelper.HideExplorerTaskbar = true;

        SetSize();

        DataContext.TabTipVisible = ConfigManager.GetTaskbarConfig(NumScreen).ShowTabTip;
        DataContext.KeyboardLayoutVisible = ConfigManager.GetTaskbarConfig(NumScreen).ShowKeyboardLayout;

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
    }

    private void ClosePopup(object sender, WindowEventArgs e)
    {
        MyPopup.IsOpen = false;
    }

    #region Properties

    public new TaskbarViewModel DataContext
    {
        get { return (TaskbarViewModel)base.DataContext; }
        set { base.DataContext = value; }
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
            else if (msg == (int)NativeMethods.WM.HOTKEY && (int)wParam == 1)
            {
                WpfScreenHelper.Screen currentScreen = WpfScreenHelper.MouseHelper.MouseScreen;
                foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
                {
                    if (tb.NumScreen == currentScreen.DisplayNumber)
                    {
                        tb.DataContext.ExpandCollapseTaskbar(!tb.DataContext.TaskbarVisible);
                        break;
                    }
                }
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
        if (!DataContext.TaskbarVisible)
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
                MySystray.DataContext.Unload();
                UnregisterHotKeyFloating();
            }
        }
    }

    private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        ShellLogger.Debug("OnLoaded on Taskbar " + NumScreen);
        DataContext.ChangeEdge(AppBarEdge);
        if (_mainScreen)
        {
            MyTaskbarApp.MyShellManager.Tasks.Initialize(new TaskCategoryProvider());
            MyTaskbarApp.MyShellManager.TasksService.FullScreenChanged += TasksService_FullScreenChanged;
            RegisterHotKeyFloating();
        }
        AddToolbars();

        FloatingButton.DataContext.SetParentTaskbar(this);
        // TODO : Start in floatinng mode, not working yet
        /*if (ConfigManager.GetTaskbarConfig(_numScreen).StartFloating)
            DataContext.ExpandCollapseTaskbar(false.ToString());*/
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
            DataContext.ResizeOn = true;
        }
        else
        {
            ResizeMode = ResizeMode.NoResize;
            DataContext.ResizeOn = false;
            DesiredHeight = Height;
            _appBarManager.SetWorkArea(Screen);
            UpdatePlugins();
            foreach (ToolbarViewModel tb in ToolsBars.Children.OfType<Toolbar>().Select(tb => tb.DataContext))
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
        DataContext.BuildToolbarsMenu();
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
        if (DataContext.TaskbarVisible)
            RegisterAppBar();
        else
            UnregisterAppBar();
        _appBarManager.SetWorkArea(Screen);
    }

    public void RegisterHotKeyFloating()
    {
        string hotkey = ConfigManager.FloatingShortcut;
        if (_windowHandle == IntPtr.Zero && !string.IsNullOrWhiteSpace(hotkey))
        {
            hotkey = hotkey.Trim().Replace(" ", "").ToLower();
            _windowHandle = new WindowInteropHelper(this).EnsureHandle();
            NativeMethods.HotKeyMods mods = NativeMethods.HotKeyMods.NOREPEAT;
            if (hotkey.Contains("{ctrl}"))
                mods |= NativeMethods.HotKeyMods.CONTROL;
            if (hotkey.Contains("{shift}"))
                mods |= NativeMethods.HotKeyMods.SHIFT;
            if (hotkey.Contains("{alt}"))
                mods |= NativeMethods.HotKeyMods.ALT;
            if (hotkey.Contains("{win}"))
                mods |= NativeMethods.HotKeyMods.WIN;
            hotkey = hotkey.Replace("{ctrl}", "").Replace("{shift}", "").Replace("{alt}", "").Replace("{win}", "").Replace("{", "").Replace("}", "");
            if (Enum.TryParse(hotkey, true, out NativeMethods.VK vk))
            {
                NativeMethods.RegisterHotKey(_windowHandle, 1, mods, (uint)vk);
            }
        }
    }

    public void UnregisterHotKeyFloating()
    {
        if (_windowHandle != IntPtr.Zero && !string.IsNullOrWhiteSpace(ConfigManager.FloatingShortcut))
            NativeMethods.UnregisterHotKey(_windowHandle, 1);
    }
}
