using System.Windows;

using Explorip.Explorer.Controls.Tabs;
using Explorip.Explorer.ViewModels;

using ExploripConfig.Configuration;

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
        Top = (mousePos.Y - 6) / screen.ScaleFactor;
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Screen screen = Screen.FromWindow(this);
        if ((Top + Height) > screen.WpfWorkingArea.Bottom)
            Top = screen.WpfWorkingArea.Bottom - Height - ConfigManager.ExplorerContextMenuMargin.Top * screen.ScaleFactor * 2 - ConfigManager.ExplorerContextMenuCornerRadius.TopLeft * screen.ScaleFactor * 3;
        if ((Left + Width) > screen.WpfWorkingArea.Right)
            Left = screen.WpfWorkingArea.Right - Width - ConfigManager.ExplorerContextMenuMargin.Right * screen.ScaleFactor;
    }

    private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
            Close();
    }
}
