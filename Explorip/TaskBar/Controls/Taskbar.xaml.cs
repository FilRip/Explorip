using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;
using ManagedShell.WindowsTray;

using Microsoft.WindowsAPICodePack.Shell.CommonFileDialogs;

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

    #region Manage toolbar

    public Grid ListToolbars
    {
        get { return ToolsBars; }
    }

    public void RefreshAllInvisibleIcons()
    {
        System.Threading.Tasks.Task.Run(async () =>
        {
            await System.Threading.Tasks.Task.Delay(1000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ToolsBars.Children.OfType<Toolbar>().ToList().ForEach(tb => tb.MyDataContext.UpdateInvisibleIcons());
            });
        });
    }

    /// <summary>
    /// Add toolbar (like QuickLaunch)
    /// </summary>
    /// <param name="path">Path/Directory where shortcuts to display are stored</param>
    /// <param name="resize">The Taskbar must be resized ?</param>
    public Toolbar AddToolbar(string path, bool resize = true)
    {
        if (!Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
            return null;
        AddBaseGrid(0, 0);
        int maxWidth = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarMaxWidth(path);
        (int, int) gridPos = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarGrid(path);
        if (gridPos.Item1 >= 0 || gridPos.Item2 >= 0)
            AddMissingGrid(gridPos.Item1 + 1, gridPos.Item2 + 1, 0, 22);
        else if (ToolsBars.Children.Count > 0)
            AddMissingGrid(ToolsBars.ColumnDefinitions.Count, ToolsBars.RowDefinitions.Count + 1, 0, 0);
        Toolbar newToolbar = new();
        newToolbar.MyDataContext.Path = path;
        if (maxWidth > 0)
            newToolbar.MaxWidth = maxWidth;
        Grid.SetRow(newToolbar, (gridPos.Item2 < 0 ? ToolsBars.RowDefinitions.Count - 1 : gridPos.Item2));
        Grid.SetColumn(newToolbar, (gridPos.Item1 < 0 ? 0 : gridPos.Item1));
        ToolsBars.Children.Add(newToolbar);
        if (resize)
        {
            if (AppBarEdge == AppBarEdge.Top || AppBarEdge == AppBarEdge.Bottom)
            {
                Height += 22;
                DesiredHeight = Height;
            }
            else
            {
                Width += 22;
                DesiredWidth = Width;
            }
            ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(path, true);
        }
        _appBarManager.SetWorkArea(Screen);
        Panel.SetZIndex(newToolbar, ConfigManager.GetTaskbarConfig(NumScreen).ToolbarZIndex(path));
        return newToolbar;
    }

    /// <summary>
    /// Add a plugin Toolbar
    /// </summary>
    /// <param name="plugin">Plugin to add</param>
    /// <param name="resize">The Taskbar must be resized ?</param>
    public void AddToolbar(IExploripToolbar plugin, bool resize = true)
    {
        double height = plugin.MinHeight;
        double width = plugin.MinWidth;
        ToolbarPlugin tp = new();
        Grid.SetColumn(plugin.ExploripToolbar, 1);
        tp.MainGrid.Children.Add(plugin.ExploripToolbar);
        tp.PluginLinked = plugin;
        AddBaseGrid(width, height);
        (int, int) gridPos = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarGrid(plugin.GuidKey.ToString());
        if (gridPos.Item1 >= 0 || gridPos.Item2 >= 0)
            AddMissingGrid(gridPos.Item1 + 1, gridPos.Item2 + 1, 0, height);
        else if (ToolsBars.Children.Count > 0)
            AddMissingGrid(ToolsBars.ColumnDefinitions.Count, ToolsBars.RowDefinitions.Count + 1, plugin.MinWidth, plugin.MinHeight);
        int numRow = (gridPos.Item2 < 0 ? ToolsBars.RowDefinitions.Count - 1 : gridPos.Item2);
        int numColumn = (gridPos.Item1 < 0 ? 0 : gridPos.Item1);
        int maxWidth = ConfigManager.GetTaskbarConfig(NumScreen).ToolbarMaxWidth(plugin.GuidKey.ToString());
        if (gridPos.Item1 >= 0 || gridPos.Item2 >= 0)
        {
            ToolsBars.RowDefinitions[numRow].Height = new GridLength(height, (height == 0 ? GridUnitType.Auto : GridUnitType.Pixel));
            ToolsBars.ColumnDefinitions[numColumn].Width = new GridLength(width, (width == 0 ? GridUnitType.Auto : GridUnitType.Pixel));
            if (maxWidth > 0)
                ToolsBars.ColumnDefinitions[numColumn].MaxWidth = maxWidth;
        }
        Grid.SetRow(tp, numRow);
        Grid.SetColumn(tp, numColumn);
        ToolsBars.Children.Add(tp);
        if (resize)
        {
            if (AppBarEdge == AppBarEdge.Top || AppBarEdge == AppBarEdge.Bottom)
            {
                Height += height > 0 ? height : plugin.ExploripToolbar.ActualHeight;
                DesiredHeight = Height;
            }
            else
            {
                Width += width > 0 ? width : plugin.ExploripToolbar.ActualWidth;
                DesiredWidth = width;
            }
            ConfigManager.GetTaskbarConfig(NumScreen).ToolbarVisible(tp.MyDataContext.Id, true);
        }
        _appBarManager.SetWorkArea(Screen);
        plugin.SetGlobalColors(ExploripSharedCopy.Constants.Colors.BackgroundColorBrush, ExploripSharedCopy.Constants.Colors.ForegroundColorBrush, ExploripSharedCopy.Constants.Colors.AccentColorBrush);
        plugin.UpdateTaskbar(_numScreen, ActualWidth, ActualHeight, Background, AppBarEdge);
        Panel.SetZIndex(tp, ConfigManager.GetTaskbarConfig(NumScreen).ToolbarZIndex(plugin.GuidKey.ToString()));
    }

    private void AddBaseGrid(double width, double height)
    {
        if (ToolsBars.ColumnDefinitions.Count == 0)
            ToolsBars.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, (width == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
        if (ToolsBars.RowDefinitions.Count == 0)
            ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height, (height == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
    }

    private void AddMissingGrid(int numColumn, int numRow, double width, double height)
    {
        while (numColumn > 0 && ToolsBars.ColumnDefinitions.Count < numColumn)
            ToolsBars.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, (width == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
        while (numRow > 0 && ToolsBars.RowDefinitions.Count < numRow)
            ToolsBars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height, (height == 0 ? GridUnitType.Auto : GridUnitType.Pixel)) });
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
            if (pluginName == Constants.Localization.PLUGINS || pluginName == Constants.Localization.NO_PLUGINS)
                return;
            if (pluginName == Constants.Localization.RELOAD_PLUGINS)
            {
                ReloadPlugins();
            }
            else if (mi.Tag is Guid guidKey)
            {
                IExploripToolbar plugin = PluginsManager.GetPlugin(guidKey);
                AddToolbar(plugin);
                if (MainScreen)
                    ConfigManager.ToolbarsPath = [.. ToolsBars.Children.OfType<BaseToolbar>().Select(tb => tb.BaseDataContext.Id)];
                ConfigManager.GetTaskbarConfig(NumScreen).TaskbarHeight = DesiredHeight;
            }
        }
    }

    private void ReloadPlugins()
    {
        PluginsManager.Reload();
        List<ToolbarPlugin> listToRemove = [];
        List<(ToolbarPlugin, IExploripToolbar)> listToReplace = [];
        foreach (ToolbarPlugin tp in ToolsBars.Children.OfType<ToolbarPlugin>())
        {
            IExploripToolbar toolbar = PluginsManager.ListPlugins().FirstOrDefault(i => i.GuidKey == tp.PluginLinked.GuidKey);
            if (toolbar == null)
                listToRemove.Add(tp);
            else if (toolbar.Version.CompareTo(tp.PluginLinked.Version) > 0)
                listToReplace.Add((tp, toolbar));
        }
        foreach (ToolbarPlugin tp in listToRemove)
            ToolsBars.Children.Remove(tp);
        foreach ((ToolbarPlugin, IExploripToolbar) tp in listToReplace)
        {
            tp.Item1.MainGrid.Children.Clear();
            tp.Item1.MainGrid.Children.Add(tp.Item2.ExploripToolbar);
            tp.Item1.PluginLinked = tp.Item2;
        }
    }

    private void UpdatePlugins()
    {
        foreach (ToolbarPlugin plugin in ToolsBars.Children.OfType<ToolbarPlugin>())
            plugin.PluginLinked.UpdateTaskbar(_numScreen, ActualWidth, ActualHeight, Background, AppBarEdge);
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
