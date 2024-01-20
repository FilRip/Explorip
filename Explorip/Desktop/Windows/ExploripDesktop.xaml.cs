using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.Helpers;

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

    public Screen AssociateScreen { get; set; }

    public IntPtr GetHandle()
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
        ManagedShell.ShellFolders.Models.ShellContextMenu contextMenu = new();
        Point position = PointToScreen(Mouse.GetPosition(this));
        contextMenu.ShowContextMenu(new DirectoryInfo[1] { new(Environment.SpecialFolder.Desktop.FullPath()) }, position);
    }
}
