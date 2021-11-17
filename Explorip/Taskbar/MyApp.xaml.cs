using System;
using ManagedShell;
using Explorip.TaskBar.Utilities;
using System.Windows;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using Application = System.Windows.Application;
using System.Collections.Generic;
using System.Linq;

namespace Explorip.TaskBar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class MyApp : Application
    {
        public DictionaryManager DictionaryManager { get; }

        private ManagedShellLogger _logger;
        private readonly List<Taskbar> _taskbarList;
        private readonly StartMenuMonitor _startMenuMonitor;
        public static ShellManager MonShellManager;

        public MyApp()
        {
            _taskbarList = new List<Taskbar>();
            MonShellManager = SetupManagedShell();

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
            MonShellManager.ExplorerHelper.HideExplorerTaskbar = false;
            MonShellManager.AppBarManager.SignalGracefulShutdown();
            Current.Shutdown();
        }

        public void ReopenTaskbar()
        {
            foreach (Taskbar taskbar in _taskbarList)
            {
                taskbar.AllowClose = true;
                taskbar?.Close();
            }
            OpenTaskbar();
        }

        private void OpenTaskbar()
        {
            Taskbar taskBar;
            taskBar = new Taskbar(_startMenuMonitor, AppBarScreen.FromPrimaryScreen(), (AppBarEdge)Settings.Instance.Edge);
            taskBar.Show();
            _taskbarList.Add(taskBar);
            if (Helpers.ExtensionsCommandLineArguments.ArgumentPresent("taskbars"))
            {
                List<AppBarScreen> appBarScreens = AppBarScreen.FromAllOthersScreen();
                foreach (AppBarScreen appBarScreen in appBarScreens)
                {
                    taskBar = new Taskbar(_startMenuMonitor, appBarScreen, (AppBarEdge)Settings.Instance.Edge);
                    taskBar.Show();
                    _taskbarList.Add(taskBar);
                }
            }
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            ExitApp();
        }

        private void App_OnSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            ExitApp();
        }

        private ShellManager SetupManagedShell()
        {
            EnvironmentHelper.IsAppRunningAsShell = NativeMethods.GetShellWindow() == IntPtr.Zero;

            _logger = new ManagedShellLogger();

            ShellConfig config = new ShellConfig()
            {
                EnableTasksService = true,
                AutoStartTasksService = false,
                TaskIconSize = ManagedShell.Common.Enums.IconSize.ExtraLarge,

                EnableTrayService = true,
                AutoStartTrayService = true,
                PinnedNotifyIcons = ManagedShell.WindowsTray.NotificationArea.DEFAULT_PINNED
            };

            return new ShellManager(config);
        }

        private void ExitApp()
        {
            MonShellManager.ExplorerHelper.HideExplorerTaskbar = false;
            DictionaryManager.Dispose();
            MonShellManager.Dispose();
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

        public List<Taskbar> ListeBarreDesTaches()
        {
            return _taskbarList;
        }
    }
}
