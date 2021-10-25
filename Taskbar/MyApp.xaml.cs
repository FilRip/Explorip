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
        private readonly ShellManager _shellManager;

        public MyApp()
        {
            _shellManager = SetupManagedShell();

            _startMenuMonitor = new StartMenuMonitor(new AppVisibilityHelper(false));
            DictionaryManager = new DictionaryManager();

            // Startup
            DictionaryManager.SetLanguageFromSettings();
            DictionaryManager.SetThemeFromSettings();
            OpenTaskbar();
        }

        public void ExitGracefully()
        {
            _shellManager.ExplorerHelper.HideExplorerTaskbar = false;
            _shellManager.AppBarManager.SignalGracefulShutdown();
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
            _taskbar = new Taskbar(_shellManager, _startMenuMonitor, AppBarScreen.FromPrimaryScreen(), (AppBarEdge)Settings.Instance.Edge);
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

            return new ShellManager(ShellManager.DefaultShellConfig);
        }

        private void ExitApp()
        {
            _shellManager.ExplorerHelper.HideExplorerTaskbar = false;
            DictionaryManager.Dispose();
            _shellManager.Dispose();
            _startMenuMonitor.Dispose();
            _logger.Dispose();
        }
    }
}
