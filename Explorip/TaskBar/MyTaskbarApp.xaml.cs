﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

using Explorip.TaskBar.Controls;
using Explorip.TaskBar.Utilities;

using ExploripConfig.Configuration;
using ExploripConfig.Helpers;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

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
        Current?.Shutdown();
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
        taskBar = new Taskbar(_startMenuMonitor, AppBarScreen.FromPrimaryScreen(), ConfigManager.GetTaskbarConfig(Screen.PrimaryScreen.DeviceName.TrimStart('.', '\\')).Edge);
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
            taskBar = new Taskbar(_startMenuMonitor, appBarScreen, ConfigManager.GetTaskbarConfig(appBarScreen.DeviceName.TrimStart('.', '\\')).Edge);
            taskBar.Show();
            _taskbarList.Add(taskBar);
        }
    }

    public void ShowTaskbarOnAllOthersScreen()
    {
        if (_taskbarList.Count > 1)
        {
            foreach (Taskbar taskbar in _taskbarList)
                if (!taskbar.MainScreen)
                {
                    ConfigManager.GetTaskbarConfig(taskbar.ScreenName).ShowTaskbar = false;
                    taskbar.Close();
                }
        }
        else
        {
            if (_taskbarList.Count == 1 && Screen.AllScreens.Count() > 1)
            {
                foreach (Screen screen in Screen.AllScreens)
                    if (!screen.Primary)
                        ConfigManager.GetTaskbarConfig(screen.DeviceName.TrimStart('.', '\\')).ShowTaskbar = true;
                MakeTaskbarOnAllScreen();
            }
        }
    }

    private void App_OnExit(object sender, ExitEventArgs e)
    {
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
#if DEBUG
        _logger?.Dispose();
#endif
    }

    public Taskbar MainTaskbar
    {
        get
        {
            if (_taskbarList != null)
                return _taskbarList.First(item => item.MainScreen);
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

        _startMenuMonitor = new StartMenuMonitor(new AppVisibilityHelper(true));
        DictionaryManager = new DictionaryManager();

        // Startup
        DictionaryManager.SetThemeFromSettings();
        OpenTaskbar();
    }
}
