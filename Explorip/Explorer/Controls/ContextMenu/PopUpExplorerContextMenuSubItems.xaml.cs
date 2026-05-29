using System.Windows;

using Explorip.Explorer.ViewModels;

using ManagedShell.Interop;

namespace Explorip.Explorer.Controls.ContextMenu;

/// <summary>
/// Logique d'interaction pour PopUpExplorerContextMenu.xaml
/// </summary>
public partial class PopUpExplorerContextMenuSubItems : Window
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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        System.Drawing.Point mousePos = new();
        NativeMethods.GetCursorPos(ref mousePos);
        Left = mousePos.X;
        Top = mousePos.Y;
    }
}
