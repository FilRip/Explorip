using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

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

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().NumScreen).ShowClock)
            MyDataContext.ShowClock();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().NumScreen).ShowClock)
            MyDataContext.HideClock();
    }
}
