using System;
using System.Diagnostics;
#if !DEBUG
using System.IO;
#endif
using System.Linq;
using System.Threading;
using System.Windows;

using Explorip.Helpers;
#if !DEBUG
using Explorip.Updater;
#endif

using ExploripApi;

using ExploripConfig.Configuration;

using Microsoft.Win32;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace Explorip;

public static class Program
{
    private static Application _WpfHost;
    internal static bool ModeShell { get; private set; }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread()]
    public static void Main(string[] args)
    {
#if !DEBUG
        if (Directory.Exists(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), AutoUpdater.UpdateFolder)))
        {
            Process.Start("autoupdate.cmd", AutoUpdater.UpdateFolder);
            return;
        }
        AutoUpdater.SearchNewVersion(true);
#endif
        ModeShell = true;
        Mutex mutexProcess;

        Process[] process = Process.GetProcessesByName("explorer");
        if (process != null && process.Length > 0)
        {
            ModeShell = !process.AsEnumerable().Any(proc => StringComparer.OrdinalIgnoreCase.Equals(proc.MainModule?.FileName ?? "", System.IO.Path.Combine(Environment.SpecialFolder.Windows.FullPath(), "explorer.exe")));
        }

        Constants.Localization.LoadTranslation();
        ExploripSharedCopy.Constants.Colors.LoadTheme();
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
                    HookCopyOperations.InstallHook();
                _WpfHost = new Explorer.MyExplorerApp();
                _WpfHost.Run();
            }
            else
            {
                IpcServerManager.SendNewWindow(args);
            }
        }
        mutexProcess.Dispose();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        if (MessageBox.Show($"Unhandled error happends : {Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}At : {ex.StackTrace}{Environment.NewLine}{Environment.NewLine}Please report it at filrip@gmail.com or in 'issue' on official website. Thank you.{Environment.NewLine}{Environment.NewLine}The application can be unstable. Do you want to continue ?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.No)
            Environment.Exit(1);
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
    }
}
