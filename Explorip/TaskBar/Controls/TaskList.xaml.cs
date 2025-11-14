using System.Windows;
using System.Windows.Controls;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskList.xaml
/// </summary>
public partial class TaskList : UserControl
{
    private bool _isLoaded;

    public TaskList()
    {
        InitializeComponent();
    }

    public TaskListViewModel MyDataContext
    {
        get { return (TaskListViewModel)DataContext; }
    }

    private void TaskList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if (!_isLoaded && MyTaskbarApp.MyShellManager.Tasks != null)
        {
            _isLoaded = true;
            Taskbar tb = (Taskbar)Window.GetWindow(this);
            MyDataContext.TaskbarParent = tb;
            MyDataContext.ChangeEdge(tb.AppBarEdge);

            if (tb.MainScreen)
                MyDataContext.FirstRefresh();
        }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        MyDataContext.RemoveTaskServiceEvent();
        _isLoaded = false;
    }
}
