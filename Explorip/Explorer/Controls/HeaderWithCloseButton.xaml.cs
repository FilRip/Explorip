using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Explorip.Explorer.Windows;

using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.KnownFolders;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Interaction logic for CloseableItem.xaml
/// </summary>
public partial class HeaderWithCloseButton : UserControl, INotifyPropertyChanged
{
    public HeaderWithCloseButton()
    {
        DataContext = this;
        InitializeComponent();
    }

    private TabExplorerBrowser MyTabControl
    {
        get { return (TabExplorerBrowser)MyTabItem.Parent; }
    }

    private TabItem MyTabItem
    {
        get { return (TabItem)Parent; }
    }

    private bool _plusButton;
    public bool PlusButton
    {
        get { return _plusButton; }
        set
        {
            _plusButton = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName()] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Context menu

    private TabItemExplorerBrowser CurrentTabExplorer
    {
        get
        {
            if (MyTabControl.Items.Count == 0)
                return null;
            return MyTabControl.SelectedItem as TabItemExplorerBrowser;
        }
    }

    private void NewTab_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentTabExplorer == null)
            MyTabControl.AddNewTab((ShellObject)KnownFolders.Desktop);
        else
            MyTabControl.AddNewTab(CurrentTabExplorer.ExplorerBrowser.NavigationLog.CurrentLocation);
    }

    private void NewRegistryEditorTab_Click(object sender, RoutedEventArgs e)
    {
        MyTabControl.AddNewTab(new TabItemRegedit());
    }

    private void CloseTab_Click(object sender, RoutedEventArgs e)
    {
        TabExplorerBrowser myTabControl = MyTabControl;
        if (MyTabControl.Items.Count == 2 && !MyTabControl.AllowCloseLastTab)
            return;
        ((TabItemExplorip)MyTabControl.SelectedItem).Dispose();
        myTabControl.HideTab();
    }

    private void CloseAllTab_Click(object sender, RoutedEventArgs e)
    {
        TabExplorerBrowser myTabControl = MyTabControl;
        for (int i = myTabControl.Items.Count - 1; i >= 0; i--)
            if (myTabControl.Items[i] is TabItemExplorip currentTab && (myTabControl.SelectedItem != myTabControl.Items[i] || myTabControl.AllowCloseLastTab))
                currentTab.Dispose();
        myTabControl.HideTab();
    }

    private void NewTabOther_Click(object sender, RoutedEventArgs e)
    {
        WpfExplorerBrowser window = (WpfExplorerBrowser)Window.GetWindow(this);
        ShellObject dir = CurrentTabExplorer?.ExplorerBrowser?.NavigationLog?.CurrentLocation ?? (ShellObject)KnownFolders.Desktop;
        if (window.LeftTab == MyTabControl)
            window.RightTab.AddNewTab(dir);
        else
            window.LeftTab.AddNewTab(dir);
    }

    #endregion

    private void ButtonClose_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (PlusButton)
            ButtonNewTab.Foreground = Brushes.Lime;
        else
            ButtonClose.Foreground = Brushes.Red;
    }

    private void ButtonClose_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (PlusButton)
            ButtonNewTab.Foreground = Brushes.White;
        else
            ButtonClose.Foreground = Brushes.Black;
    }

    private void ButtonClose_Click(object sender, RoutedEventArgs e)
    {
        if (PlusButton)
        {
            MyTabControl.AddNewTab((ShellObject)KnownFolders.Desktop);
            e.Handled = true;
        }
        else
        {
            if (MyTabControl.Items.Count == 2 && !MyTabControl.AllowCloseLastTab)
                return;
            TabExplorerBrowser previousTabControl = MyTabControl;
            if (MyTabItem is TabItemExplorip tab)
                tab.Dispose();
            previousTabControl.HideTab();
        }
    }

    #region Console contextual menu

    private void NewConsoleTab_Click(object sender, RoutedEventArgs e)
    {
        MyTabControl.Items.Insert(MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "cmd.exe" }));
        MyTabControl.SelectedIndex = MyTabControl.Items.Count - 2;
    }

    private void NewPowerShellTab_Click(object sender, RoutedEventArgs e)
    {
        MyTabControl.Items.Insert(MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "powershell.exe" }));
        MyTabControl.SelectedIndex = MyTabControl.Items.Count - 2;
    }

    private void NewAdminConsoleTab_Click(object sender, RoutedEventArgs e)
    {
        MyTabControl.Items.Insert(MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "cmd.exe", Verb = "runas", UseShellExecute = true }));
        MyTabControl.SelectedIndex = MyTabControl.Items.Count - 2;
    }

    private void NewAdminPowerShellTab_Click(object sender, RoutedEventArgs e)
    {
        MyTabControl.Items.Insert(MyTabControl.Items.Count - 1, new TabItemConsoleCommand(new ProcessStartInfo() { FileName = "conhost.exe", Arguments = "powershell.exe", Verb = "runas", UseShellExecute = true }));
        MyTabControl.SelectedIndex = MyTabControl.Items.Count - 2;
    }

    #endregion
}
