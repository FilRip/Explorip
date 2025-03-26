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
    private bool _isLoaded;
    private double _defaultButtonWidth;
    private double _taskButtonLeftMargin;
    private double _taskButtonRightMargin;

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
        _defaultButtonWidth = Application.Current.FindResource("TaskButtonWidth") as double? ?? 0;
        Thickness buttonMargin;

        if (ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Left || ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().ScreenName).Edge == AppBarEdge.Right)
            buttonMargin = Application.Current.FindResource("TaskButtonVerticalMargin") as Thickness? ?? new Thickness();
        else
            buttonMargin = Application.Current.FindResource("TaskButtonMargin") as Thickness? ?? new Thickness();

        _taskButtonLeftMargin = buttonMargin.Left;
        _taskButtonRightMargin = buttonMargin.Right;
    }

    private void TaskList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if (!_isLoaded && MyTaskbarApp.MyShellManager.Tasks != null)
        {
            _isLoaded = true;
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

        double margin = _taskButtonLeftMargin + _taskButtonRightMargin;
        double maxWidth = TasksList.ActualWidth / TasksList.Items.Count;
        double defaultWidth = _defaultButtonWidth + margin;

        if (maxWidth > defaultWidth)
            ButtonWidth = _defaultButtonWidth;
        else
            ButtonWidth = Math.Floor(maxWidth);
    }
}
