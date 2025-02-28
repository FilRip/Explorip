using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

using ExploripConfig.Configuration;

using ManagedShell.WindowsTray;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for NotifyIconList.xaml
/// </summary>
public partial class NotifyIconList : UserControl
{
    private bool isLoaded;
    private CollectionViewSource allNotifyIconsSource;
    private CollectionViewSource pinnedNotifyIconsSource;
    private readonly ObservableCollection<ManagedShell.WindowsTray.NotifyIcon> promotedIcons = [];

    public readonly static DependencyProperty NotificationAreaProperty = DependencyProperty.Register("NotificationArea", typeof(NotificationArea), typeof(NotifyIconList));

    public NotificationArea NotificationArea
    {
        get { return (NotificationArea)GetValue(NotificationAreaProperty); }
        set { SetValue(NotificationAreaProperty, value); }
    }

    public NotifyIconList()
    {
        InitializeComponent();
    }

    public void ChangePinItem(ManagedShell.WindowsTray.NotifyIcon item)
    {
        if (promotedIcons.Contains(item))
        {
            promotedIcons.Remove(item);
            ConfigManager.RemovePinnedSystray(item.Path);
        }
        else
        {
            promotedIcons.Add(item);
            ConfigManager.AddPinnedSystray(item.Path, item.PinOrder);
        }
        CompositeCollection pinnedNotifyIcons =
        [
            new CollectionContainer { Collection = promotedIcons },
            new CollectionContainer { Collection = NotificationArea.PinnedIcons },
        ];
        pinnedNotifyIconsSource = new CollectionViewSource() { Source = pinnedNotifyIcons };
    }

    private void NotifyIconList_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!isLoaded && NotificationArea != null)
        {
            string[] pinnedPath = ConfigManager.AllPinnedSystray();

            CompositeCollection allNotifyIcons =
            [
                new CollectionContainer { Collection = NotificationArea.UnpinnedIcons },
                new CollectionContainer { Collection = NotificationArea.PinnedIcons },
            ];
            allNotifyIconsSource = new CollectionViewSource() { Source = allNotifyIcons };

            if (pinnedPath?.Length > 0 && promotedIcons.Count == 0)
                foreach (string path in pinnedPath)
                {
                    ManagedShell.WindowsTray.NotifyIcon ni = NotificationArea.UnpinnedIcons.OfType<ManagedShell.WindowsTray.NotifyIcon>().SingleOrDefault(i => i.Path == path);
                    if (ni != null)
                        promotedIcons.Add(ni);
                }

            CompositeCollection pinnedNotifyIcons =
            [
                new CollectionContainer { Collection = promotedIcons },
                new CollectionContainer { Collection = NotificationArea.PinnedIcons },
            ];
            pinnedNotifyIconsSource = new CollectionViewSource() { Source = pinnedNotifyIcons };

            NotificationArea.UnpinnedIcons.CollectionChanged += UnpinnedIcons_CollectionChanged;
            NotificationArea.NotificationBalloonShown += NotificationArea_NotificationBalloonShown;

            if (ConfigManager.CollapseNotifyIcons)
            {
                NotifyIcons.ItemsSource = pinnedNotifyIconsSource.View;
                SetToggleVisibility();

                if (NotifyIconToggleButton.IsChecked == true)
                {
                    NotifyIconToggleButton.IsChecked = false;
                }
            }
            else
            {
                NotifyIcons.ItemsSource = allNotifyIconsSource.View;
            }

            isLoaded = true;
        }
    }

    private void NotificationArea_NotificationBalloonShown(object sender, NotificationBalloonEventArgs e)
    {
        // This is used to promote unpinned icons to show when the tray is collapsed.

        if (NotificationArea == null)
        {
            return;
        }

        ManagedShell.WindowsTray.NotifyIcon notifyIcon = e.Balloon.NotifyIcon;

        if (NotificationArea.PinnedIcons.Contains(notifyIcon))
        {
            // Do not promote pinned icons (they're already there!)
            return;
        }

        if (promotedIcons.Contains(notifyIcon))
        {
            // Do not duplicate promoted icons
            return;
        }

        promotedIcons.Add(notifyIcon);

        DispatcherTimer unpromoteTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(e.Balloon.Timeout + 500) // Keep it around for a few ms for the animation to complete
        };
        unpromoteTimer.Tick += (object mysender, EventArgs ea) =>
        {
            if (promotedIcons.Contains(notifyIcon))
            {
                promotedIcons.Remove(notifyIcon);
            }
            unpromoteTimer.Stop();
        };
        unpromoteTimer.Start();
    }

    private void NotifyIconList_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (NotificationArea != null)
        {
            NotificationArea.UnpinnedIcons.CollectionChanged -= UnpinnedIcons_CollectionChanged;
            NotificationArea.NotificationBalloonShown -= NotificationArea_NotificationBalloonShown;
        }

        isLoaded = false;
    }

    private void UnpinnedIcons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        SetToggleVisibility();
    }

    private void NotifyIconToggleButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (NotifyIconToggleButton.IsChecked == true)
        {

            NotifyIcons.ItemsSource = allNotifyIconsSource.View;
        }
        else
        {
            NotifyIcons.ItemsSource = pinnedNotifyIconsSource.View;
        }
    }

    private void SetToggleVisibility()
    {
        if (!ConfigManager.CollapseNotifyIcons)
            return;

        if (NotificationArea.UnpinnedIcons.IsEmpty)
        {
            NotifyIconToggleButton.Visibility = Visibility.Collapsed;

            if (NotifyIconToggleButton.IsChecked == true)
            {
                NotifyIconToggleButton.IsChecked = false;
            }
        }
        else
        {
            NotifyIconToggleButton.Visibility = Visibility.Visible;
        }
    }
}
