using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Logging;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskList.xaml
/// </summary>
public partial class TaskList : UserControl
{
    private bool _isLoaded;
    private bool _isMainScreen = false;
    private bool _ignoreReload = false;

    public TaskList()
    {
        InitializeComponent();
        IsVisibleChanged += TaskList_IsVisibleChanged;
    }

    private void TaskList_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ignoreReload = true;
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            await Task.Delay(1000);
            _ignoreReload = false;
        });
    }

    public TaskListViewModel MyDataContext
    {
        get { return (TaskListViewModel)DataContext; }
    }

    private void TaskList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if ((!_isLoaded || !_ignoreReload) && MyTaskbarApp.MyShellManager.Tasks != null)
        {
            _isLoaded = true;
            Taskbar tb = (Taskbar)Window.GetWindow(this);
            MyDataContext.TaskbarParent = tb;
            MyDataContext.ChangeEdge(tb.AppBarEdge);
            _isMainScreen = tb.MainScreen;
            ShellLogger.Debug("OnLoaded of TaskList Screen " + tb.NumScreen);
            if (_isMainScreen)
                MyDataContext.FirstRefresh();
        }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        if (_ignoreReload)
            return;

        ShellLogger.Debug("OnUnloaded of TaskList Screen " + ((Taskbar)Window.GetWindow(this)).NumScreen);

        MyDataContext.RemoveTaskServiceEvent();
        if (_isMainScreen)
            TaskListViewModel.DisposeAllApplicationWindow();
        _isLoaded = false;
    }
}
