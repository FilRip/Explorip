using System.Windows;

using ExploripCopy.ViewModels;

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
            Constants.Colors.LoadTheme();
            Constants.Localization.LoadTranslation();
            Constants.Icons.LoadIcons();

            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            NotifyIconViewModel.Instance.SetControl(notifyIcon);
        }
    }
}
