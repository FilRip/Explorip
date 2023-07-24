using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

using Explorip.Helpers;

namespace Explorip
{
    public static class Program
    {
        private static Application _WpfHost;
#pragma warning disable S2223 // Non-constant static fields should not be visible
        internal static bool ModeShell;
#pragma warning restore S2223 // Non-constant static fields should not be visible

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

            mutexProcess = new Mutex(true, "ExploripTaskbar", out bool processNotLaunched);
            if (ExtensionsCommandLineArguments.ArgumentExists("taskbar") ||
                ExtensionsCommandLineArguments.ArgumentExists("taskbars"))
            {
                if (processNotLaunched)
                {
                    _WpfHost = new TaskBar.MyDesktopApp();
                    _WpfHost.Run();
                }
            }
            else
            {
                if (processNotLaunched || args.Contains("newinstance"))
                {
                    IpcServerManager.InitChannel();
                    if (!ExtensionsCommandLineArguments.ArgumentExists("withoutHook"))
                        HookCopyOperations.GetInstance().InstallHook();
                    _WpfHost = new Explorer.MyExplorerApp();
                    _WpfHost.Run();
                }
                else
                {
                    IpcServerManager.SendMessage(args);
                }
            }
            mutexProcess?.Dispose();
        }

        public static Application MonApp
        {
            get { return _WpfHost; }
        }
    }
}
