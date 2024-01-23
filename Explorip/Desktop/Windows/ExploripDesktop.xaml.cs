using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.Desktop.Models;
using Explorip.Desktop.ViewModels;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell.Common.Helpers;

using WpfScreenHelper;

namespace Explorip.Desktop.Windows;

/// <summary>
/// Logique d'interaction pour ExploripDesktop.xaml
/// </summary>
public partial class ExploripDesktop : Window
{
    private IntPtr _handle;

    public ExploripDesktop()
    {
        InitializeComponent();
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(new WindowInteropHelper(this).EnsureHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    internal Screen AssociateScreen { get; set; }

    internal ExploripDesktopViewModel MyDataContext
    {
        get { return (ExploripDesktopViewModel)DataContext; }
    }

    internal IntPtr GetHandle()
    {
        if (_handle == IntPtr.Zero)
            _handle = new WindowInteropHelper(this).EnsureHandle();
        return _handle;
    }

    internal void InitDesktopWindow()
    {
        Show();
        this.SetWindowPosition((int)AssociateScreen.WorkingArea.X, (int)AssociateScreen.WorkingArea.Y, (int)AssociateScreen.WorkingArea.Width, (int)AssociateScreen.WorkingArea.Height);
        WindowHelper.HideWindowFromTasks(GetHandle());
        WindowHelper.ShowWindowDesktop(GetHandle());
    }

    private void Window_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneDesktopShellItem item)
        {
            if (!item.IsSelected)
            {
                MyDataContext.UnselectAll();
                item.IsSelected = true;
            }
        }
        else
            MyDataContext.UnselectAll();

        ManagedShell.ShellFolders.Models.ShellContextMenu contextMenu = new();
        Point position = PointToScreen(Mouse.GetPosition(this));
        contextMenu.ShowContextMenu(MyDataContext.ListSelectedItem().ToArray(), position);
    }

    private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneDesktopShellItem)
            return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            return;
        MyDataContext.UnselectAll();
    }
}
