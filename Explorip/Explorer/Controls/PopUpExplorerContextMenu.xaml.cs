using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Explorer.Controls.Tabs;
using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour PopUpExplorerContextMenu.xaml
/// </summary>
public partial class PopUpExplorerContextMenu : ContextMenu
{
    private TabItemExplorerBrowser _parentTab;

    public PopUpExplorerContextMenu()
    {
        InitializeComponent();
    }

    public new PopUpExplorerContextMenuViewModel DataContext
    {
        get { return (PopUpExplorerContextMenuViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    public TabItemExplorerBrowser ParentTab
    {
        get { return _parentTab; }
        set
        {
            _parentTab = value;
            DataContext.SetParentTab(_parentTab);
        }
    }

    private void Popup_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataContext.Closing();
    }

    private void Popup_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataContext.Closing();
    }
}
