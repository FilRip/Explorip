﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

using Explorip.TaskBar.Utilities;

using ExploripConfig.Configuration;
using ExploripConfig.Helpers;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

namespace Explorip.TaskBar;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class MyTaskbarApp : Application
{
    public DictionaryManager DictionaryManager { get; }

    private ManagedShellLogger _logger;
    private readonly List<Taskbar> _taskbarList;
    private readonly StartMenuMonitor _startMenuMonitor;
    public static ShellManager MyShellManager { get; private set; }

    public MyTaskbarApp()
    {
        _taskbarList = [];
        MyShellManager = SetupManagedShell();

        _startMenuMonitor = new StartMenuMonitor(new AppVisibilityHelper(true));
        DictionaryManager = new DictionaryManager();

        // Startup
        DictionaryManager.SetLanguageFromSettings();
        DictionaryManager.SetThemeFromSettings();
        OpenTaskbar();
    }

    public void ExitGracefully()
    {
        foreach (Taskbar taskbar in _taskbarList)
        {
            taskbar.AllowClose = true;
            taskbar.Close();
        }
        MyShellManager.AppBarManager.SignalGracefulShutdown();
        ExitApp();
        Current.Shutdown();
    }

    public void ReopenTaskbar()
    {
        foreach (Taskbar taskbar in _taskbarList)
        {
            taskbar.AllowClose = true;
            taskbar.Close();
        }
        OpenTaskbar();
    }

    private void OpenTaskbar()
    {
        Taskbar taskBar;
        taskBar = new Taskbar(_startMenuMonitor, AppBarScreen.FromPrimaryScreen(), (AppBarEdge)ConfigManager.Edge);
        taskBar.Show();
        _taskbarList.Add(taskBar);
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(taskBar).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
        if (ExtensionsCommandLineArguments.ArgumentExists("taskbars"))
            ShowTaskbarOnAllOthersScreen();
    }

    public void ShowTaskbarOnAllOthersScreen()
    {
        if (_taskbarList.Count == 1 && WpfScreenHelper.Screen.AllScreens.Count() > 1)
        {
            Taskbar taskBar;
            List<AppBarScreen> appBarScreens = AppBarScreen.FromAllOthersScreen();
            foreach (AppBarScreen appBarScreen in appBarScreens)
            {
                taskBar = new Taskbar(_startMenuMonitor, appBarScreen, (AppBarEdge)ConfigManager.Edge);
                taskBar.Show();
                _taskbarList.Add(taskBar);
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

        _logger = new ManagedShellLogger();

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
        _logger.Dispose();
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
}
