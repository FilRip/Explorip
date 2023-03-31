using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Explorip.Helpers;

namespace Explorip
{
    public static class Program
    {
        private static System.Windows.Application _WpfHost;
#pragma warning disable S2223 // Non-constant static fields should not be visible
        internal static Mutex _mutexTaskbar;
        internal static bool ModeShell;
#pragma warning restore S2223 // Non-constant static fields should not be visible

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        public static void Main()
        {
            ModeShell = true;

            Process[] process = Process.GetProcessesByName("explorer");
            if (process != null && process.Length > 0)
            {
                ModeShell = process.AsEnumerable().Any(proc => StringComparer.OrdinalIgnoreCase.Equals(proc.MainModule?.FileName ?? "", Environment.SpecialFolder.Windows.Repertoire() + "\\explorer.exe"));
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;

            if (ExtensionsCommandLineArguments.ArgumentPresent("taskbar"))
            {
                _mutexTaskbar = new Mutex(true, "ExploripTaskbar", out bool taskBarNotLaunched);
                if (taskBarNotLaunched)
                {
                    _WpfHost = new TaskBar.MyDesktopApp();
                    _WpfHost.Run();
                }
                _mutexTaskbar.Dispose();
            }
            else
            {
                if (!ExtensionsCommandLineArguments.ArgumentPresent("withoutHook"))
                    HookCopyOperations.GetInstance().InstallHook();
                _WpfHost = new Explorer.WPF.MyExplorerApp();
                _WpfHost.Run();
            }
        }

        public static System.Windows.Application MonApp
        {
            get { return _WpfHost; }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (_WpfHost != null)
            {
                ((TaskBar.MyDesktopApp)_WpfHost).ExitGracefully();
            }
        }
    }
}
