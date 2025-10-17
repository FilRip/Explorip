﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

using Explorip.Plugins;
using Explorip.StartMenu.Window;
using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Utilities;

using ExploripConfig.Configuration;
using ExploripConfig.Helpers;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;

using Microsoft.Win32;

using WpfScreenHelper;

namespace Explorip.TaskBar;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class MyTaskbarApp : Application
{
    public DictionaryManager DictionaryManager { get; set; }

#if DEBUG
    private ManagedShellLogger _logger;
#endif
    private List<Taskbar> _taskbarList;
    private StartMenuMonitor _startMenuMonitor;
    private Thread _threadAutoLock;
    private static bool DisableAutoLock;

    public static ShellManager MyShellManager { get; private set; }

    public MyTaskbarApp()
    {
        InitializeComponent();
    }

    public void ExitGracefully()
    {
        Models.HookRefreshLanguageLayout.UnHook();
        foreach (Taskbar taskbar in _taskbarList)
        {
            taskbar.AllowClose = true;
            taskbar.Close();
        }
        _taskbarList.Clear();
        MyShellManager.AppBarManager.SignalGracefulShutdown();
        ExitApp();
        if (ConfigManager.TaskbarReplaceStartMenu)
            StartMenuWindow.MyStartMenu?.Close();
        Explorip.Helpers.HookTaskbarListHelper.UninstallHook();
        Current?.Shutdown();
    }

