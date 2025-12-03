using System.Windows.Controls;

using ComputerInfo.ViewModels;

using Microsoft.Win32;

namespace ComputerInfo.Controls;

public partial class ComputerInfoControl : UserControl
{
    public ComputerInfoControl()
    {
        InitializeComponent();
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if ((e.Reason == SessionSwitchReason.ConsoleConnect ||
            e.Reason == SessionSwitchReason.RemoteConnect ||
            e.Reason == SessionSwitchReason.SessionLogon ||
            e.Reason == SessionSwitchReason.SessionUnlock) && MyDataContext.IsPause)
        {
            MyDataContext.Pause();
        }
        else
        {
            MyDataContext.Dispose();
        }
    }

    public ComputerInfoViewModel MyDataContext
    {
        get { return (ComputerInfoViewModel)DataContext; }
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (MyDataContext.IsPause)
            MyDataContext.Pause();
    }

    private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        MyDataContext.Dispose();
    }
}
