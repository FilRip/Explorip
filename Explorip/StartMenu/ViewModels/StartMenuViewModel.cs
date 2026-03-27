using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Helpers;
using Explorip.StartMenu.Controls;
using Explorip.StartMenu.Window;
using Explorip.TaskBar;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;

using Microsoft.Win32;

using Securify.ShellLink;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuViewModel : ObservableObject, IDisposable
{
    private readonly double _iconSizeWidth, _iconSizeHeight, _iconSizeWidth2, _iconSizeHeight2;
    private readonly ContextMenu _cmUser, _cmStop, _cmStartMenu;
    private CustomColor _winColor;
    private FileSystemWatcher _startMenuCommonWatcher;
    private FileSystemWatcher _startMenuUserWatcher;
    private FileSystemWatcher _pinnedAppWatcher;
    private FileSystemWatcher _pinnedApp2Watcher;

    [ObservableProperty()]
    private ObservableCollection<StartMenuItemViewModel> _startMenuItems;
    [ObservableProperty()]
    private ObservableCollection<PinnedShortutViewModel> _pinnedShortcut;
    [ObservableProperty()]
    private ObservableCollection<PinnedShortutViewModel> _pinnedShortcut2;
    [ObservableProperty()]
    private bool _showPanel2;
    [ObservableProperty()]
    private bool _showApplicationsPrograms;
    [ObservableProperty()]
    private double _height;
    [ObservableProperty()]
    private CornerRadius _pinnedAppCornerRadius;

    private bool disposedValue;

    public Action ShowWindow { get; set; }
    public Action HideWindow { get; set; }

    public StartMenuViewModel()
    {
        #region Build all context menu

        _cmStartMenu = new()
        {
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
        };
        CreateMenuItem(_cmStartMenu, Constants.Localization.SHOW_SECOND_START_MENU_PANEL, ChangeShowPanel2, true);
        CreateMenuItem(_cmStartMenu, Constants.Localization.SHOW_STARTMENUITEM_STARTWINDOW, ChangeShowStartPrograms, true);
        CreateMenuItem(_cmStartMenu, Constants.Localization.REFRESH, RefreshAll);
        CreateMenuItem(_cmStartMenu, Constants.Localization.CUSTOM_COLOR, ShowCustomColor);
        if (MyStartMenuApp.MyShellManager != null)
        {
            CreateMenuItem(_cmStartMenu, Constants.Localization.QUIT + " StartMenu", Leave);
        }

        _cmUser = new()
        {
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
        };
        CreateMenuItem(_cmUser, Constants.Localization.LOCK, ShellHelper.Lock, icon: Constants.Icons.Lock);
        CreateMenuItem(_cmUser, Constants.Localization.DISCONNECT, ShellHelper.Logoff, icon: Constants.Icons.SignOut);
        CreateMenuItem(_cmUser, Constants.Localization.CHANGE_USER, ShellHelper.Lock, icon: Constants.Icons.SwitchUser);

        _cmStop = new()
        {
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
        };
        CreateMenuItem(_cmStop, Constants.Localization.PUT_HYBERNATE, Hybernate, icon: Constants.Icons.Sleep);
        CreateMenuItem(_cmStop, Constants.Localization.SHUTDOWN, Shutdown, icon: Constants.Icons.Shutdown);
        CreateMenuItem(_cmStop, Constants.Localization.RESTART, Restart, icon: Constants.Icons.Refresh);

        #endregion

        _iconSizeHeight = ConfigManager.StartMenu.StartMenuIconSizeHeight;
        _iconSizeWidth = ConfigManager.StartMenu.StartMenuIconSizeWidth;
        _iconSizeHeight2 = ConfigManager.StartMenu.StartMenuIconSizeHeight2;
        _iconSizeWidth2 = ConfigManager.StartMenu.StartMenuIconSizeWidth2;

        _showPanel2 = ConfigManager.StartMenu.StartMenuShowPinnedApp2;
        _showApplicationsPrograms = ConfigManager.StartMenu.StartMenuShowApplicationsPrograms;

        _height = ConfigManager.StartMenu.StartMenuHeight;
        _pinnedAppCornerRadius = ConfigManager.StartMenu.StartMenuPinnedAppCornerRadius;

        _startMenuItems = [];
        _pinnedShortcut = [];
        _pinnedShortcut2 = [];

        RefreshAll();
    }

    private static void CreateMenuItem(ContextMenu cm, string label, Action onClick, bool checkable = false, ImageSource icon = null)
    {
        MenuItem mi = new()
        {
            Header = label,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            BorderBrush = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
            IsCheckable = checkable,
        };
        mi.Click += (sender, e) =>
        {
            onClick();
        };
        if (icon != null)
            mi.Icon = new Image() { Source = icon };
        cm.Items.Add(mi);
    }

    #region Shutdown/Restart

    public static void Shutdown()
    {
        ShellHelper.StartProcess("shutdown", "/s /t 0", hidden: true);
        ((MyTaskbarApp)Application.Current).ExitGracefully();
    }

    public static void Restart()
    {
        ShellHelper.StartProcess("shutdown", "/r /t 0", hidden: true);
        ((MyTaskbarApp)Application.Current).ExitGracefully();
    }

    public static void Hybernate()
    {
        ShellHelper.StartProcess("shutdown", "/h", hidden: true);
    }

    public static bool IsHybernateEnabled
    {
        get
        {
            bool result = false;
            RegistryKey Key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power");
            if (Key != null)
            {
                if (Key.GetValue("HibernateEnabled") != null)
                    result = (int)Key.GetValue("HibernateEnabled") == 1;
                else if (Key.GetValue("HibernateEnabledDefault") != null)
                    return (int)Key.GetValue("HibernateEnabledDefault") == 1;
            }
            Key.Close();
            return result;
        }
    }

    private static bool IsRestartPending
    {
        get
        {
            return IsPendingReboot() || IsWUPendingReboot()/* || IsPendingFileRenameOperations()*/;
        }
    }

    /// <summary>
    /// An application need to reboot
    /// </summary>
    private static bool IsPendingReboot()
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing");
        if (key != null && key.GetValue("RebootPending") != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// A Windows Update need to reboot
    /// </summary>
    private static bool IsWUPendingReboot()
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired");
        if (key != null)
            return true;
        return false;
    }

    /// <summary>
    /// An application need to reboot to load it's new file (need a restart to free loaded files and rename them by the new ones)<br/>
    /// For examples : some ShellHook of Explorer. The application still working, but not with latest installed version<br/>
    /// An application that said "Installation finished. Need to reboot to apply modifications". It's optional for now, but necessary to use latest version
    /// </summary>
#pragma warning disable S1144 // Unused private types or members should be removed
    private static bool IsPendingFileRenameOperations()
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager");
        if (key != null && key.GetValue("PendingFileRenameOperations") != null)
        {
            return true;
        }
        return false;
    }
