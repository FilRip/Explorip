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
using Explorip.TaskBar;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;

using Microsoft.Win32;

using Securify.ShellLink;

namespace Explorip.StartMenu.ViewModels;

public partial class StartMenuViewModel : ObservableObject
{
    private readonly double _iconSizeWidth, _iconSizeHeight, _iconSizeWidth2, _iconSizeHeight2;
    private readonly ContextMenu _cmUser, _cmStop, _cmStartMenu;
    private CustomColor _winColor;

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

        _cmUser = new()
        {
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
        };
        CreateMenuItem(_cmUser, Constants.Localization.LOCK, ShellHelper.Lock, icon: Constants.Icons.Lock);
        CreateMenuItem(_cmUser, Constants.Localization.DISCONNECT, ShellHelper.Logoff);

        _cmStop = new()
        {
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
            Margin = new Thickness(0),
        };
        CreateMenuItem(_cmStop, Constants.Localization.PUT_HYBERNATE, Hybernate);
        CreateMenuItem(_cmStop, Constants.Localization.SHUTDOWN, Shutdown, icon: Constants.Icons.Shutdown);
        CreateMenuItem(_cmStop, Constants.Localization.RESTART, Restart, icon: Constants.Icons.Refresh);

        #endregion

        _iconSizeHeight = ConfigManager.StartMenuIconSizeHeight;
        _iconSizeWidth = ConfigManager.StartMenuIconSizeWidth;
        _iconSizeHeight2 = ConfigManager.StartMenuIconSizeHeight2;
        _iconSizeWidth2 = ConfigManager.StartMenuIconSizeWidth2;

        _showPanel2 = ConfigManager.StartMenuShowPinnedApp2;
        _showApplicationsPrograms = ConfigManager.ShowApplicationsPrograms;

        _height = ConfigManager.StartMenuHeight;

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

    private static bool IsPendingReboot()
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing");
        if (key != null && key.GetValue("RebootPending") != null)
        {
            return true;
        }
        return false;
    }

    private static bool IsWUPendingReboot()
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired");
        if (key != null)
            return true;
        return false;
    }

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

    public void RefreshAll()
    {
        RefreshPrograms();
        RefreshPinnedShortcut();
        RefreshPinnedShortcut2();
    }

    public void RefreshPrograms()
    {
        StartMenuItems = [];
        string commonSMFolder = Environment.ExpandEnvironmentVariables(ConfigManager.CommonProgramsPath);
        if (string.IsNullOrWhiteSpace(commonSMFolder))
            commonSMFolder = Path.Combine(Environment.SpecialFolder.CommonStartMenu.FullPath(), "Programs");
        string mySMFolder = Environment.ExpandEnvironmentVariables(ConfigManager.CurrentUserProgramsPath);
        if (string.IsNullOrWhiteSpace(mySMFolder))
            mySMFolder = Path.Combine(Environment.SpecialFolder.StartMenu.FullPath(), "Programs");
        List<StartMenuItemViewModel> commonSM = GetRootSM(new ShellFolder(commonSMFolder, IntPtr.Zero));
        StartMenuItems.AddRange(commonSM);
        List<StartMenuItemViewModel> MySM = GetRootSM(new ShellFolder(mySMFolder, IntPtr.Zero));
        StartMenuItems.AddRange(MySM);
        StartMenuItems = [.. StartMenuItems.OrderBy(i => i.Name)];
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

    public void RefreshPinnedShortcut()
    {
        PinnedShortcut = [];
        string path = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenuPinnedShortcutPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file, this));
    }

    public void RefreshPinnedShortcut2()
    {
        PinnedShortcut2 = [];
        string path = Environment.ExpandEnvironmentVariables(ConfigManager.StartMenuPinnedShortcutPath2);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        ShellFolder sf = new(path, IntPtr.Zero);
        foreach (ShellFile file in sf.Files)
            PinnedShortcut.Add(new PinnedShortutViewModel(file, this, true));
    }

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
        ConfigManager.StartMenuShowPinnedApp2 = ShowPanel2;
    }

    private void ChangeShowStartPrograms()
    {
        ShowApplicationsPrograms = !ShowApplicationsPrograms;
        ConfigManager.ShowApplicationsPrograms = ShowApplicationsPrograms;
    }

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

    [RelayCommand()]
    private void Drop(DragEventArgs e)
    {
        if (e.Source is ListView lv)
        {
            if (lv.Name == "FirstPanel")
                DropToPanel(e, ConfigManager.StartMenuPinnedShortcutPath, RefreshPinnedShortcut);
            else
                DropToPanel(e, ConfigManager.StartMenuPinnedShortcutPath2, RefreshPinnedShortcut2);
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
}
