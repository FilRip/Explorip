using System.Windows;

using Explorip.TaskBar.Utilities;

using ManagedShell;

namespace Explorip.StartMenu;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class MyStartMenuApp : Application
{
    public static ShellManager MyShellManager { get; private set; }
    private ManagedShellLogger _logger;

    public MyStartMenuApp()
    {
        InitializeComponent();
        MyShellManager = SetupManagedShell();
    }

    private ShellManager SetupManagedShell()
    {
#if DEBUG
        _logger = new ManagedShellLogger();
#endif
        ShellConfig config = new()
        {
            EnableTasksService = true,
            AutoStartTasksService = true,
            EnableTrayService = false,
            AutoStartTrayService = false,
        };

        return new ShellManager(config);
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _logger?.Dispose();
    }
}
