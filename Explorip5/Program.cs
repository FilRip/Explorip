using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Explorip.Forms;
using Explorip.Helpers;

namespace Explorip
{
    static class Program
    {
        private static System.Windows.Application _WpfHost;
        private static Mutex _mutexTaskbar;
        public static bool ModeShell;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ModeShell = true;

            _mutexTaskbar = new Mutex(true, "ExploripTaskbar", out bool taskBarLaunched);
            Process[] process = Process.GetProcessesByName("explorer");
            if (process != null && process.Length > 0)
            {
                foreach (Process proc in process)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(proc.MainModule?.FileName ?? "", Environment.SpecialFolder.Windows.Repertoire() + "\\explorer.exe"))
                    {
                        ModeShell = false;
                        break;
                    }
                }
            }

            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;

            Themes.AutoTheme.InitButtons();

            if (ExtensionsCommandLineArguments.ArgumentPresent("taskbar") && !taskBarLaunched)
            {
                _mutexTaskbar.Dispose();
                _WpfHost = new TaskBar.MyApp();
                _WpfHost.Run();
            }
            else
            {
                Application.Run(new FormExplorerBrowser(args));
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
                ((TaskBar.MyApp)_WpfHost).ExitGracefully();
            }
        }
    }
}
