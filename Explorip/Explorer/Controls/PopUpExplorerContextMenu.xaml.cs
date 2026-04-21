using System.Windows.Controls.Primitives;

using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour PopUpExplorerContextMenu.xaml
/// </summary>
public partial class PopUpExplorerContextMenu : Popup
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
}
