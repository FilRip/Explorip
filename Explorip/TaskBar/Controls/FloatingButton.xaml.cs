using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Interop;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Logique d'interaction pour FloatingButton.xaml
/// </summary>
public partial class FloatingButton : UserControl
{
    private const int DelayBeforeStartDrag = 1000;

    private Timer _timer;
    private bool _exitDrag;

    public FloatingButton()
    {
        InitializeComponent();
    }

    public FloatingButtonViewModel MyDataContext
    {
        get { return (FloatingButtonViewModel)DataContext; }
    }

    public void SaveNewPos()
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            MyDataContext.NewYPos = MyDataContext.ParentTaskbar.Top;
        }, System.Windows.Threading.DispatcherPriority.Background);
    }

    private void ToggleButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _exitDrag = false;
        if (_timer == null || _timer.IsDisposed())
            _timer = new Timer(StartDrag, null, DelayBeforeStartDrag, Timeout.Infinite);
        else
            _timer.Change(DelayBeforeStartDrag, Timeout.Infinite);
    }

    private void StartDrag(object userData)
    {
        Point offset = default;
        Application.Current.Dispatcher.Invoke(() =>
        {
            offset = Mouse.GetPosition(this);
            Mouse.OverrideCursor = Cursors.ScrollNS;
        });
        System.Drawing.Point posReference = new();
        NativeMethods.GetCursorPos(ref posReference);
        while (!_exitDrag)
        {
            System.Drawing.Point pos = new();
            NativeMethods.GetCursorPos(ref pos);
            Application.Current.Dispatcher.Invoke(() => { MyDataContext.ParentTaskbar.Top = (pos.Y / MyDataContext.ParentTaskbar.Screen.DpiScale) - offset.Y; });
            Thread.Sleep(10);
        }
        SaveNewPos();
        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
    }

    private void ToggleButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.OverrideCursor != null)
            e.Handled = true;
        StopDrag();
    }

    private void StopDrag()
    {
        if (_timer != null && !_timer.IsDisposed())
            _timer.Dispose();
        _exitDrag = true;
    }
}
