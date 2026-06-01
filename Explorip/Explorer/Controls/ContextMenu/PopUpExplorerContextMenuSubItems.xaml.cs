using System.Windows;

using Explorip.Explorer.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using WpfScreenHelper;

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
        Screen screen = Screen.FromPoint(new Point(mousePos.X, mousePos.Y));
        Left = mousePos.X / screen.ScaleFactor;
        Top = (mousePos.Y - 6) / screen.ScaleFactor;
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            Screen screen = Screen.FromWindow(this);
            if ((Top + Height) > screen.WpfWorkingArea.Bottom)
                Top = screen.WpfWorkingArea.Bottom - Height - ConfigManager.ExplorerContextMenuMargin.Top - ConfigManager.ExplorerContextMenuMargin.Bottom;
            if ((Left + Width) > screen.WpfWorkingArea.Right)
                Left = screen.WpfWorkingArea.Right - Width - ConfigManager.ExplorerContextMenuMargin.Right;
        }, System.Windows.Threading.DispatcherPriority.Render);
    }

    private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
            Close();
    }
}
