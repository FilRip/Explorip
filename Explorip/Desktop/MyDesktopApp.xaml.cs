using System;
using System.Windows;

using Explorip.Desktop.Windows;

using ManagedShell.Common.Helpers;

using WpfScreenHelper;

namespace Explorip.Desktop;

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
        ExploripDesktop desktop = new()
        {
            AssociateScreen = Screen.PrimaryScreen,
        };
        desktop.InitDesktopWindow();
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        // TODO : Close my window desktop(s)
    }
}
