using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using Explorip.Desktop.Controls;
using Explorip.Desktop.ViewModels;
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
    private int _nbColumns;
    private int _nbRows;

    public ExploripDesktop()
    {
        InitializeComponent();
        DataContext = new ExploripDesktopViewModel(this);
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(GetHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    internal void RefreshGrid()
    {
        _nbColumns = (int)AssociateScreen.WorkingArea.Width / (int)(Constants.Desktop.ITEM_SIZE_X.Value * AssociateScreen.ScaleFactor);
        _nbRows = (int)AssociateScreen.WorkingArea.Height / (int)(Constants.Desktop.ITEM_SIZE_Y.Value * AssociateScreen.ScaleFactor);

        for (int i = 0; i < _nbColumns; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = Constants.Desktop.ITEM_SIZE_X,
            });

        for (int i = 0; i < _nbRows; i++)
            MainGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = Constants.Desktop.ITEM_SIZE_Y,
            });
    }

    internal void AddItem(OneDesktopItem item)
    {
        int nbColumn = 0, nbRow = -1;
        bool findEmptyPlace = false;
        while (!findEmptyPlace)
        {
            nbRow++;
            if (nbRow == _nbRows)
            {
                nbRow = 0;
                nbColumn++;
                if (nbColumn == _nbColumns)
                    break;
            }
            findEmptyPlace = !MainGrid.Children.OfType<OneDesktopItem>().Any(c => Grid.GetRow(c) == nbRow && Grid.GetColumn(c) == nbColumn);
        }
        if (findEmptyPlace)
            MainGrid.Children.Add(item);
        Grid.SetColumn(item, nbColumn);
        Grid.SetRow(item, nbRow);
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
        MyDataContext.RefreshDesktopContent();
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
        FileSystemInfo[] listItems = MyDataContext.ListSelectedItem();
        if (listItems.Length == 0)
            listItems = listItems.Add(new DirectoryInfo(Environment.SpecialFolder.DesktopDirectory.FullPath()));
        contextMenu.ShowContextMenu(listItems, position);
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
