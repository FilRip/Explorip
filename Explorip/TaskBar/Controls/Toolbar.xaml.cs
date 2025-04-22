using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.TaskBar.ViewModels;

using ManagedShell.ShellFolders;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for Toolbar.xaml
/// </summary>
public partial class Toolbar : UserControl
{
    private bool _isLoaded;

    public Toolbar()
    {
        InitializeComponent();
    }

    public ToolbarViewModel MyDataContext
    {
        get { return (ToolbarViewModel)DataContext; }
    }

    #region Events

    private void ToolbarIcon_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ToolbarButton icon)
            return;

        Mouse.Capture(null);

        if (icon.DataContext is not ShellFile file || string.IsNullOrWhiteSpace(file.Path))
            return;

        if (MyDataContext.InvokeContextMenu(file, false))
            e.Handled = true;
    }

    private void ToolbarIcon_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ToolbarButton icon)
            return;

        ShellFile file = icon.DataContext as ShellFile;

        if (MyDataContext.InvokeContextMenu(file, true))
            e.Handled = true;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            MyDataContext.Init(this);
        }
    }

    #endregion
}
