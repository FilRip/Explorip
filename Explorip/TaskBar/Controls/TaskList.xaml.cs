using System;
using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.AppBar;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskList.xaml
/// </summary>
public partial class TaskList : UserControl
{
    private bool isLoaded;
    private double DefaultButtonWidth;
    private double TaskButtonLeftMargin;
    private double TaskButtonRightMargin;

    public readonly static DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(TaskList), new PropertyMetadata(new double()));

    public TaskList()
    {
        InitializeComponent();
    }

    public TaskListViewModel MyDataContext
    {
        get { return (TaskListViewModel)DataContext; }
    }

    public double ButtonWidth
    {
        get { return (double)GetValue(ButtonWidthProperty); }
        set { SetValue(ButtonWidthProperty, value); }
    }

    private void SetStyles()
    {
        DefaultButtonWidth = Application.Current.FindResource("TaskButtonWidth") as double? ?? 0;
        Thickness buttonMargin;

        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Left || ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Right)
            buttonMargin = Application.Current.FindResource("TaskButtonVerticalMargin") as Thickness? ?? new Thickness();
        else
            buttonMargin = Application.Current.FindResource("TaskButtonMargin") as Thickness? ?? new Thickness();

        TaskButtonLeftMargin = buttonMargin.Left;
        TaskButtonRightMargin = buttonMargin.Right;
    }

    private void TaskList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if (!isLoaded && MyTaskbarApp.MyShellManager.Tasks != null)
        {
            isLoaded = true;
            Taskbar tb = this.FindControlParent<Taskbar>();
            MyDataContext.TaskbarParent = tb;
            MyDataContext.ChangeEdge(tb.AppBarEdge);

            if (tb.MainScreen)
                MyDataContext.FirstRefresh();
        }

        SetStyles();
    }

    private void TaskList_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetTaskButtonWidth();
    }

    private void SetTaskButtonWidth()
    {
        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Left || ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Right)
        {
            ButtonWidth = ActualWidth;
            return;
        }

        double margin = TaskButtonLeftMargin + TaskButtonRightMargin;
        double maxWidth = TasksList.ActualWidth / TasksList.Items.Count;
        double defaultWidth = DefaultButtonWidth + margin;

        if (maxWidth > defaultWidth)
            ButtonWidth = DefaultButtonWidth;
        else
            ButtonWidth = Math.Floor(maxWidth);
    }
}
