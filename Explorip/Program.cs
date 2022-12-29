using System;
using System.Windows.Forms;

using Explorip.Forms;

namespace Explorip
{
    public static class Program
    {
        private static System.Windows.Application _WpfHost;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += Application_ApplicationExit;

            if (Helpers.ExtensionsCommandLineArguments.ArgumentPresent("taskbar"))
            {
                _WpfHost = new TaskBar.MyApp();
                _WpfHost.Run();
            }
            else
            {
                //Application.Run(new FormFilRipExplorer(args));
                Application.Run(new FormExplorerBrowser(args));
                /*_WpfHost = new WPF.MyApp();
                _WpfHost.Run();*/
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
