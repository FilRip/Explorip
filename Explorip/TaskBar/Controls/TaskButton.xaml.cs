﻿using System;
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
        multiBinding.Bindings.Add(new Binding(nameof(_appWindow.State)));

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
        if (e.PropertyName == nameof(_appWindow.State))
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
            _appWindow.StartNewInstance();
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
            _appWindow.StartNewInstance();
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
            if (appWin != _appWindow)
            {
                (_appWindow.Position, appWin.Position) = (appWin.Position, _appWindow.Position);
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
            data.SetData(_appWindow);
            DragDrop.DoDragDrop(this, _appWindow, DragDropEffects.Move);
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
        _appWindow.IsPinnedApp = false;
        _appWindow.OnPropertyChanged(nameof(_appWindow.IsPinnedApp));
        if (!string.IsNullOrWhiteSpace(_appWindow.PinnedShortcut) && File.Exists(_appWindow.PinnedShortcut))
            File.Delete(_appWindow.PinnedShortcut);
        TaskList taskList = this.FindVisualParent<TaskList>();
        if (!_appWindow.Launched)
            _appWindow.Dispose();
        taskList.MyDataContext.RefreshMyCollectionView();
    }

    private void PinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Shortcut sc;
        string filename = Path.GetFileNameWithoutExtension(_appWindow.WinFileName);
        if (_appWindow.IsUWP)
        {
            sc = Shortcut.CreateShortcut(Path.Combine(Environment.SpecialFolder.Windows.FullPath(), "explorer.exe"), $"shell:AppsFolder\\{_appWindow.AppUserModelID}");
            filename = _appWindow.Title;
            foreach (char c in Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, ' ');
        }
        else
            sc = Shortcut.CreateShortcut(_appWindow.WinFileName, _appWindow.Arguments);
        string path = Path.Combine(Environment.SpecialFolder.ApplicationData.FullPath(), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
        path = Path.Combine(path, filename + ".lnk");
        if (!File.Exists(path))
            sc.WriteToFile(path);
        _appWindow.IsPinnedApp = true;
        _appWindow.PinnedShortcut = path;
        _appWindow.OnPropertyChanged(nameof(_appWindow.IsPinnedApp));
    }

    private void StartNewInstanceMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _appWindow.StartNewInstance();
    }

    private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_appWindow.ListWindows?.Count > 0)
            foreach (IntPtr handle in _appWindow.ListWindows)
                NativeMethods.SendMessage(handle, NativeMethods.WM.CLOSE, 0, 0);
    }

    #endregion
}
