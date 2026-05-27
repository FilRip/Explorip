using System.Windows.Controls.Primitives;

using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour PopUpExplorerContextMenu.xaml
/// </summary>
public partial class PopUpExplorerContextMenuSubItems : Popup
{
    public PopUpExplorerContextMenuSubItems()
    {
        InitializeComponent();
    }

    public new ContextMenuEntryViewModel DataContext
    {
        get { return (ContextMenuEntryViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void Popup_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //DataContext.Closing();
    }

    private void Popup_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //DataContext.Closing();
    }
}
