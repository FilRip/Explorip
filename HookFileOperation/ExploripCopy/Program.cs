using System;
using System.Windows;

namespace ExploripCopy;

public static class Program
{
    [STAThread()]
    public static void Main(string[] args)
    {
#if !DEBUG
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
        try
        {
            new ExploripCopyApp().Run();
        }
        catch (Exception ex)
        {
            CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(ex, false));
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        if (MessageBox.Show($"Unhandled error happends : {Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}At : {ex.StackTrace}{Environment.NewLine}{Environment.NewLine}Please report it at filrip@gmail.com or in 'issue' on official website. Thank you.{Environment.NewLine}{Environment.NewLine}The application can be unstable. Do you want to continue ?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.No)
            Environment.Exit(1);
    }
}
