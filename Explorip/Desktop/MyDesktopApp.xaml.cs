using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Explorip.Desktop.Windows;

using WpfScreenHelper;

using static Explorip.Helpers.ExtensionsCommandLineArguments;

namespace Explorip.Desktop;

/// <summary>
/// Logique d'interaction pour MyApp.xaml
/// </summary>
public partial class MyDesktopApp : Application
{
    private readonly List<ExploripDesktop> _listDesktop = [];

    public MyDesktopApp()
    {
        InitializeComponent();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        ManagedShell.Common.Helpers.ShellHelper.ToggleDesktopIcons(false);
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        ExploripDesktop desktop = new()
        {
            AssociateScreen = Screen.PrimaryScreen,
        };
        desktop.InitDesktopWindow();
        _listDesktop.Add(desktop);
        if (ArgumentExists("desktops"))
        {
            foreach (Screen screen in Screen.AllScreens.Where(s => !s.Primary))
            {
                ExploripDesktop ed = new()
                {
                    AssociateScreen = screen,
                };
                ed.InitDesktopWindow();
                _listDesktop.Add(ed);
            }
        }
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        foreach (ExploripDesktop desktop in _listDesktop)
            desktop.Dispatcher.Invoke(() =>
            {
                desktop.Close();
            });
        ManagedShell.Common.Helpers.ShellHelper.ToggleDesktopIcons(true);
    }
}
