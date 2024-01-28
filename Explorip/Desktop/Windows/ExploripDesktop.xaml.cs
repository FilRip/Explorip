using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.Desktop.Controls;
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
    private const int NB_COLUMNS = 20;
    private const int NB_ROWS = 10;

    public ExploripDesktop()
    {
        InitializeComponent();
        DataContext = new ExploripDesktopViewModel();
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(GetHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    internal void RefreshGrid()
    {
        GridLength gridWidth, gridHeight;

        gridWidth = new GridLength(AssociateScreen.WorkingArea.Width / NB_COLUMNS, GridUnitType.Pixel);
        gridHeight = new GridLength(AssociateScreen.WorkingArea.Height / NB_ROWS, GridUnitType.Pixel);

        for (int i = 0; i < NB_COLUMNS; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = gridWidth,
            });

        for (int i = 0; i < NB_ROWS; i++)
            MainGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = gridHeight,
            });
    }

    internal void AddObject(OneDesktopItem item)
    {
        // TODO : Search grid cell to add item on it
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
        RefreshGrid();
        Show();
        this.SetWindowPosition((int)AssociateScreen.WorkingArea.X, (int)AssociateScreen.WorkingArea.Y, (int)AssociateScreen.WorkingArea.Width, (int)AssociateScreen.WorkingArea.Height);
        WindowHelper.HideWindowFromTasks(GetHandle());
        WindowHelper.ShowWindowDesktop(GetHandle());
    }

    private void Window_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneDesktopItemViewModel item)
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
        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneDesktopItemViewModel)
            return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            return;
        MyDataContext.UnselectAll();
    }
}
