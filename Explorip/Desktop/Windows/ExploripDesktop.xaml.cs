using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.Helpers;

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
        System.Drawing.Point position = new((int)Mouse.GetPosition(this).X, (int)Mouse.GetPosition(this).Y);
        contextMenu.ShowContextMenu(new DirectoryInfo[1] { new(Environment.SpecialFolder.Desktop.FullPath()) }, position);
    }
}
