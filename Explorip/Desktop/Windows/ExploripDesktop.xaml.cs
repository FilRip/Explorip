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

using ExploripConfig.Configuration;

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
    public DesktopConfig MyDesktopConfig { get; private set; }

    public ExploripDesktop() : this(Screen.PrimaryScreen) { }

    public ExploripDesktop(Screen screen)
    {
        InitializeComponent();
        AssociateScreen = screen;
        base.DataContext = new ExploripDesktopViewModel(this);
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            WindowsSettings.UseImmersiveDarkMode(GetHandle(), true);
            Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
        }
        MyDesktopConfig = ConfigManager.GetDesktopConfig(ScreenId);
        if (!MyDesktopConfig.HideBackground)
            Background = MyDesktopConfig.DesktopBackground;

        HwndSource.FromHwnd(new WindowInteropHelper(this).EnsureHandle()).AddHook(WndProc);
    }

    internal void RefreshGrid()
    {
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.RowDefinitions.Clear();

        DataContext.NbColumns = (int)AssociateScreen.WorkingArea.Width / (int)(MyDesktopConfig.ItemSizeX * AssociateScreen.ScaleFactor);
        DataContext.NbRows = (int)AssociateScreen.WorkingArea.Height / (int)(MyDesktopConfig.ItemSizeY * AssociateScreen.ScaleFactor);

        for (int i = 0; i < DataContext.NbColumns; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(MyDesktopConfig.ItemSizeX, GridUnitType.Pixel),
            });

        for (int i = 0; i < DataContext.NbRows; i++)
            MainGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(MyDesktopConfig.ItemSizeY, GridUnitType.Pixel),
            });
    }

    internal void AddItem(OneDesktopItem item)
    {
        int nbColumn = 0, nbRow = -1;
        bool findEmptyPlace = false;
        while (!findEmptyPlace)
        {
            nbRow++;
            if (nbRow == DataContext.NbRows)
            {
                nbRow = 0;
                nbColumn++;
                if (nbColumn == DataContext.NbColumns)
                    break;
            }
            findEmptyPlace = !MainGrid.Children.OfType<OneDesktopItem>().Any(c => Grid.GetRow(c) == nbRow && Grid.GetColumn(c) == nbColumn);
        }
        if (findEmptyPlace)
        {
            if (!MainGrid.Children.Contains(item))
                MainGrid.Children.Add(item);
            (int, int) position = MyDesktopConfig.GetItemPosition(item.DataContext.Name);
            if (position.Item1 >= 0 && position.Item2 >= 0)
            {
                nbColumn = position.Item1;
                nbRow = position.Item2;
            }
            Grid.SetColumn(item, nbColumn);
            Grid.SetRow(item, nbRow);
        }
    }

    internal Screen AssociateScreen { get; set; }
    private int ScreenId
    {
        get { return AssociateScreen.DisplayNumber; }
    }

    internal new ExploripDesktopViewModel DataContext
    {
        get { return (ExploripDesktopViewModel)base.DataContext; }
        set { base.DataContext = value; }
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
        if (msg == (int)NativeMethods.WM.SYSCOMMAND)
            handled = true;
        else if (msg == (int)NativeMethods.WM.WINDOWPOSCHANGING)
        {
            NativeMethods.WindowPos wndPos = NativeMethods.WindowPos.FromMessage(lParam);

            if (!wndPos.flags.HasFlag(NativeMethods.EShowWindowPos.SWP_NOZORDER))
            {
                IntPtr lowestHwnd = WindowHelper.GetLowestDesktopParentHwnd();

                if (lowestHwnd != IntPtr.Zero)
                    wndPos.hwndInsertAfter = NativeMethods.GetWindow(lowestHwnd, NativeMethods.GetWindowCmd.GW_HWNDPREV);
                else
                    wndPos.hwndInsertAfter = (IntPtr)NativeMethods.WindowZOrder.HWND_BOTTOM;

                wndPos.UpdateMessage(lParam);
            }
        }
        else if ((msg == (int)NativeMethods.WM.SETTINGCHANGE && wParam.ToInt32() == (int)NativeMethods.ESystemParametersInfo.SETWORKAREA) ||
            (msg == (int)NativeMethods.WM.DPICHANGED))
        {
            RefreshGrid();
            foreach (OneDesktopItem item in DataContext.ListItems())
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
        DataContext.RefreshDesktopContent();
    }

    private void Window_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        DataContext.ActionRightClickCommand.Execute(e);
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
        DataContext.UnSelectAll();
    }

    private bool _selection;
    private Point _selectionStart;
    private readonly Pen _selectionPen = new(ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject, 2);

    private bool SelectInRectangle()
    {
        _selection = false;
        InvalidateVisual();
        Rect rect = new(PointToScreen(_selectionStart), PointToScreen(Mouse.GetPosition(this)));
        if (rect.Width == 0 && rect.Height == 0)
            return false;
        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            DataContext.UnSelectAll();
        foreach (OneDesktopItem item in DataContext.ListItems().Where(i => rect.IntersectsWith(i.GetAbsoluteRectangle())))
            item.DataContext.IsSelected = true;
        return true;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (_selection)
            drawingContext.DrawRectangle(ExploripSharedCopy.Constants.Colors.SelectedBackgroundShellObject, _selectionPen, new Rect(_selectionStart, Mouse.GetPosition(this)));
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
                DataContext.StartDrag((OneDesktopItemViewModel)((FrameworkElement)Mouse.DirectlyOver).DataContext);
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
        DataContext.ActionOnKeyCommand.Execute(e);
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        DataContext.Drop(e);
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        ExploripDesktopViewModel.DragOver(e);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DataContext?.Dispose();
    }
}
