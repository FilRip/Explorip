using Explorip.Forms;
using System;
using System.Windows.Forms;

namespace Explorip
{
    public static class Program
    {
        private static TaskBar.MyApp _WpfHost;

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
                //Application.Run(new FormTaskBar());
                /*Form maForm = new FormTaskBar();
                maForm.Show();*/
                _WpfHost = new TaskBar.MyApp();
                //maForm.Close();
                _WpfHost.Run();
            }
            else
            {
                Application.Run(new FormExplorer(args));
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (_WpfHost != null)
            {
                _WpfHost.ExitGracefully();
            }
        }
    }
}
