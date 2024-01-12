using System;
using System.Windows;

using Explorip.Desktop.Windows;
using Explorip.WinAPI;

using WpfScreenHelper;

using static WpfScreenHelper.Screen;

namespace Explorip.Desktop
{
    /// <summary>
    /// Logique d'interaction pour MyApp.xaml
    /// </summary>
    public partial class MyDesktopApp : Application
    {
        public MyDesktopApp()
        {
            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ExploripDesktop desktop = new();
            desktop.Show();
            User32.SetParent(desktop.GetHandle(), ManagedShell.Common.Helpers.WindowHelper.GetLowestDesktopChildHwnd());
            desktop.SetWindowPosition((int)PrimaryScreen.WorkingArea.X, (int)PrimaryScreen.WorkingArea.Y, (int)PrimaryScreen.WorkingArea.Width, (int)PrimaryScreen.WorkingArea.Height);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // TODO : Close my window desktop(s)
        }
    }
}
