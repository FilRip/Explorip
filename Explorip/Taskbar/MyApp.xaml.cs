using System;
using ManagedShell;
using Explorip.TaskBar.Utilities;
using System.Windows;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using Application = System.Windows.Application;

namespace Explorip.TaskBar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class MyApp : Application
    {
        public DictionaryManager DictionaryManager { get; }

        private ManagedShellLogger _logger;
        private Taskbar _taskbar;
        private readonly StartMenuMonitor _startMenuMonitor;
        public static ShellManager MonShellManager;

        public MyApp()
        {
            MonShellManager = SetupManagedShell();

            _startMenuMonitor = new StartMenuMonitor(new AppVisibilityHelper(false));
            DictionaryManager = new DictionaryManager();

            // Startup
            DictionaryManager.SetLanguageFromSettings();
            DictionaryManager.SetThemeFromSettings();
            OpenTaskbar();
        }

        public void ExitGracefully()
        {
            MonShellManager.ExplorerHelper.HideExplorerTaskbar = false;
            MonShellManager.AppBarManager.SignalGracefulShutdown();
            Current.Shutdown();
        }

        public void ReopenTaskbar()
        {
            _taskbar.AllowClose = true;
            _taskbar?.Close();
            OpenTaskbar();
        }

        private void OpenTaskbar()
        {
            _taskbar = new Taskbar(_startMenuMonitor, AppBarScreen.FromPrimaryScreen(), (AppBarEdge)Settings.Instance.Edge);
            _taskbar.Show();
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
    }
}
