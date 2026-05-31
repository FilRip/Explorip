using System.Windows;

using Explorip.Explorer.Controls.Tabs;
using Explorip.Explorer.ViewModels;

using ManagedShell.Interop;

using WpfScreenHelper;

namespace Explorip.Explorer.Controls.ContextMenu;

/// <summary>
/// Logique d'interaction pour PopUpExplorerContextMenu.xaml
/// </summary>
public partial class PopUpExplorerContextMenu : Window
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
            Owner = GetWindow(_parentTab);
            DataContext.SetParentTab(_parentTab);
            DataContext.Close = Close;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        System.Drawing.Point mousePos = new();
        NativeMethods.GetCursorPos(ref mousePos);
        Screen screen = Screen.FromPoint(new Point(mousePos.X, mousePos.Y));
        Left = mousePos.X / screen.ScaleFactor;
        Top = mousePos.Y / screen.ScaleFactor;
    }
}
