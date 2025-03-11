using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for Clock.xaml
/// </summary>
public partial class Clock : UserControl
{
    public Clock()
    {
        InitializeComponent();
        MyDataContext.SetDispatcher(Dispatcher);
    }

    private ClockViewModel MyDataContext
    {
        get { return (ClockViewModel)DataContext; }
    }

    private static void OpenDateTimeCpl()
    {
        ShellHelper.StartProcess("start timedate.cpl");
    }

    private void Clock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (EnvironmentHelper.IsWindows11OrBetter)
        {
            ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.KEY_A);
        }
        else
        {
            ShellHelper.ShellKeyCombo(ManagedShell.Interop.NativeMethods.VK.LWIN, ManagedShell.Interop.NativeMethods.VK.LMENU, ManagedShell.Interop.NativeMethods.VK.KEY_D);
        }
    }

    private void Clock_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        MyDataContext.SingleClickStop();
        OpenDateTimeCpl();

        e.Handled = true;
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        OpenDateTimeCpl();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).ShowClock)
            MyDataContext.ShowClock();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).ShowClock)
            MyDataContext.HideClock();
    }
}
