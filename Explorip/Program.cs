using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Explorip.Forms;
using Explorip.Helpers;

namespace Explorip
{
    public static class Program
    {
        private static System.Windows.Application _WpfHost;
        private static Mutex _mutexTaskbar;
        public static bool ModeShell;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        public static void Main(string[] args)
        {
            ModeShell = true;

            _mutexTaskbar = new Mutex(true, "ExploripTaskbar", out bool taskBarNotLaunched);
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;

            Themes.AutoTheme.InitButtons();

            if (ExtensionsCommandLineArguments.ArgumentPresent("taskbar") && taskBarNotLaunched)
            {
                _mutexTaskbar.Dispose();
                _WpfHost = new TaskBar.MyDesktopApp();
                _WpfHost.Run();
            }
            else
            {
                //Application.Run(new FormExplorerBrowser(args));
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
