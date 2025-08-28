using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.Common.Logging;
using ManagedShell.WindowsTray;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for NotifyIconList.xaml
/// </summary>
[ObservableObject()]
public partial class NotifyIconList : UserControl
{
    private bool isLoaded;

    public NotifyIconList()
    {
        InitializeComponent();
    }

    public NotifyIconListViewModel MyDataContext
    {
        get { return (NotifyIconListViewModel)DataContext; }
    }

    public void ChangePinItem(ManagedShell.WindowsTray.NotifyIcon item)
    {
        ShellLogger.Debug($"ChangePinned systray icon of {item.Title}");

        item.IsPinned = !item.IsPinned;
        if (item.IsPinned)
            ConfigManager.AddPinnedSystray(item.Path, item.PinOrder);
        else
            ConfigManager.RemovePinnedSystray(item.Path);

        MyDataContext.RefreshCollectionView();
    }

    private void NotifyIconList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!isLoaded && MyTaskbarApp.MyShellManager.NotificationArea != null)
        {
            MyDataContext.ChangeEdge(this.FindControlParent<Taskbar>().AppBarEdge);

            if (this.FindControlParent<Taskbar>().MainScreen)
            {
                string[] pinnedPath = ConfigManager.AllPinnedSystray();

                if (pinnedPath?.Length > 0 && MyTaskbarApp.MyShellManager.NotificationArea?.TrayIcons?.Count > 0)
                    foreach (string path in pinnedPath)
                    {
                        IEnumerable<ManagedShell.WindowsTray.NotifyIcon> listNotifIcon = MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons.OfType<ManagedShell.WindowsTray.NotifyIcon>().Where(i => string.Compare(i.Path, path, true) == 0 || (path.IndexOf(System.IO.Path.DirectorySeparatorChar) < 0 && string.Compare(System.IO.Path.GetFileName(i.Path), path, true) == 0));
                        if (listNotifIcon.Any())
                            foreach (ManagedShell.WindowsTray.NotifyIcon notifIcon in listNotifIcon)
                                notifIcon.IsPinned = true;
                    }

                MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons.CollectionChanged += SystrayIcons_CollectionChanged;
                MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;
            }

            MyDataContext.RebuildCollectionView();

            MyDataContext.ShowAllIcons = !ConfigManager.GetTaskbarConfig(this.FindControlParent<Taskbar>().NumScreen).CollapseNotifyIcons;

            MyDataContext.RefreshCollectionView();

            isLoaded = true;
        }
    }

    private void NotificationArea_NotificationBalloonShown(object sender, NotificationBalloonEventArgs e)
    {
        // This is used to promote unpinned icons to show when the tray is collapsed.

        if (MyTaskbarApp.MyShellManager.NotificationArea == null)
            return;

        ShellLogger.Debug($"NotificationBalloonShown of {e.Balloon.Title}");

        ManagedShell.WindowsTray.NotifyIcon notifyIcon = e.Balloon.NotifyIcon;

        if (notifyIcon != null && !notifyIcon.IsPinned)
        {
            notifyIcon.IsPinned = true;
            MyDataContext.RefreshCollectionView();
            DispatcherTimer unpromoteTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(e.Balloon.Timeout + 500) // Keep it around for a few ms for the animation to complete
            };
            unpromoteTimer.Tick += (mysender, ea) =>
            {
                notifyIcon.IsPinned = false;
                unpromoteTimer.Stop();
            };
            unpromoteTimer.Start();
        }
    }

    private void NotifyIconList_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (MyTaskbarApp.MyShellManager.NotificationArea != null)
        {
            MyTaskbarApp.MyShellManager.NotificationArea.TrayIcons.CollectionChanged -= SystrayIcons_CollectionChanged;
            MyTaskbarApp.MyShellManager.NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
        }

        isLoaded = false;
    }

    private void SystrayIcons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ShellLogger.Debug("UnpinnedIcons CollectionChanged");

        bool refreshList = false;
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            string[] pinnedPath = ConfigManager.AllPinnedSystray();
            foreach (ManagedShell.WindowsTray.NotifyIcon item in e.NewItems)
            {
                if (pinnedPath.Any(pin => string.Compare(pin, item.Path, true) == 0) || (pinnedPath.Any(pin => pin.IndexOf(System.IO.Path.DirectorySeparatorChar) < 0 && string.Compare(System.IO.Path.GetFileName(item.Path), pin, true) == 0)))
                {
                    refreshList = true;
                    item.IsPinned = true;
                }
            }
        }

        if (refreshList)
        {
            ShellLogger.Debug("Refresh Systray list");
            MyDataContext.RefreshCollectionView();
        }
    }
}