    private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
        Current.Dispatcher.Invoke(() =>
        {
            try
            {
                ShellLogger.Debug("Display settings changed, updating taskbar positions and sizes.");
                Screen.ForceRefreshListScreens();

                // Remove taskbar from screen that does not exists anymore
                for (int i = _taskbarList.Count - 1; i >= 0; i--)
                {
                    if (!Screen.AllScreens.Any(s => s.DisplayNumber == _taskbarList[i].NumScreen))
                    {
                        _taskbarList[i].AllowClose = true;
                        _taskbarList[i].Close();
                        _taskbarList.RemoveAt(i);
                    }
                }

                // Refresh taskbar still present
                foreach (Taskbar taskbar in _taskbarList)
                    taskbar.SetPositionAndSize();

                // Add taskbar on new monitor that is plugged
                foreach (Screen screen in Screen.AllScreens.Where(s => !_taskbarList.Any(tb => tb.NumScreen == s.DisplayNumber)))
                {
                    Taskbar taskBar = new(_startMenuMonitor, AppBarScreen.FromScreen(screen.DisplayNumber));
                    taskBar.Show();
                    _taskbarList.Add(taskBar);
                }
            }
            catch (Exception ex)
            {
                ShellLogger.Error(ex.Message, ex);
            }
        });
    }

    private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.ConsoleConnect ||
            e.Reason == SessionSwitchReason.RemoteConnect ||
            e.Reason == SessionSwitchReason.SessionLogon ||
            e.Reason == SessionSwitchReason.SessionUnlock)
        {
            ShellLogger.Debug("Enable background work");
            MyShellManager.ExplorerHelper.Disable = false;
            MyShellManager.NotificationArea.Disable = false;
            MyShellManager.FullScreenHelper.Disable = false;
            DisableAutoLock = false;
        }
        else
        {
            ShellLogger.Debug("Disable background work");
            MyShellManager.ExplorerHelper.Disable = true;
            MyShellManager.NotificationArea.Disable = true;
            MyShellManager.FullScreenHelper.Disable = true;
            DisableAutoLock = true;
        }
    }

    public void ReopenTaskbar()
    {
        foreach (Taskbar taskbar in _taskbarList)
        {
            taskbar.IsReopening = true;
            taskbar.AllowClose = true;
            taskbar.Close();
        }
        OpenTaskbar();
    }

    private void OpenTaskbar()
    {
        Taskbar taskBar;
        taskBar = new Taskbar(_startMenuMonitor, AppBarScreen.FromPrimaryScreen());
        taskBar.Show();
        _taskbarList.Add(taskBar);
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(taskBar).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
        if (ExtensionsCommandLineArguments.ArgumentExists("taskbars"))
            MakeTaskbarOnAllScreen();
    }

    private void MakeTaskbarOnAllScreen()
    {
        Taskbar taskBar;
        List<AppBarScreen> appBarScreens = AppBarScreen.FromAllOthersScreen();
        foreach (AppBarScreen appBarScreen in appBarScreens)
        {
            taskBar = new Taskbar(_startMenuMonitor, appBarScreen);
            taskBar.Show();
            _taskbarList.Add(taskBar);
        }
    }

    public void ShowTaskbarOnAllOthersScreen()
    {
        if (_taskbarList.Count > 1)
        {
            for (int i = _taskbarList.Count - 1; i >= 0; i--)
                if (!_taskbarList[i].MainScreen)
                {
                    ConfigManager.GetTaskbarConfig(_taskbarList[i].NumScreen).ShowTaskbar = false;
                    _taskbarList[i].AllowClose = true;
                    _taskbarList[i].Close();
                    _taskbarList.RemoveAt(i);
                }
        }
        else
        {
            if (_taskbarList.Count == 1 && Screen.AllScreens.Count() > 1)
            {
                foreach (Screen screen in Screen.AllScreens)
                    if (!screen.Primary)
                        ConfigManager.GetTaskbarConfig(screen.DisplayNumber).ShowTaskbar = true;
                MakeTaskbarOnAllScreen();
            }
        }
    }

    private void App_OnExit(object sender, ExitEventArgs e)
    {
        _threadAutoLock?.Abort();
        ExitGracefully();
    }

    private void App_OnSessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        ExitGracefully();
    }

    private ShellManager SetupManagedShell()
    {
        EnvironmentHelper.IsAppRunningAsShell = NativeMethods.GetShellWindow() == IntPtr.Zero;

#if DEBUG
        _logger = new ManagedShellLogger();
#endif

        ShellConfig config = new()
        {
            EnableTasksService = true,
            AutoStartTasksService = false,
            TaskIconSize = ManagedShell.Common.Enums.IconSize.ExtraLarge,

            EnableTrayService = true,
            AutoStartTrayService = true,
            PinnedNotifyIcons = ManagedShell.WindowsTray.NotificationArea.DEFAULT_PINNED,
        };

        return new ShellManager(config);
    }

    private void ExitApp()
    {
        MyShellManager.ExplorerHelper.HideExplorerTaskbar = false;
        DictionaryManager.Dispose();
        MyShellManager.Dispose();
        _startMenuMonitor.Dispose();
        Monitorian.MonitorsManager.Clean();
#if DEBUG
        _logger?.Dispose();
#endif
    }

    public Taskbar MainTaskbar
    {
        get
        {
            if (_taskbarList != null)
                return _taskbarList.Single(item => item.MainScreen);
            return null;
        }
    }

    public List<Taskbar> ListAllTaskbar()
    {
        return _taskbarList;
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        _taskbarList = [];
        MyShellManager = SetupManagedShell();
        MyShellManager.Tasks.SetInitialWindows = false;
        MyShellManager.TasksService.GroupApplicationsWindows = ConfigManager.GroupedApplicationWindow;

        _startMenuMonitor = new StartMenuMonitor(new AppVisibilityHelper(true));
        DictionaryManager = new DictionaryManager();

        PluginsManager.LoadPlugins();

        // Startup
        DictionaryManager.SetThemeFromSettings();
        OpenTaskbar();

        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

        if (ConfigManager.AutoLockOnMonitorPowerOff)
        {
            _threadAutoLock = new Thread(new ThreadStart(CheckMonitorPower));
            _threadAutoLock.Start();
        }

        if (ConfigManager.HookTaskbarList)
            Explorip.Helpers.HookTaskbarListHelper.InstallHook();
    }

    private static void CheckMonitorPower()
    {
        try
        {
            while (true)
            {
                if (!DisableAutoLock)
                {
                    bool lockSession = false;
                    Microsoft.Win32.SafeHandles.SafeFileHandle handle = NativeMethods.CreateFile("\\\\.\\LCD", 0, FileShare.None, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
                    if (!handle.IsInvalid)
                    {
#pragma warning disable S3869 // "SafeHandle.DangerousGetHandle" should not be called
                        NativeMethods.GetDevicePowerState(handle.DangerousGetHandle(), out bool on);
                        if (!on)
                        {
                            lockSession = true;
                        }
                        NativeMethods.CloseHandle(handle.DangerousGetHandle());
#pragma warning restore S3869 // "SafeHandle.DangerousGetHandle" should not be called
                    }
                    if ((lockSession || handle.IsInvalid) && Monitorian.MonitorsManager.AllMonitorsOff(true))
                    {
                        ShellLogger.Debug("Auto lock");
                        ShellHelper.Lock();
                    }
                    handle.Dispose();
                    Thread.Sleep(3000);
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }
}
