using Explorip.Forms;
using System;
using System.Windows.Forms;

namespace Explorip
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Helpers.ExtensionsCommandLineArguments.ArgumentPresent("taskbar"))
                Application.Run(new FormTaskBar());
            else
                Application.Run(new FormExplorer());
        }
    }
}
