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
            Taskbar tb = this.FindControlParent<Taskbar>();
            MyDataContext.TaskbarParent = tb;
            MyDataContext.ChangeEdge(tb.AppBarEdge);

            if (tb.MainScreen)
                MyDataContext.FirstRefresh();
        }
    }
}
