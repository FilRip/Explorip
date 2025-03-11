using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Explorip.Helpers;
using Explorip.TaskBar.Converters;

using ExploripConfig.Configuration;

using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskButton.xaml
/// </summary>
public partial class TaskButton : UserControl
{
    private ApplicationWindow Window;
    private readonly TaskButtonStyleConverter StyleConverter = new();
    private ApplicationWindow.WindowState PressedWindowState = ApplicationWindow.WindowState.Inactive;
    private TaskThumbButton _thumb;
    private bool _isLoaded;
    private Timer _timerBeforeShowThumbnail;
    private bool _mouseOver;

    public TaskButton()
    {
        InitializeComponent();
        SetStyle();
    }

    private void SetStyle()
    {
        MultiBinding multiBinding = new()
        {
            Converter = StyleConverter,
        };

        multiBinding.Bindings.Add(new Binding { RelativeSource = RelativeSource.Self });
        multiBinding.Bindings.Add(new Binding("State"));

        AppButton.SetBinding(StyleProperty, multiBinding);
    }

    private void ScrollIntoView()
    {
        if (Window == null)
        {
            return;
        }

        if (Window.State == ApplicationWindow.WindowState.Active)
        {
            BringIntoView();
        }
    }

    private void TaskButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        Window = DataContext as ApplicationWindow;

        // drag support - delayed activation using system setting
        dragTimer = new DispatcherTimer { Interval = SystemParameters.MouseHoverTime };
        dragTimer.Tick += DragTimer_Tick;

        if (Window != null)
        {
            Window.PropertyChanged += Window_PropertyChanged;
        }

        _isLoaded = true;
    }

    private void Window_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "State")
        {
            ScrollIntoView();
        }
    }

    private void TaskButton_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        if (Window != null)
        {
            Window.PropertyChanged -= Window_PropertyChanged;
        }

        _isLoaded = false;
        _mouseOver = false;
        CloseThumbnail();
    }

    #region System context menu

    private void AppButton_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (Window == null)
        {
            return;
        }

        NativeMethods.WindowShowStyle wss = Window.ShowStyle;
        int ws = Window.WindowStyles;

        // disable window operations depending on current window state. originally tried implementing via bindings but found there is no notification we get regarding maximized state
        MaximizeMenuItem.IsEnabled = wss != NativeMethods.WindowShowStyle.ShowMaximized && (ws & (int)NativeMethods.WindowStyles.WS_MAXIMIZEBOX) != 0;
        MinimizeMenuItem.IsEnabled = wss != NativeMethods.WindowShowStyle.ShowMinimized && (ws & (int)NativeMethods.WindowStyles.WS_MINIMIZEBOX) != 0;
        RestoreMenuItem.IsEnabled = wss != NativeMethods.WindowShowStyle.ShowNormal;
        MoveMenuItem.IsEnabled = wss == NativeMethods.WindowShowStyle.ShowNormal;
        SizeMenuItem.IsEnabled = wss == NativeMethods.WindowShowStyle.ShowNormal && (ws & (int)NativeMethods.WindowStyles.WS_MAXIMIZEBOX) != 0;
    }

    private void CloseMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Window?.Close();
    }

    private void RestoreMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Window?.Restore();
    }

    private void MoveMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Window?.Move();
    }

    private void SizeMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Window?.Size();
    }

    private void MinimizeMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Window?.Minimize();
    }

    private void MaximizeMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Window?.Maximize();
    }

    #endregion

    private void AppButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Window.ListWindows.Count == 1 || Window.Handle != IntPtr.Zero)
        {
            if (PressedWindowState == ApplicationWindow.WindowState.Active)
            {
                Window.Minimize();
            }
            else if (Window.State != ApplicationWindow.WindowState.Unknown)
            {
                Window.BringToFront();
            }
        }
        else if (Window.ListWindows.Count == 0)
        {
            ManagedShell.Common.Helpers.ShellHelper.StartProcess(Window.WinFileName, Window.Arguments);
            Window.OnPropertyChanged(nameof(ApplicationWindow.Handle));
        }
    }

    private void AppButton_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            PressedWindowState = Window.State;
        }
    }

    private void AppButton_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            if (Window == null)
            {
                return;
            }

            ManagedShell.Common.Helpers.ShellHelper.StartProcess(Window.WinFileName, Window.Arguments);
            // TODO : Update Handle and State
            Window.OnPropertyChanged(nameof(ApplicationWindow.Handle));
        }
    }

    public ApplicationWindow ApplicationWindow
    {
        get { return Window; }
    }

    #region Drag
    private bool inDrag;
    private DispatcherTimer dragTimer;

    private void DragTimer_Tick(object sender, EventArgs e)
    {
        if (inDrag)
        {
            Window?.BringToFront();
        }

        dragTimer.Stop();
    }

    private void AppButton_OnDragEnter(object sender, DragEventArgs e)
    {
        if (!inDrag)
        {
            inDrag = true;
            dragTimer.Start();
        }
    }

    private void AppButton_OnDragLeave(object sender, DragEventArgs e)
    {
        if (inDrag)
        {
            dragTimer.Stop();
            inDrag = false;
        }
    }
    #endregion

    #region Thumbnail

    private void AppButton_MouseEnter(object sender, MouseEventArgs e)
    {
        _mouseOver = true;
        if (Window.Handle == IntPtr.Zero && Window.ListWindows.Count == 0)
            return;

        if (ConfigManager.GetTaskbarConfig(this.FindVisualParent<Taskbar>().ScreenName).TaskbarDisableThumb)
            return;

        _timerBeforeShowThumbnail = new Timer(ShowThumbnail, null, ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
    }

    private void ShowThumbnail(object userData)
    {
        if (!_mouseOver || !_isLoaded)
            return;
        Application.Current.Dispatcher.Invoke(() =>
        {
            _thumb?.Close();
            _thumb = new TaskThumbButton(this);
            _thumb.Show();
        });
    }

    private void AppButton_MouseLeave(object sender, MouseEventArgs e)
    {
        _mouseOver = false;
        if (Window.Handle == IntPtr.Zero && Window.ListWindows.Count == 0)
        {
            CloseThumbnail();
            return;
        }

        Task.Run(async () =>
        {
            await Task.Delay(200);
            if (_thumb != null && !_thumb.MouseIn)
                CloseThumbnail();
        });
    }

    private void CloseThumbnail()
    {
        Application.Current?.Dispatcher?.Invoke(() =>
        {
            try
            {
                _timerBeforeShowThumbnail?.Change(Timeout.Infinite, Timeout.Infinite);
                _timerBeforeShowThumbnail?.Dispose();
            }
            catch (Exception) { /* Ignore errors */ }
            _thumb?.Close();
        });
    }

    #endregion

    public Taskbar TaskbarParent
    {
        get
        {
            return this.FindVisualParent<Taskbar>();
        }
    }
}
