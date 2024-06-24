using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

using Explorip.Helpers;

using ExploripApi;

using ExploripConfig.Configuration;
using ExploripConfig.Helpers;

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
        ModeShell = true;
        Mutex mutexProcess;

        Process[] process = Process.GetProcessesByName("explorer");
        if (process != null && process.Length > 0)
        {
            ModeShell = !process.AsEnumerable().Any(proc => StringComparer.OrdinalIgnoreCase.Equals(proc.MainModule?.FileName ?? "", Environment.SpecialFolder.Windows.FullPath() + "\\explorer.exe"));
        }

        Constants.Localization.LoadTranslation();
        Constants.Colors.LoadTheme();
        Constants.Icons.Init();
        ConfigManager.Init();

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
        else if (ArgumentExists("taskbar") ||
            ArgumentExists("taskbars"))
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
                    HookCopyOperations.GetInstance().InstallHook();
                _WpfHost = new Explorer.MyExplorerApp();
                _WpfHost.Run();
            }
            else
            {
                args ??= [];
                args = args.Remove("explorer");
                args = args.Remove("withoutHook");
                args = args.Remove("useOwnCopier");
                args = args.Remove("newinstance");
                args = args.Remove("disablewriteconfig");
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
