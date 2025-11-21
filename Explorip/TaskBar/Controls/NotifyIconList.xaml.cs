using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for NotifyIconList.xaml
/// </summary>
[ObservableObject()]
public partial class NotifyIconList : UserControl
{
    private bool _isLoaded;
    private readonly object _lockLoaded;
    private bool _ignoreReload = false;

    public NotifyIconList()
    {
        InitializeComponent();
        _lockLoaded = new object();
        this.IsVisibleChanged += NotifyIconList_IsVisibleChanged;
    }

    private void NotifyIconList_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ignoreReload = true;
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            await Task.Delay(1000);
            _ignoreReload = false;
        });
    }

    public NotifyIconListViewModel MyDataContext
    {
        get { return (NotifyIconListViewModel)DataContext; }
    }

    private void NotifyIconList_OnLoaded(object sender, RoutedEventArgs e)
    {
        lock (_lockLoaded)
        {
            if ((!_isLoaded || !_ignoreReload) && MyTaskbarApp.MyShellManager.NotificationArea != null)
            {
                _isLoaded = true;
                MyDataContext.Init(this);
                MyDataContext.Resume();
            }
        }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        lock (_lockLoaded)
        {
            if (_ignoreReload)
                return;

            MyDataContext.Unload();
            MyDataContext.Pause();
            _isLoaded = false;
        }
    }
}
