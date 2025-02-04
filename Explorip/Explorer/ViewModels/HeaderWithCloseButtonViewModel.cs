using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.Explorer.Windows;

using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

namespace Explorip.Explorer.ViewModels;

public partial class HeaderWithCloseButtonViewModel(HeaderWithCloseButton control) : ObservableObject()
{
    #region Fields

    private readonly HeaderWithCloseButton _control = control;

    #endregion

    #region Fields Property

    [ObservableProperty()]
    private string _title;
    [ObservableProperty()]
    private bool _plusButton;

    #endregion

    #region Context menu commands

    [RelayCommand()]
    private void NewTab()
    {
        if (_control.CurrentTabExplorer == null)
            _control.MyTabControl.AddNewTab((ShellObject)KnownFolders.Desktop);
        else
            _control.MyTabControl.AddNewTab(_control.CurrentTabExplorer.ExplorerBrowser.NavigationLog.CurrentLocation);
    }

    [RelayCommand()]
    private void NewRegistryEditorTab()
    {
        _control.MyTabControl.AddNewTab(new TabItemRegedit());
    }

    [RelayCommand()]
    private void CloseTab()
    {
        TabExplorerBrowser myTabControl = _control.MyTabControl;
        if (_control.MyTabControl.Items.Count == 2 && !_control.MyTabControl.AllowCloseLastTab)
            return;
        ((TabItemExplorip)_control.MyTabControl.SelectedItem).Dispose();
        myTabControl.HideTab();
    }

    [RelayCommand()]
    private void CloseAllTab()
    {
        TabExplorerBrowser myTabControl = _control.MyTabControl;
        for (int i = myTabControl.Items.Count - 1; i >= 0; i--)
            if (myTabControl.Items[i] is TabItemExplorip currentTab && (myTabControl.SelectedItem != myTabControl.Items[i] || myTabControl.AllowCloseLastTab))
                currentTab.Dispose();
        myTabControl.HideTab();
    }

    [RelayCommand()]
    private void NewTabOther()
    {
        WpfExplorerBrowser window = (WpfExplorerBrowser)System.Windows.Window.GetWindow(_control);
        ShellObject dir = _control.CurrentTabExplorer?.ExplorerBrowser?.NavigationLog?.CurrentLocation ?? (ShellObject)KnownFolders.Desktop;
        if (window.LeftTab == _control.MyTabControl)
            window.RightTab.AddNewTab(dir);
        else
            window.LeftTab.AddNewTab(dir);
    }

    [RelayCommand()]
    private void NewConsoleTab()
    {
        _control.MyTabControl.Items.Insert(_control.MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "cmd.exe" }));
        _control.MyTabControl.SelectedIndex = _control.MyTabControl.Items.Count - 2;
    }

    [RelayCommand()]
    private void NewPowerShellTab()
    {
        _control.MyTabControl.Items.Insert(_control.MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "powershell.exe" }));
        _control.MyTabControl.SelectedIndex = _control.MyTabControl.Items.Count - 2;
    }

    [RelayCommand()]
    private void NewAdminConsoleTab()
    {
        _control.MyTabControl.Items.Insert(_control.MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "cmd.exe", Verb = "runas", UseShellExecute = true }));
        _control.MyTabControl.SelectedIndex = _control.MyTabControl.Items.Count - 2;
    }

    [RelayCommand()]
    private void NewAdminPowerShellTab()
    {
        _control.MyTabControl.Items.Insert(_control.MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "powershell.exe", Verb = "runas", UseShellExecute = true }));
        _control.MyTabControl.SelectedIndex = _control.MyTabControl.Items.Count - 2;
    }

    [RelayCommand()]
    private void NewEmbeddedWindowTab()
    {
        _control.MyTabControl.AddNewTab(new TabItemWindowEmbedded());
    }

    #endregion
}
