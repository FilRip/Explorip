using System.Windows;

using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Logging;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for Toolbar.xaml
/// </summary>
public partial class Toolbar : BaseToolbar
{
    public Toolbar() : base()
    {
        InitializeComponent();
    }

    public new ToolbarViewModel DataContext
    {
        get { return (ToolbarViewModel)base.DataContext; }
        set { base.DataContext = value; }
    }

    #region Events

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded || !_ignoreReload)
        {
            ShellLogger.Debug("OnLoaded on Toolbar " + DataContext.Id + " on screen : " + ((Taskbar)Window.GetWindow(this)).NumScreen.ToString());
            _isLoaded = true;
            DataContext.Init(this);
        }
    }

    #endregion

    private void BaseToolbar_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_ignoreReload)
            return;

        ShellLogger.Debug("OnUnloaded on Toolbar " + DataContext.Id + " on screen : " + DataContext.ParentTaskbar.NumScreen.ToString());
        _isLoaded = false;
        DataContext.UnloadFolder();
    }
}
