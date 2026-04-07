using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ExploripFileExplorer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        string subDir;
#if DEBUG
        subDir = "debug";
#else
        subDir = "release";
#endif
        Process.Start(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), subDir, "explorip.exe"));
    }
}
