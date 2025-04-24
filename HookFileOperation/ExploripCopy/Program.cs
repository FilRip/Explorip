using System;
using System.IO;
using System.Windows;

namespace ExploripCopy;

public static class Program
{
    private const string AppDomainName = "ExploripCopyAppDomain";

    public static void Main(string[] args)
    {
        if (AppDomain.CurrentDomain.FriendlyName != AppDomainName)
        {
            AppDomainSetup appDomainSetup = AppDomain.CurrentDomain.SetupInformation;
            appDomainSetup.ShadowCopyFiles = "true";
            appDomainSetup.ShadowCopyDirectories = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            appDomainSetup.ApplicationBase = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            AppDomain myAppDomain = AppDomain.CreateDomain(AppDomainName, null, appDomainSetup);
#if !DEBUG
            myAppDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            try
            {
                myAppDomain.ExecuteAssembly(Environment.GetCommandLineArgs()[0], args);
            }
            catch (Exception ex)
            {
                CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(ex, false));
            }
            return;
        }
        new App().Run();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        if (MessageBox.Show($"Unhandled error happends : {Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}At : {ex.StackTrace}{Environment.NewLine}{Environment.NewLine}Please report it at filrip@gmail.com or in 'issue' on official website. Thank you.{Environment.NewLine}{Environment.NewLine}The application can be unstable. Do you want to continue ?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.No)
            Environment.Exit(1);
    }
}