#pragma warning restore S1144 // Unused private types or members should be removed

    #endregion

    public void RefreshAll()
    {
        RefreshPrograms();
        RefreshPinnedShortcut();
        RefreshPinnedShortcut2();
    }

    #region Start Menu Program

    public void RefreshPrograms()
    {
        if (_startMenuCommonWatcher != null)
        {
            UnregisterStartMenuWatcher(_startMenuCommonWatcher);
            _startMenuCommonWatcher.Dispose();
        }
        if (_startMenuUserWatcher != null)
        {
            UnregisterStartMenuWatcher(_startMenuUserWatcher);
            _startMenuUserWatcher.Dispose();
        }
        StartMenuItems.Clear();
        string commonSMFolder = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenu.StartMenuCommonProgramsPath);
        if (string.IsNullOrWhiteSpace(commonSMFolder))
            commonSMFolder = Path.Combine(Environment.SpecialFolder.CommonStartMenu.FullPath(), "Programs");
        string mySMFolder = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenu.StartMenuCurrentUserProgramsPath);
        if (string.IsNullOrWhiteSpace(mySMFolder))
            mySMFolder = Path.Combine(Environment.SpecialFolder.StartMenu.FullPath(), "Programs");
        List<StartMenuItemViewModel> commonSM = GetRootSM(new ShellFolder(commonSMFolder, IntPtr.Zero));
        StartMenuItems.AddRange(commonSM);
        List<StartMenuItemViewModel> MySM = GetRootSM(new ShellFolder(mySMFolder, IntPtr.Zero));
        StartMenuItems.AddRange(MySM);
        StartMenuItems = [.. StartMenuItems.OrderBy(i => i.Name)];
        _startMenuCommonWatcher = new(commonSMFolder)
        {
            IncludeSubdirectories = true,
        };
        _startMenuCommonWatcher.Renamed += StartMenuWatcher_Renamed;
        _startMenuCommonWatcher.Deleted += StartMenuWatcher_Changed;
        _startMenuCommonWatcher.Created += StartMenuWatcher_Changed;
        _startMenuCommonWatcher.Changed += StartMenuWatcher_Changed;
        _startMenuUserWatcher = new(mySMFolder)
        {
            IncludeSubdirectories = true,
        };
        _startMenuUserWatcher.Renamed += StartMenuWatcher_Renamed;
        _startMenuUserWatcher.Deleted += StartMenuWatcher_Changed;
        _startMenuUserWatcher.Created += StartMenuWatcher_Changed;
        _startMenuUserWatcher.Changed += StartMenuWatcher_Changed;
    }

    private void UnregisterStartMenuWatcher(FileSystemWatcher fsw)
    {
        fsw.Renamed -= StartMenuWatcher_Renamed;
        fsw.Deleted -= StartMenuWatcher_Changed;
        fsw.Created -= StartMenuWatcher_Changed;
        fsw.Changed -= StartMenuWatcher_Changed;
    }

    private void StartMenuWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        RefreshPrograms();
    }

    private void StartMenuWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        RefreshPrograms();
    }

    private List<StartMenuItemViewModel> GetRootSM(ShellFolder folder)
    {
        List<StartMenuItemViewModel> ret = [];
        foreach (ShellFile sf in folder.Files)
        {
            StartMenuItemViewModel alreadyExist = StartMenuItems.SingleOrDefault(i => i.Name == sf.DisplayName);
            if (alreadyExist != null)
            {
                if (sf.IsFolder)
                {
                    ShellFolder newShellFolder = new(sf.Path, IntPtr.Zero);
                    if (newShellFolder.Files.Count > 0)
                        alreadyExist.LoadChildren(newShellFolder);
                }
            }
            else
            {
                StartMenuItemViewModel smivm = new(sf, 0, this);
                if (!sf.IsFolder || smivm.Children.Count > 0)
                    StartMenuItems.Add(smivm);
            }
        }

        return ret;
    }

    #endregion

    #region Pinned App

    public void RefreshPinnedShortcut()
    {
        if (_pinnedAppWatcher != null)
        {
            UnregisterPinnedAppWatcher(_pinnedAppWatcher);
            _pinnedAppWatcher.Dispose();
        }
        PinnedShortcut.Clear();
        string path = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenu.StartMenuPinnedShortcutPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file, this));
        _pinnedAppWatcher = new(path)
        {
            IncludeSubdirectories = true,
        };
        _pinnedAppWatcher.Renamed += PinnedAppWatcher_Renamed;
        _pinnedAppWatcher.Deleted += PinnedAppWatcher_Changed;
        _pinnedAppWatcher.Created += PinnedAppWatcher_Changed;
        _pinnedAppWatcher.Changed += PinnedAppWatcher_Changed;
    }

    private void UnregisterPinnedAppWatcher(FileSystemWatcher fsw)
    {
        fsw.Renamed -= PinnedAppWatcher_Renamed;
        fsw.Deleted -= PinnedAppWatcher_Changed;
        fsw.Created -= PinnedAppWatcher_Changed;
        fsw.Changed -= PinnedAppWatcher_Changed;
    }

    private void PinnedAppWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        RefreshPinnedShortcut();
    }

    private void PinnedAppWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        RefreshPinnedShortcut();
    }

    #region PinnedApp2

    public void RefreshPinnedShortcut2()
    {
        if (_pinnedApp2Watcher != null)
        {
            UnregisterPinnedApp2Watcher(_pinnedApp2Watcher);
            _pinnedApp2Watcher.Dispose();
        }
        PinnedShortcut2.Clear();
        string path = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenu.StartMenuPinnedShortcutPath2);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file, this, true));
        _pinnedApp2Watcher = new(path)
        {
            IncludeSubdirectories = true,
        };
        _pinnedApp2Watcher.Renamed += PinnedAppWatcher_Renamed;
        _pinnedApp2Watcher.Deleted += PinnedAppWatcher_Changed;
        _pinnedApp2Watcher.Created += PinnedAppWatcher_Changed;
        _pinnedApp2Watcher.Changed += PinnedAppWatcher_Changed;
    }

    private void UnregisterPinnedApp2Watcher(FileSystemWatcher fsw)
    {
        fsw.Renamed -= PinnedApp2Watcher_Renamed;
        fsw.Deleted -= PinnedApp2Watcher_Changed;
        fsw.Created -= PinnedApp2Watcher_Changed;
        fsw.Changed -= PinnedApp2Watcher_Changed;
    }

    private void PinnedApp2Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        RefreshPinnedShortcut2();
    }

    private void PinnedApp2Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        RefreshPinnedShortcut2();
    }

    #endregion

    #endregion

    public void HideAllContextMenu()
    {
        _cmUser.IsOpen = false;
        _cmStartMenu.IsOpen = false;
        _cmStop.IsOpen = false;
    }

    public double IconSizeWidth
    {
        get { return _iconSizeWidth; }
    }

    public double IconSizeHeight
    {
        get { return _iconSizeHeight; }
    }

    public double IconSizeWidth2
    {
        get { return _iconSizeWidth2; }
    }

    public double IconSizeHeight2
    {
        get { return _iconSizeHeight2; }
    }

    [RelayCommand()]
    private void StartButton()
    {
        ((MenuItem)_cmStartMenu.Items[0]).IsChecked = ShowPanel2;
        ((MenuItem)_cmStartMenu.Items[1]).IsChecked = ShowApplicationsPrograms;
        _cmStartMenu.IsOpen = true;
    }

    private void ChangeShowPanel2()
    {
        ShowPanel2 = !ShowPanel2;
        ConfigManager.StartMenu.StartMenuShowPinnedApp2 = ShowPanel2;
    }

    private void ChangeShowStartPrograms()
    {
        ShowApplicationsPrograms = !ShowApplicationsPrograms;
        ConfigManager.StartMenu.StartMenuShowApplicationsPrograms = ShowApplicationsPrograms;
    }

    private void ShowCustomColor()
    {
        _winColor ??= new CustomColor();
        _winColor.Show();
        _winColor.Activate();
    }

    [RelayCommand()]
    private void ParamButton()
    {
        ShellHelper.ShowConfigPanel();
    }

    [RelayCommand()]
    private void StopButton()
    {
        ((MenuItem)_cmStop.Items[0]).Visibility = IsHybernateEnabled ? Visibility.Visible : Visibility.Collapsed;
        ((MenuItem)_cmStop.Items[1]).Header = IsRestartPending ? Constants.Localization.UPDATE_AND_SHUTDOWN : Constants.Localization.SHUTDOWN;
        ((Image)((MenuItem)_cmStop.Items[1]).Icon).Source = IsRestartPending ? Constants.Icons.UpdateAndShutdown : Constants.Icons.Shutdown;
        ((MenuItem)_cmStop.Items[2]).Header = IsRestartPending ? Constants.Localization.UPDATE_AND_RESTART : Constants.Localization.RESTART;
        ((Image)((MenuItem)_cmStop.Items[2]).Icon).Source = IsRestartPending ? Constants.Icons.UpdateAndRestart : Constants.Icons.Refresh;
        _cmStop.IsOpen = true;
    }

    [RelayCommand()]
    private void UserButton()
    {
        _cmUser.IsOpen = true;
    }

    #region Drag'n Drop

    [RelayCommand()]
    private void Drop(DragEventArgs e)
    {
        if (e.Source is ListView lv)
        {
            if (lv.Name == "FirstPanel")
                DropToPanel(e, ConfigManager.StartMenu.StartMenuPinnedShortcutPath, RefreshPinnedShortcut);
            else if (lv.Name == "SecondPanel")
                DropToPanel(e, ConfigManager.StartMenu.StartMenuPinnedShortcutPath2, RefreshPinnedShortcut2);
        }
    }

    private static void DropToPanel(DragEventArgs e, string path, Action refresh)
    {
        path = Environment.ExpandEnvironmentVariables(path);
        if (e.Data is DataObject data && data.GetDataPresent(DataFormats.FileDrop))
        {
            foreach (string file in data.GetFileDropList())
            {
                string dest = Path.Combine(path, Path.GetFileName(file));
                if (Path.GetExtension(file) == ".lnk")
                {
                    if (!File.Exists(dest))
                        File.Copy(file, dest);
                }
                else
                {
                    Shortcut sc = Shortcut.CreateShortcut(file);
                    dest = Path.Combine(path, Path.GetFileNameWithoutExtension(file) + ".lnk");
                    if (!File.Exists(dest))
                        sc.WriteToFile(dest);
                }
            }
            refresh();
        }
    }

    #endregion

    private static void Leave()
    {
        Application.Current.Dispatcher.Invoke(StartMenuWindow.MyStartMenu.Close);
    }

    #region IDisposable

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _startMenuCommonWatcher?.Dispose();
                _startMenuUserWatcher?.Dispose();
                StartMenuItems.Clear();
                PinnedShortcut.Clear();
                PinnedShortcut2.Clear();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
