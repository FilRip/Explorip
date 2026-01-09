using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Explorip.Explorer.Windows;

using ExploripConfig.Configuration;

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabExplorerBrowser.xaml
/// </summary>
public partial class TabExplorerBrowser : TabControl
{
    public TabExplorerBrowser()
    {
        DataContext = this;
        InitializeComponent();
    }

    public bool AllowCloseLastTab { get; set; }

    #region tabcontrol when add/remove tabitem

    public void HideTab()
    {
        if (MyTabControl.Items.Count == 1)
        {
            MyTabControl.Visibility = Visibility.Collapsed;
            WpfExplorerBrowser window = (WpfExplorerBrowser)Window.GetWindow(this);
            window.HideRightTab();
        }
    }

    public void CloseAllTabs()
    {
        if (MyTabControl.Items.Count > 0)
            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
                if (MyTabControl.Items[i] is TabItemExplorip tabItem)
                    tabItem.Dispose();
    }

    public void AddNewTab(ShellObject location, string title = "")
    {
        TabItemExplorerBrowser item = new();
        item.SetTempTitle(title);
        item.Navigation(location);
        Items.Insert(Items.Count - 1, item);
        WpfExplorerBrowser window = (WpfExplorerBrowser)Window.GetWindow(this);
        if (window.RightTab == MyTabControl && MyTabControl.Items.Count > 1)
            window.ShowRightTab();
        SelectedItem = item;
    }

    public void AddNewTab(string path)
    {
        AddNewTab(ShellObject.FromParsingName(path));
    }

    public void AddNewTab(TabItemExplorip tab)
    {
        Items.Insert(Items.Count - 1, tab);
        SelectedItem = tab;
    }

    #endregion

    #region Properties

    public TabItemExplorip CurrentTab
    {
        get { return (TabItemExplorip)SelectedItem; }
    }

    public TabItemExplorerBrowser CurrentTabExplorer
    {
        get { return (TabItemExplorerBrowser)SelectedItem; }
    }

    public WpfExplorerBrowser MyWindow
    {
        get { return (WpfExplorerBrowser)Window.GetWindow(this); }
    }

    #endregion

    #region Shortcuts

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Key == Key.F4 && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            if (Items.Count > 2 || AllowCloseLastTab)
            {
                Items.Remove(SelectedItem);
                HideTab();
            }
        }
        else if (e.Key == Key.N && e.KeyboardDevice.Modifiers == ModifierKeys.Control && CurrentTab is TabItemExplorerBrowser tieb)
        {
            AddNewTab(tieb.CurrentDirectory);
        }
        else if (e.Key == Key.Right && e.KeyboardDevice.Modifiers == ModifierKeys.Control &&
            MyWindow.MyDataContext.SelectionLeft)
        {
            MyWindow.CopyLeft.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        else if (e.Key == Key.Left && e.KeyboardDevice.Modifiers == ModifierKeys.Control &&
            MyWindow.MyDataContext.SelectionRight)
        {
            MyWindow.CopyRight.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        base.OnKeyUp(e);
    }

    #endregion

    private void MyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItem is TabItemPlusExplorerBrowser && SelectedIndex > 0)
        {
            SelectedIndex--;
            e.Handled = true;
        }
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is TabItemExplorip tabSelecting)
            tabSelecting.RaiseOnSelecting();
        if (e.RemovedItems?.Count > 0 && e.RemovedItems[0] is TabItemExplorip tabDeselecting)
            tabDeselecting.RaiseOnDeSelecting();
    }

    public Vector GetVisualOffset()
    {
        return VisualOffset;
    }

    private void TabControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged && Name == "LeftTab" && ConfigManager.AllowWrite)
        {
            RegistryKey registryKey = ConfigManager.MyRegistryKey.CreateSubKey("LeftTab");
            registryKey.SetValue("Width", e.NewSize.Width.ToString());
        }
    }
}
