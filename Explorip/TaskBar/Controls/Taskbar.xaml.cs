using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Explorip.Helpers;
using Explorip.Plugins;
using Explorip.StartMenu.Window;
using Explorip.TaskBar.Utilities;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ExploripPlugins;

using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;
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
    private readonly int _numScreen;

    public Popup MyPopup { get; private set; }
    public int TimeBeforeAutoCloseThumb { get; private set; }

    public Taskbar(StartMenuMonitor startMenuMonitor, AppBarScreen screen)
        : base(MyTaskbarApp.MyShellManager.AppBarManager, MyTaskbarApp.MyShellManager.ExplorerHelper, MyTaskbarApp.MyShellManager.FullScreenHelper, screen, ConfigManager.GetTaskbarConfig(screen.NumScreen).Edge, 0)
    {
        InitializeComponent();

        _mainScreen = screen.Primary;
        _numScreen = screen.NumScreen;

        DataContext = new TaskbarViewModel(this);

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
    }

    private void ClosePopup(object sender, WindowEventArgs e)
    {
        MyPopup.IsOpen = false;
    }

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
            },
        });
    }

    private void SetFontSmoothing()
    {
        VisualTextRenderingMode = ConfigManager.GetTaskbarConfig(NumScreen).AllowFontSmoothing ? TextRenderingMode.Auto : TextRenderingMode.Aliased;
    }

    private void Taskbar_OnLocationChanged(object sender, EventArgs e)
    {
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

    protected override void CustomClosing()
    {
        if (AllowClose && !IsReopening)
        {
            MyTaskbarApp.MyShellManager.TasksService.WindowActivated -= ClosePopup;
            if (_mainScreen)
                _explorerHelper.HideExplorerTaskbar = false;
        }
    }

    private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
    {
        MyDataContext.ChangeEdge(AppBarEdge);
        if (_mainScreen)
        {
            ExitFullScreen += Taskbar_ExitFullScreen;
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
            foreach (string path in listToolbars.Where(p => ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(p)))
            {
                if (Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
                    AddToolbar(path, false);
                else if (Guid.TryParse(path, out Guid guidPlugin))
                {
                    IExploripToolbar plugin = PluginsManager.GetPlugin(guidPlugin);
                    if (plugin != null)
                        AddToolbar(plugin, false);
                }
            }
        }
    }

    private void Taskbar_ExitFullScreen(object sender, EventArgs e)
    {
        foreach (Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
            tb.MyTaskList.MyDataContext.FirstRefresh();
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

    private void Taskbar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _lastMousePosition = e.GetPosition(ToolsBars);
    }

    #endregion

    #region Manage toolbar

    public Grid ListToolbars
    {
        get { return ToolsBars; }
    }

    public Toolbar AddToolbar(string path, bool resize = true)
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
            ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(path, true);
        }
        _appBarManager.SetWorkArea(Screen);
        return newToolbar;
    }

    public void AddToolbar(IExploripToolbar plugin, bool resize = true)
    {
        double height = Math.Max(24, plugin.MinHeight);
        ToolbarPlugin tp = new();
        Grid.SetColumn(plugin.ExploripToolbar, 1);
        tp.MainGrid.Children.Add(plugin.ExploripToolbar);
        tp.PluginLinked = plugin;
        ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height, GridUnitType.Pixel) });
        Grid.SetRow(tp, ToolsBars.RowDefinitions.Count - 1);
        Grid.SetColumn(tp, 0);
        ToolsBars.Children.Add(tp);
        if (resize)
        {
            Height += height;
            DesiredHeight = Height;
            ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(tp.MyDataContext.Id, true);
        }
        _appBarManager.SetWorkArea(Screen);
        plugin.SetGlobalColors(ExploripSharedCopy.Constants.Colors.BackgroundColorBrush, ExploripSharedCopy.Constants.Colors.ForegroundColorBrush, ExploripSharedCopy.Constants.Colors.AccentColorBrush);
        plugin.UpdateTaskbar(_numScreen, ActualWidth, ActualHeight, Background, AppBarEdge);
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
                    ConfigManager.ToolbarsPath = [.. ToolsBars.Children.OfType<BaseToolbar>().Select(tb => tb.BaseDataContext.Id)];
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = DesiredHeight;
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

    private void ToolbarListPlugins_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is MenuItem mi)
        {
            string pluginName = mi.Header.ToString();
            if (pluginName == "Plugins" || pluginName == "No plugins loaded")
                return;
            IExploripToolbar plugin = PluginsManager.GetPlugin(pluginName);
            AddToolbar(plugin);
            if (MainScreen)
                ConfigManager.ToolbarsPath = [.. ToolsBars.Children.OfType<BaseToolbar>().Select(tb => tb.BaseDataContext.Id)];
            ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = DesiredHeight;
        }
    }

    private void UpdatePlugins()
    {
        foreach (ToolbarPlugin plugin in ToolsBars.Children.OfType<ToolbarPlugin>())
            plugin.PluginLinked.UpdateTaskbar(_numScreen, ActualWidth, ActualHeight, Background, AppBarEdge);
    }

    #endregion

    private void MenuToolbars_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        MyDataContext.BuildToolbarsMenu();
    }
}
