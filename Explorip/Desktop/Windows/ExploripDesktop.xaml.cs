using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Explorip.Desktop.Controls;
using Explorip.Desktop.ViewModels;
using Explorip.Helpers;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;

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
        DataContext = new ExploripDesktopViewModel(this);
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(GetHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
    }

    internal void RefreshGrid()
    {
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.RowDefinitions.Clear();

        MyDataContext.NbColumns = (int)AssociateScreen.WorkingArea.Width / (int)(Constants.Desktop.ITEM_SIZE_X.Value * AssociateScreen.ScaleFactor);
        MyDataContext.NbRows = (int)AssociateScreen.WorkingArea.Height / (int)(Constants.Desktop.ITEM_SIZE_Y.Value * AssociateScreen.ScaleFactor);

        for (int i = 0; i < MyDataContext.NbColumns; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = Constants.Desktop.ITEM_SIZE_X,
            });

        for (int i = 0; i < MyDataContext.NbRows; i++)
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
            if (nbRow == MyDataContext.NbRows)
            {
                nbRow = 0;
                nbColumn++;
                if (nbColumn == MyDataContext.NbColumns)
                    break;
            }
            findEmptyPlace = !MainGrid.Children.OfType<OneDesktopItem>().Any(c => Grid.GetRow(c) == nbRow && Grid.GetColumn(c) == nbColumn);
        }
        if (findEmptyPlace)
        {
            if (!MainGrid.Children.Contains(item))
                MainGrid.Children.Add(item);
            Grid.SetColumn(item, nbColumn);
            Grid.SetRow(item, nbRow);
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
        {
            _handle = new WindowInteropHelper(this).EnsureHandle();
            HwndSource.FromHwnd(_handle).AddHook(WndProc);
        }
        return _handle;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == (int)NativeMethods.WM.WINDOWPOSCHANGING)
        {
            NativeMethods.WindowPos wndPos = NativeMethods.WindowPos.FromMessage(lParam);

            if (!wndPos.flags.HasFlag(NativeMethods.SWP.SWP_NOZORDER))
            {
                IntPtr lowestHwnd = WindowHelper.GetLowestDesktopParentHwnd();

                if (lowestHwnd != IntPtr.Zero)
                    wndPos.hwndInsertAfter = NativeMethods.GetWindow(lowestHwnd, NativeMethods.GetWindow_Cmd.GW_HWNDPREV);
                else
                    wndPos.hwndInsertAfter = (IntPtr)NativeMethods.WindowZOrder.HWND_BOTTOM;

                wndPos.UpdateMessage(lParam);
            }
        }
        else if ((msg == (int)NativeMethods.WM.SETTINGCHANGE && wParam.ToInt32() == (int)NativeMethods.SPI.SETWORKAREA) ||
            (msg == (int)NativeMethods.WM.DPICHANGED))
        {
            RefreshGrid();
            foreach (OneDesktopItem item in MyDataContext.ListItems())
                AddItem(item);
        }

        return IntPtr.Zero;
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
        MyDataContext.ActionRightClickCommand.Execute(e);
    }

    #region Rectangle selection

    private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        if (_selection && SelectInRectangle())
            return;

        if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneDesktopItemViewModel)
            return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            return;
        MyDataContext.UnSelectAll();
    }

    private bool _selection;
    private Point _selectionStart;
    private readonly Pen _selectionPen = new(Constants.Colors.SelectedBackgroundShellObject, 2);

    private bool SelectInRectangle()
    {
        _selection = false;
        InvalidateVisual();
        Rect rect = new(PointToScreen(_selectionStart), PointToScreen(Mouse.GetPosition(this)));
        if (rect.Width == 0 && rect.Height == 0)
            return false;
        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            MyDataContext.UnSelectAll();
        foreach (OneDesktopItem item in MyDataContext.ListItems().Where(i => rect.IntersectsWith(i.GetAbsoluteRectangle())))
            item.MyDataContext.IsSelected = true;
        return true;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (_selection)
            drawingContext.DrawRectangle(Constants.Colors.SelectedBackgroundShellObject, _selectionPen, new Rect(_selectionStart, Mouse.GetPosition(this)));
    }

    private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_selection)
            InvalidateVisual();
        else if (_isDragging)
        {
            Point diffPos = e.GetPosition(this).GetDelta(_startDragging);
            if (diffPos.X > 16 || diffPos.Y > 16)
            {
                _isDragging = false;
                MyDataContext.StartDrag((OneDesktopItemViewModel)((FrameworkElement)Mouse.DirectlyOver).DataContext);
            }
        }
    }

    private bool _isDragging;
    private Point _startDragging;
    private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ((Mouse.DirectlyOver is not FrameworkElement element || element.DataContext is not OneDesktopItemViewModel))
        {
            if (!_selection)
            {
                _selection = true;
                _selectionStart = e.GetPosition(this);
            }
        }
        else
        {
            _isDragging = true;
            _startDragging = e.GetPosition(this);
        }
    }

    private void Window_MouseLeave(object sender, MouseEventArgs e)
    {
        if (_selection)
        {
            _selection = false;
            InvalidateVisual();
        }
        _isDragging = false;
    }

    #endregion

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        MyDataContext.ActionOnKeyCommand.Execute(e);
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        MyDataContext.Drop(e);
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        MyDataContext.DragOver(e);
    }
}
