using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

using ExploripConfig.Configuration;

using ExploripCopy.ViewModels;

using Hardcodet.Wpf.TaskbarNotification;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

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
            ExploripSharedCopy.Constants.Localization.LoadTranslation();
            Constants.Localization.LoadTranslation();
            Constants.Icons.LoadIcons();
            ExploripCopyConfig.Init();

            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            NotifyIconViewModel.Instance.SetControl(notifyIcon);

            GUI.MainWindow mainWindow = new();
            mainWindow.Show();

            if (ExploripCopyConfig.InjectWindowsExplorer && Process.GetProcessesByName("explorer").Length > 0)
            {
                string path = ArgumentPathExe();
                if (ArgumentVariableExists("currentdir"))
                    path = ArgumentValue("currentdir");
                string filename = "HookFileOperationsManager.exe";
                if (!File.Exists(Path.Combine(path, filename)))
                    path = Path.Combine(path, "lib");
                Process.Start(Path.Combine(path, filename), Process.GetProcessesByName("explorer")[0].Id.ToString());
            }
        }
        else
        {
            Current.Exit -= Application_Exit;
            Environment.Exit(-1);
        }
    }
}
