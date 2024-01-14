using System;
using System.Windows;
using System.Windows.Interop;

using ExploripCopy;

namespace Explorip.Desktop.Windows;

/// <summary>
/// Logique d'interaction pour ExploripDesktop.xaml
/// </summary>
public partial class ExploripDesktop : Window
{
    public ExploripDesktop()
    {
        InitializeComponent();
    }

    public IntPtr GetHandle()
    {
        return new WindowInteropHelper(this).EnsureHandle();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Program.MyCurrentApp.Shutdown();
    }
}
