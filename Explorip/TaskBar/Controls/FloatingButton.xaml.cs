using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Logging;
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

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;
        MyDataContext.SetParentTaskbar((Taskbar)Window.GetWindow(this));
    }

    private void ToggleButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _exitDrag = false;
        if (_timer == null || _timer.IsDisposed())
            _timer = new Timer(StartDrag, null, DelayBeforeStartDrag, Timeout.Infinite);
        else
            _timer.Change(DelayBeforeStartDrag, Timeout.Infinite);
        ShellLogger.Debug("Start Timer to drag floating button");
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
        ShellLogger.Debug("Start moving floating button");
        while (!_exitDrag)
        {
            System.Drawing.Point pos = new();
            NativeMethods.GetCursorPos(ref pos);
            Application.Current.Dispatcher.Invoke(() => { MyDataContext.ParentTaskbar.Top = pos.Y - offset.Y; });
            if (Math.Abs(pos.X - posReference.X) > 56)
                StopDrag();
            Thread.Sleep(10);
        }
        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.None; });
        ShellLogger.Debug("Stop moving floating button");
    }

    private void ToggleButton_MouseLeave(object sender, MouseEventArgs e)
    {
        StopDrag();
    }

    private void ToggleButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        StopDrag();
    }

    private void StopDrag()
    {
        if (_timer != null && !_timer.IsDisposed())
            _timer.Dispose();
        _exitDrag = true;
        ShellLogger.Debug("End Timer to drag floating button");
    }

    private void ToggleButton_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && !_exitDrag)
            StopDrag();
    }
}
