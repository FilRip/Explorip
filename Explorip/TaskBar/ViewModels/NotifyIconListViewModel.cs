using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;
using Explorip.TaskBar.Helpers;

using ExploripConfig.Configuration;

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
    private bool _alreadyInit;
    private Controls.NotifyIconList _parentControl;

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
        ListSystrayIcons?.Refresh();
    }

    public static void RefreshAllCollectionView()
    {
        ShellLogger.Debug("Refresh All Collection of SysTray");
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            foreach (Controls.Taskbar tb in ((MyTaskbarApp)Application.Current).ListAllTaskbar())
                tb.MySystray.DataContext.RefreshCollectionView();
        }, DispatcherPriority.Background);
    }

    public void RebuildCollectionView()
    {
        ShellLogger.Debug("Rebuild All Collection of SysTray");
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

    public static void ChangePinItem(NotifyIcon item)
    {
        ShellLogger.Debug($"ChangePinned systray icon of {item.Title}");

        item.IsPinned = !item.IsPinned;
        if (item.IsPinned)
            ConfigManager.AddPinnedSystray(item.Path, item.PinOrder);
        else
            ConfigManager.RemovePinnedSystray(item.Path);

        RefreshAllCollectionView();
    }

    public Controls.Taskbar ParentTaskbar
    {
        get { return _parentControl.FindControlParent<Controls.Taskbar>(); }
    }

    public void Init(Controls.NotifyIconList control)
    {
        if (_alreadyInit)
            return;
        _parentControl = control;
        _alreadyInit = true;
        ChangeEdge(ParentTaskbar.AppBarEdge);

        if (ParentTaskbar.MainScreen)
        {
            string[] pinnedPath = ConfigManager.AllPinnedSystray();

            if (pinnedPath?.Length > 0 && MyTaskbarApp.MyShellManager.NotificationArea?.TrayIcons?.Count > 0)
                foreach (string path in pinnedPath)
                {
                    IEnumerable<NotifyIcon> listNotifIcon = MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons.OfType<NotifyIcon>().Where(i => !i.IsHidden && (string.Compare(i.Path, path, true) == 0 || (path.IndexOf(Path.DirectorySeparatorChar) < 0 && string.Compare(Path.GetFileName(i.Path), path, true) == 0)));
                    if (listNotifIcon.Any())
                        foreach (NotifyIcon notifIcon in listNotifIcon)
                            notifIcon.IsPinned = true;
                }
        }

        RegisterEvents();
        RebuildCollectionView();

        ShowAllIcons = !ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).CollapseNotifyIcons;

        RefreshCollectionView();
    }

    private void NotificationArea_NotificationBalloonShown(object sender, NotificationBalloonEventArgs e)
    {
        // This is used to promote unpinned icons to show when the tray is collapsed.

        if (MyTaskbarApp.MyShellManager.NotificationArea == null ||
            MyTaskbarApp.MyShellManager.NotificationArea.Disable)
        {
            return;
        }

        ShellLogger.Debug($"NotificationBalloonShown of {e.Balloon.Title}");

        NotifyIcon notifyIcon = e.Balloon.NotifyIcon;

        if (notifyIcon != null && !notifyIcon.IsPinned)
        {
            notifyIcon.IsPinned = true;
            RefreshCollectionView();
            DispatcherTimer unpromoteTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(e.Balloon.Timeout + 500), // Keep it around for a few ms for the animation to complete
            };
            unpromoteTimer.Tick += (mysender, ea) =>
            {
                notifyIcon.IsPinned = false;
                unpromoteTimer.Stop();
            };
            unpromoteTimer.Start();
        }
    }

    private static void SystrayIcons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ShellLogger.Debug("SystrayIcons CollectionChanged");

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            string[] pinnedPath = ConfigManager.AllPinnedSystray();
            foreach (NotifyIcon item in e.NewItems)
            {
                if (!item.IsHidden && pinnedPath.Any(pin => string.Compare(pin, item.Path, true) == 0) || (pinnedPath.Any(pin => pin.IndexOf(Path.DirectorySeparatorChar) < 0 && string.Compare(Path.GetFileName(item.Path), pin, true) == 0)))
                    item.IsPinned = true;
            }
        }

        ShellLogger.Debug("Refresh Systray list");
        RefreshAllCollectionView();
    }

    public void Unload()
    {
        UnregisterEvents();
    }

    private void UnregisterEvents()
    {
        if (ParentTaskbar.MainScreen && MyTaskbarApp.MyShellManager.NotificationArea != null)
        {
            MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons.CollectionChanged -= SystrayIcons_CollectionChanged;
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
        }
    }

    private void RegisterEvents()
    {
        if (ParentTaskbar.MainScreen && MyTaskbarApp.MyShellManager.NotificationArea != null)
        {
            UnregisterEvents();
            MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons.CollectionChanged += SystrayIcons_CollectionChanged;
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
        }
    }

    public void Pause()
    {
        // For unknown reason, for now, when session is locked, on some computers/windows (not all)
        // There is a memoryleak of systray icons. So we disable it when session locked, and restore/refresh it when session is back
        UnregisterEvents();
        ListSystrayIcons = null;
    }

    public void Resume()
    {
        RegisterEvents();
        RebuildCollectionView();
    }
}
