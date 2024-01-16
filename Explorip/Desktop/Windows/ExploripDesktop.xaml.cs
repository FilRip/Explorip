using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

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
}
