using System;
using System.Threading;
using System.Windows;

using ExploripCopy.ViewModels;

using Hardcodet.Wpf.TaskbarNotification;

namespace ExploripCopy;

/// <summary>
/// Logique d'interaction pour App.xaml
/// </summary>
public partial class ExploripCopyApp : Application
{
    private TaskbarIcon notifyIcon;
    private Mutex _mutexProcess;

    public ExploripCopyApp()
    {
        InitializeComponent();
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        notifyIcon.Dispose();
        _mutexProcess?.Dispose();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        _mutexProcess = new Mutex(true, "ExploripCopy", out bool processNotLaunched);
        if (processNotLaunched)
        {
            ExploripSharedCopy.Constants.Colors.LoadTheme();
            Constants.Localization.LoadTranslation();
            Constants.Icons.LoadIcons();
            ExploripConfig.Configuration.ConfigManager.Init();

            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            NotifyIconViewModel.Instance.SetControl(notifyIcon);

            GUI.MainWindow mainWindow = new();
            mainWindow.Show();
        }
        else
        {
            Current.Exit -= Application_Exit;
            Environment.Exit(-1);
        }
    }
}
