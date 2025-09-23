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
    private bool isLoaded;
    private readonly object _lockLoaded;

    public NotifyIconList()
    {
        InitializeComponent();
        _lockLoaded = new object();
    }

    public NotifyIconListViewModel MyDataContext
    {
        get { return (NotifyIconListViewModel)DataContext; }
    }

    private void NotifyIconList_OnLoaded(object sender, RoutedEventArgs e)
    {
        lock (_lockLoaded)
        {
            if (!isLoaded && MyTaskbarApp.MyShellManager.NotificationArea != null)
            {
                isLoaded = true;
                MyDataContext.Init(this);
            }
        }
    }
}
