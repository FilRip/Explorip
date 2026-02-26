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
            e.Reason == SessionSwitchReason.SessionUnlock) && DataContext.IsPause)
        {
            DataContext.Pause();
        }
        else
        {
            DataContext.Dispose();
        }
    }

    public new ComputerInfoViewModel DataContext
    {
        get { return (ComputerInfoViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext.IsPause)
            DataContext.Pause();
    }

    private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        DataContext.Dispose();
    }
}
