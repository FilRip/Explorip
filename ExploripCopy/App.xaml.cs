using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

namespace ExploripCopy
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }
    }
}
