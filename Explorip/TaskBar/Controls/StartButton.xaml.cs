using System.Windows;

using Explorip.TaskBar.Utilities;
using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for StartButton.xaml
/// </summary>
public partial class StartButton : System.Windows.Controls.UserControl
{
    public readonly static DependencyProperty StartMenuMonitorProperty = DependencyProperty.Register("StartMenuMonitor", typeof(StartMenuMonitor), typeof(StartButton));

    public StartMenuMonitor StartMenuMonitor
    {
        get { return (StartMenuMonitor)GetValue(StartMenuMonitorProperty); }
        set { SetValue(StartMenuMonitorProperty, value); }
    }

    public new StartButtonViewModel DataContext
    {
        get { return (StartButtonViewModel)base.DataContext; }
    }

    public StartButton()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        DataContext.ParentControl = this;
        DataContext.Init();
        StartMenuMonitor.StartMenuVisibilityChanged += DataContext.AppVisibilityHelper_StartMenuVisibilityChanged;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        StartMenuMonitor.StartMenuVisibilityChanged -= DataContext.AppVisibilityHelper_StartMenuVisibilityChanged;
    }
}
