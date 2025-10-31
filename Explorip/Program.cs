using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using Explorip.Helpers;

using ExploripApi;

using ExploripConfig.Configuration;

using Microsoft.Win32;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace Explorip;

public static class Program
{
    private static Application _WpfHost;
    internal static bool ModeShell { get; private set; }
    private const string AppDomainName = "ExploripAppDomain";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread()]
    public static void Main(string[] args)
    {
        if (AppDomain.CurrentDomain.FriendlyName != AppDomainName)
        {
            AppDomainSetup appDomainSetup = AppDomain.CurrentDomain.SetupInformation;
            appDomainSetup.ShadowCopyFiles = "true";
            appDomainSetup.ShadowCopyDirectories = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            appDomainSetup.ApplicationBase = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            AppDomain myAppDomain = AppDomain.CreateDomain(AppDomainName, null, appDomainSetup);
#if !DEBUG
            myAppDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            try
            {
                myAppDomain.ExecuteAssembly(Environment.GetCommandLineArgs()[0], args);
            }
            catch (Exception ex)
            {
                CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(ex, false));
            }
            try
            {
                (_WpfHost as TaskBar.MyTaskbarApp)?.ExitGracefully();
            }
            catch (Exception) { /* Ignore errors when trying to gracefully closed taskbar */ }
            return;
        }

        ModeShell = true;
        Mutex mutexProcess;

        Process[] process = Process.GetProcessesByName("explorer");
        if (process != null && process.Length > 0)
            ModeShell = !process.AsEnumerable().Any(proc => StringComparer.OrdinalIgnoreCase.Equals(proc.MainModule?.FileName ?? "", Path.Combine(Environment.SpecialFolder.Windows.FullPath(), "explorer.exe")));

        Constants.Localization.LoadTranslation();

#if !DEBUG
        if (Directory.Exists(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), Updater.AutoUpdater.UpdateFolder)))
        {
            Process.Start("autoupdate.cmd", Updater.AutoUpdater.UpdateFolder);
            return;
        }
        Updater.AutoUpdater.SearchNewVersion(false);
#endif

        ExploripSharedCopy.Constants.Colors.LoadTheme();
        ExploripSharedCopy.Constants.Localization.LoadTranslation();
        Constants.Icons.Init();
        ConfigManager.Init();

        if (ArgumentExists("StartMenu"))
        {
            _WpfHost = new StartMenu.MyStartMenuApp();
            _WpfHost.Run();
            return;
        }

        if (ArgumentExists("desktop") || ArgumentExists("desktops"))
        {
            mutexProcess = new Mutex(true, "ExploripDesktop", out bool processNotLaunched);
            if (processNotLaunched)
            {
                RegisterSystemEvents();
                _WpfHost = new Desktop.MyDesktopApp();
                _WpfHost.Run();
            }
        }
        else if (ArgumentExists("taskbar") || ArgumentExists("taskbars"))
        {
            mutexProcess = new Mutex(true, "ExploripTaskbar", out bool processNotLaunched);
            if (processNotLaunched)
            {
                RegisterSystemEvents();
                _WpfHost = new TaskBar.MyTaskbarApp();
                _WpfHost.Run();
            }
        }
        else
        {
            mutexProcess = new Mutex(true, "ExploripFileExplorer", out bool processNotLaunched);
            if (processNotLaunched || args.Contains("newinstance"))
            {
                if (processNotLaunched)
                {
                    IpcServerManager.InitChannel(new IpcServer());
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                }
                if (ConfigManager.HookCopy)
                    HookCopyOperationsHelper.InstallHook();
                _WpfHost = new Explorer.MyExplorerApp();
                _WpfHost.Run();
            }
            else
            {
                if (args?.Length > 0 && Directory.Exists(args[0]))
                    IpcServerManager.SendNewWindow(args);
                else
                    IpcServerManager.SendNewWindow([Environment.SpecialFolder.System.FullPath().Substring(0, 3)]);
            }
        }
        mutexProcess.Dispose();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        if (MessageBox.Show($"Unhandled error happends : {Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}At : {ex.StackTrace}{Environment.NewLine}{Environment.NewLine}Please report it at filrip@gmail.com or in 'issue' on official website. Thank you.{Environment.NewLine}{Environment.NewLine}The application can be unstable. Do you want to continue ?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
            if (_WpfHost is TaskBar.MyTaskbarApp app)
            {
                try
                {
                    app.ExitGracefully();
                }
                catch (Exception) { /* Ignore errors when trying to gracefully closed taskbar */ }
            }
            Environment.Exit(1);
        }
    }

    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        IpcServerManager.Shutdown();
    }

    public static Application MyCurrentApp
    {
        get { return _WpfHost; }
    }

    private static void RegisterSystemEvents()
    {
        SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
        // TODO : Warn taskbar and/or dekstop
        WpfScreenHelper.Screen.ResetMultiMonitorSupport();
        Monitorian.MonitorsManager.Clean();
    }
}
