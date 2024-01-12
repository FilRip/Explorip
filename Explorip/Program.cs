using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

using Explorip.Helpers;

using ExploripApi;

using static Explorip.Helpers.ExtensionsCommandLineArguments;

namespace Explorip
{
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
                ModeShell = process.AsEnumerable().Any(proc => StringComparer.OrdinalIgnoreCase.Equals(proc.MainModule?.FileName ?? "", Environment.SpecialFolder.Windows.FullPath() + "\\explorer.exe"));
            }

            Constants.Localization.LoadTranslation();
            Constants.Colors.LoadTheme();
            Constants.Icons.Init();

            if (ArgumentExists("desktop"))
            {
                mutexProcess = new Mutex(true, "ExploripDesktop", out bool processNotLaunched);
                if (processNotLaunched)
                {
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
                    }
                    if (!ArgumentExists("withoutHook"))
                        HookCopyOperations.GetInstance().InstallHook();
                    _WpfHost = new Explorer.MyExplorerApp();
                    _WpfHost.Run();
                }
                else
                {
                    args ??= [];
                    args = args.Remove("explorer");
                    IpcServerManager.SendNewWindow(args);
                }
            }
            mutexProcess.Dispose();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            IpcServerManager.Shutdown();
        }

        public static Application MyCurrentApp
        {
            get { return _WpfHost; }
        }
    }
}
