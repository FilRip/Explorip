using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.TaskBar.Helpers;

using ManagedShell.AppBar;
using ManagedShell.Common.Logging;
using ManagedShell.WindowsTray;

namespace Explorip.TaskBar.ViewModels;

public partial class NotifyIconListViewModel : ObservableObject
{
    private AppBarEdge _currentEdge;

    [ObservableProperty()]
    private ICollectionView _listSystrayIcons;
    [ObservableProperty()]
    private bool _showAllIcons;

    public void ChangeEdge(AppBarEdge newEdge)
    {
        _currentEdge = newEdge;
        OnPropertyChanged(nameof(StackPanelOrientation));
    }

    public Orientation StackPanelOrientation
    {
        get { return _currentEdge.GetOrientation(); }
    }

    public void RefreshCollectionView()
    {
        ListSystrayIcons.Refresh();
    }

    public void RebuildCollectionView()
    {
        ListSystrayIcons = new ListCollectionView(MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons);
        ListSystrayIcons.SortDescriptions.Add(new SortDescription(nameof(NotifyIcon.PinOrder), ListSortDirection.Ascending));
        ListSystrayIcons.Filter = FilterSystrayIcon;
    }

    private bool FilterSystrayIcon(object icon)
    {
        return (icon is NotifyIcon ni && !ni.IsHidden && (ShowAllIcons || ni.IsPinned));
    }

    partial void OnShowAllIconsChanged(bool value)
    {
        ShellLogger.Debug("Change show/hide unpinned icon in systray");
        RefreshCollectionView();
    }
}
