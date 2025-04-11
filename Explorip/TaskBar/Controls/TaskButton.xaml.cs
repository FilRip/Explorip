using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Explorip.Constants;
using Explorip.Helpers;
using Explorip.TaskBar.Converters;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskButton.xaml
/// </summary>
public partial class TaskButton : UserControl
{
    private ApplicationWindow _appWindow;
    private readonly TaskButtonStyleConverter StyleConverter = new();
    private ApplicationWindow.WindowState PressedWindowState = ApplicationWindow.WindowState.Inactive;
    private TaskThumbButton _thumb;
    private bool _isLoaded;
    private Timer _timerBeforeShowThumbnail;
    private bool _mouseOver;
    private bool _startDrag;

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

        multiBinding.Bindings.Add(new Binding()
        {
            RelativeSource = RelativeSource.Self,
        });
        multiBinding.Bindings.Add(new Binding(nameof(ApplicationWindow.State)));

        AppButton.SetBinding(StyleProperty, multiBinding);
    }

    private void ScrollIntoView()
    {
        if (_appWindow == null)
            return;

        if (_appWindow.State == ApplicationWindow.WindowState.Active)
            BringIntoView();
    }

    private void TaskButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isLoaded)
            return;

        _appWindow = DataContext as ApplicationWindow;

        if (_appWindow != null)
            _appWindow.PropertyChanged += Window_PropertyChanged;

        double size = ConfigManager.GetTaskbarConfig(((Taskbar)Window.GetWindow(this)).ScreenName).TaskButtonSize;
        MyTaskIcon.Height = size;
        MyTaskIcon.Width = size;
        MyTaskIconBack.Height = size;
        MyTaskIconBack.Width = size;

        _isLoaded = true;
    }

    private void Window_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ApplicationWindow.State))
            ScrollIntoView();
    }

    private void TaskButton_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
            return;

        if (_appWindow != null)
            _appWindow.PropertyChanged -= Window_PropertyChanged;

        _isLoaded = false;
        _mouseOver = false;
        CloseThumbnail();
    }

    private void AppButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _timerBeforeShowThumbnail?.Change(ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
        }
        catch (Exception) { /* Ignore errors */ }
        if (_appWindow.ListWindows.Count == 1)
        {
            if (PressedWindowState == ApplicationWindow.WindowState.Active)
                _appWindow.Minimize();
            else if (_appWindow.State != ApplicationWindow.WindowState.Unknown)
                _appWindow.BringToFront();
        }
        else if (_appWindow.ListWindows.Count == 0)
            ShellHelper.StartProcess(_appWindow.WinFileName, _appWindow.Arguments);
    }

    private void AppButton_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            PressedWindowState = _appWindow.State;
            DragMouseDown();
        }
    }

    private void AppButton_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && (Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightCtrl) == KeyStates.Down)))
        {
            if (_appWindow == null)
                return;
            ShellHelper.StartProcess(_appWindow.WinFileName, _appWindow.Arguments);
        }
        DragMouseUp();
    }

    #region Properties

    public ApplicationWindow ApplicationWindow
    {
        get { return _appWindow; }
    }

    public Taskbar TaskbarParent
    {
        get { return this.FindVisualParent<Taskbar>(); }
    }

    #endregion

    #region Drag

    private void DragMouseDown()
    {
        _startDrag = true;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        StartDrag();
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
    }

    private void DragMouseUp()
    {
        _startDrag = false;
    }

    private void AppButton_OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data)
        {
            ApplicationWindow appWin = (ApplicationWindow)data.GetData(typeof(ApplicationWindow));
            if (appWin != ApplicationWindow)
            {
                (ApplicationWindow.Position, appWin.Position) = (appWin.Position, ApplicationWindow.Position);
                this.FindVisualParent<TaskList>().MyDataContext.RefreshMyCollectionView();
            }
        }
    }

    private async Task StartDrag()
    {
        await Task.Delay(WindowsConstants.DelayIgnoreDrag);
        if (_startDrag)
        {
            DataObject data = new();
            data.SetData(ApplicationWindow);
            DragDrop.DoDragDrop(this, ApplicationWindow, DragDropEffects.Move);
            _startDrag = true;
        }
    }

    #endregion

    #region Thumbnail

    private void AppButton_MouseEnter(object sender, MouseEventArgs e)
    {
        _mouseOver = true;
        if (_appWindow.ListWindows.Count == 0)
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
        if (_appWindow.ListWindows.Count == 0)
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

    #region Context menu

    private void UnpinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ApplicationWindow.IsPinnedApp = false;
        ApplicationWindow.OnPropertyChanged(nameof(ApplicationWindow.IsPinnedApp));
        if (string.IsNullOrWhiteSpace(ApplicationWindow.PinnedShortcut))
            return;
        if (File.Exists(ApplicationWindow.PinnedShortcut))
            File.Delete(ApplicationWindow.PinnedShortcut);
        if (!ApplicationWindow.Launched)
            ApplicationWindow.Dispose();
    }

    private void PinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Shortcut sc = Shortcut.CreateShortcut(ApplicationWindow.WinFileName, ApplicationWindow.Arguments);
        string path = Path.Combine(Environment.SpecialFolder.ApplicationData.FullPath(), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
        path = Path.Combine(path, Path.GetFileNameWithoutExtension(ApplicationWindow.WinFileName) + ".lnk");
        if (!File.Exists(path))
            sc.WriteToFile(path);
        ApplicationWindow.IsPinnedApp = true;
        ApplicationWindow.PinnedShortcut = path;
        ApplicationWindow.OnPropertyChanged(nameof(ApplicationWindow.IsPinnedApp));
    }

    private void StartNewInstanceMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ShellHelper.StartProcess(ApplicationWindow.WinFileName, ApplicationWindow.Arguments);
    }

    private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ApplicationWindow.ListWindows?.Count > 0)
            foreach (IntPtr handle in ApplicationWindow.ListWindows)
                NativeMethods.SendMessage(handle, NativeMethods.WM.CLOSE, 0, 0);
    }

    #endregion
}
