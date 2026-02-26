using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CoolBytes.InteropWinRT;
using CoolBytes.JumpList;
using CoolBytes.JumpList.Automatic;
using CoolBytes.JumpList.Custom;

using Explorip.Constants;
using Explorip.Helpers;

using ExploripConfig.Configuration;

using ExploripSharedCopy.Helpers;

using ManagedShell.Interop;
using ManagedShell.ShellFolders;
using ManagedShell.WindowsTasks;

using Securify.ShellLink;

using VirtualDesktop;

using WpfScreenHelper;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for TaskButton.xaml
/// </summary>
public partial class TaskButton : UserControl
{
    #region Fields

    private ApplicationWindow.WindowState PressedWindowState = ApplicationWindow.WindowState.Inactive;
    private TaskThumbButton _thumb;
    private bool _isLoaded;
    private Timer _timerBeforeShowThumbnail;
    private bool _mouseOver;
    private bool _startDrag;

    #endregion

    #region Constructor/Initializer

    public TaskButton()
    {
        InitializeComponent();
    }

    private void ScrollIntoView()
    {
        if (DataContext == null)
            return;

        if (DataContext.State == ApplicationWindow.WindowState.Active)
            BringIntoView();
    }

    private void TaskButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isLoaded)
            return;

        DataContext?.PropertyChanged += Window_PropertyChanged;

        double size = ConfigManager.GetTaskbarConfig(((Taskbar)Window.GetWindow(this)).NumScreen).TaskButtonSize;
        MyTaskIcon.Height = size;
        MyTaskIcon.Width = size;
        MyTaskIconBack.Height = size;
        MyTaskIconBack.Width = size;

        if (ConfigManager.GetTaskbarConfig(((Taskbar)Window.GetWindow(this)).NumScreen).ShowTitleApplicationWindow)
        {
            IconColumn.Width = new GridLength(size + 20, GridUnitType.Pixel);
            TxtTitle.Margin = new Thickness(0, 0, ConfigManager.MarginTitleApplicationWindow, 0);
        }
        else
        {
            TitleColumn.Width = new GridLength(0, GridUnitType.Pixel);
            TxtTitle.Margin = new Thickness(0);
        }
        ProgressBarWindow.Height = size + 20;
        if (ConfigManager.TaskbarProgressBarHeight > 0 && ConfigManager.TaskbarProgressBarHeight < size + 20)
            ProgressBarWindow.Height = ConfigManager.TaskbarProgressBarHeight;
        ProgressBarWindow.Foreground = ConfigManager.TaskButtonProgressBarColor;

        if (!ConfigManager.UseJumpList)
        {
            JumpListContextMenu.Visibility = Visibility.Collapsed;
            SeparatorJumpListContextMenu.Visibility = Visibility.Collapsed;
        }

        _isLoaded = true;
    }

    private void Window_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DataContext.State))
            ScrollIntoView();
    }

    private void TaskButton_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
            return;

        DataContext?.PropertyChanged -= Window_PropertyChanged;

        _isLoaded = false;
        _mouseOver = false;
        CloseThumbnail();
    }

    #endregion

    #region Actions button

    private void AppButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!_timerBeforeShowThumbnail.IsDisposed())
                _timerBeforeShowThumbnail?.Change(ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
        }
        catch (Exception) { /* Ignore errors */ }
        if (DataContext?.ListWindows?.Count == 1)
        {
            if (PressedWindowState == ApplicationWindow.WindowState.Active)
                DataContext.Minimize();
            else if (DataContext.State != ApplicationWindow.WindowState.Unknown)
                DataContext.BringToFront();
        }
        else if (DataContext?.ListWindows?.Count == 0)
            DataContext?.StartNewInstance();
        else
            try
            {
                _timerBeforeShowThumbnail.Change(0, Timeout.Infinite);
            }
            catch (Exception) { /* Ignore errors */ }
    }

    private void AppButton_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            PressedWindowState = DataContext?.State ?? ApplicationWindow.WindowState.Unknown;
            DragMouseDown(e);
        }
    }

    private void AppButton_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && (Keyboard.GetKeyStates(Key.LeftCtrl).HasFlag(KeyStates.Down) || Keyboard.GetKeyStates(Key.RightCtrl).HasFlag(KeyStates.Down))))
        {
            if (DataContext == null)
                return;
            DataContext.StartNewInstance();
        }
        DragMouseUp();
    }

    #endregion

    #region Properties

    public new ApplicationWindow DataContext
    {
        get { return base.DataContext as ApplicationWindow; }
        set { base.DataContext = value; }
    }

    public Taskbar TaskbarParent
    {
        get { return this.FindVisualParent<Taskbar>(); }
    }

    #endregion

    #region Drag'n Drop

    private void DragMouseDown(MouseEventArgs e)
    {
        _startDrag = true;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        StartDrag(e);
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
    }

    private void DragMouseUp()
    {
        _startDrag = false;
    }

    private void AppButton_OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data && DataContext != null)
        {
            if (data.GetDataPresent(typeof(ApplicationWindow)))
            {
                ApplicationWindow appWin = (ApplicationWindow)data.GetData(typeof(ApplicationWindow));
                if (appWin != DataContext)
                {
                    (DataContext.Position, appWin.Position) = (appWin.Position, DataContext.Position);
                    this.FindVisualParent<TaskList>().DataContext.RefreshMyCollectionView();
                }
            }
            else if (data.GetFileDropList()?.Count > 0)
            {
                if (DataContext.ListWindows.Count == 1)
                    DataContext.BringToFront();
                else if (DataContext.ListWindows.Count > 1)
                {
                    _mouseOver = true;
                    ShowThumbnail(null);
                }
            }
        }
    }

    private async Task StartDrag(MouseEventArgs e)
    {
        await Task.Delay(WindowsConstants.DelayIgnoreDrag);
        if (_startDrag && DataContext != null)
        {
            DataObject data = new();
            data.SetData(DataContext);
            CloseThumbnail();
            DragGhostAdorner.StartDragGhost(this, e);
            DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
            DragGhostAdorner.StopDragGhost();
        }
    }

    private void AppButton_Drop(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data && data.GetFileDropList()?.Count > 0 && DataContext != null)
        {
            StringBuilder arguments = new();
            foreach (string file in data.GetFileDropList())
            {
                if (arguments.Length > 0)
                    arguments.Append(' ');
                arguments.Append("\"" + file + "\"");
            }
            DataContext.StartNewInstance(arguments.ToString());
        }
        DragMouseUp();
    }

    private void UserControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        DragGhostAdorner.UpdateDragGhost();
        e.UseDefaultCursors = false;
        e.Handled = true;
    }

    #endregion

    #region Thumbnail

    private void AppButton_MouseEnter(object sender, MouseEventArgs e)
    {
        _mouseOver = true;
        if (DataContext == null || DataContext.ListWindows.Count == 0)
            return;

        if (ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).TaskbarDisableThumb)
            return;

        _timerBeforeShowThumbnail?.Dispose();
        _timerBeforeShowThumbnail = new Timer(ShowThumbnail, null, ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
    }

    private void ShowThumbnail(object userData)
    {
        if (!_mouseOver || !_isLoaded)
            return;
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                _thumb?.Close();
                _thumb = new TaskThumbButton(this);
                _thumb.Show();
            }
            catch (Exception) { /* Ignore errors */ }
        });
    }

    private void AppButton_MouseLeave(object sender, MouseEventArgs e)
    {
        _mouseOver = false;
        if (DataContext == null || DataContext.ListWindows.Count == 0)
        {
            CloseThumbnail();
            return;
        }

        Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (TaskbarParent != null)
                {
                    await Task.Delay(TaskbarParent.TimeBeforeAutoCloseThumb);
                    if (_thumb != null && !_thumb.DataContext.MouseIn)
                        CloseThumbnail();
                }
            });
        });
    }

    private void CloseThumbnail()
    {
        Application.Current?.Dispatcher?.Invoke(() =>
        {
            try
            {
                if (!_timerBeforeShowThumbnail.IsDisposed())
                {
                    _timerBeforeShowThumbnail?.Change(Timeout.Infinite, Timeout.Infinite);
                    _timerBeforeShowThumbnail?.Dispose();
                }
            }
            catch (Exception) { /* Ignore errors */ }
            _thumb?.Close();
        });
    }

    #endregion

    #region Context menu

    private void UnpinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext == null)
            return;
        DataContext.IsPinnedApp = false;
        DataContext.OnPropertyChanged(nameof(DataContext.IsPinnedApp));
        if (!string.IsNullOrWhiteSpace(DataContext.PinnedShortcut) && File.Exists(DataContext.PinnedShortcut))
            File.Delete(DataContext.PinnedShortcut);
        TaskList taskList = this.FindVisualParent<TaskList>();
        if (!DataContext.Launched)
            DataContext.Dispose();
        taskList.DataContext.RefreshMyCollectionView();
    }

    private void PinMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext == null)
            return;
        Shortcut sc;
        string filename = Path.GetFileNameWithoutExtension(DataContext.WinFileName);
        if (DataContext.IsUWP)
        {
            sc = Shortcut.CreateShortcut(Path.Combine(Environment.SpecialFolder.Windows.FullPath(), "explorer.exe"), $"shell:AppsFolder\\{DataContext.AppUserModelID}");
            filename = DataContext.Title;
            foreach (char c in Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, ' ');
        }
        else
            sc = Shortcut.CreateShortcut(DataContext.WinFileName, DataContext.Arguments);
        string path = ConfigManager.GetTaskbarConfig(TaskbarParent.NumScreen).PathPinnedApp;
        path = Path.Combine(path, filename + ".lnk");
        if (!File.Exists(path))
            sc.WriteToFile(path);
        DataContext.IsPinnedApp = true;
        DataContext.PinnedShortcut = path;
        DataContext.OnPropertyChanged(nameof(DataContext.IsPinnedApp));
    }

    private void StartNewInstanceMenuItem_Click(object sender, RoutedEventArgs e)
    {
        DataContext?.StartNewInstance();
    }

    private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext?.ListWindows?.Count > 0)
            foreach (IntPtr handle in DataContext.ListWindows.Select(w => w.Handle))
                NativeMethods.SendMessage(handle, NativeMethods.WM.CLOSE, 0, 0);
    }

    #endregion

    #region Context menu Moving window

    private void MoveToScreen_MouseEnter(object sender, MouseEventArgs e)
    {
        MoveToScreen.Items.Clear();
        foreach (Screen screen in Screen.AllScreens)
        {
            MenuItem mi = new()
            {
                Header = screen.DisplayNumber.ToString(),
                Tag = screen,
            };
            mi.Click += MoveToScreen_Click;
            MoveToScreen.Items.Add(mi);
        }
    }

    private void MoveToVirtualDesktop_MouseEnter(object sender, MouseEventArgs e)
    {
        MoveToVirtualDesktop.Items.Clear();
        foreach (VirtualDesktop.Models.VirtualDesktop vd in VirtualDesktopManager.GetDesktops())
        {
            MenuItem mi = new()
            {
                Header = vd.Name,
                Tag = vd,
            };
            if (DataContext?.ListWindows?.Count > 0 && vd.Id == DataContext.ListWindows[0].VirtualDesktopId)
                mi.IsEnabled = false;
            mi.Click += MoveToVirtualDesktop_Click;
            MoveToVirtualDesktop.Items.Add(mi);
        }
    }

    private void MoveToScreen_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is Screen screenDest && DataContext?.ListWindows?.Count > 0)
        {
            Screen screenSrc = Screen.FromHandle(DataContext.ListWindows[0].Handle);
            NativeMethods.Rect size;
            if (screenSrc.ScaleFactor == screenDest.ScaleFactor)
                WindowScreenHelper.SetWindowPosition(DataContext.ListWindows[0].Handle, (int)screenDest.WpfWorkingArea.X, (int)screenDest.WpfWorkingArea.Y);
            else
            {
                NativeMethods.GetWindowRect(DataContext.ListWindows[0].Handle, out size);
                double width = size.Width / screenSrc.ScaleFactor;
                double height = size.Height / screenSrc.ScaleFactor;
                if (screenDest.ScaleFactor > screenSrc.ScaleFactor)
                {
                    width *= screenDest.ScaleFactor;
                    height *= screenDest.ScaleFactor;
                }
                else
                {
                    width /= screenDest.ScaleFactor;
                    height /= screenDest.ScaleFactor;
                }
                width = Math.Min(screenDest.WpfWorkingArea.Width - 16, width);
                height = Math.Min(screenDest.WpfWorkingArea.Height - 16, height);

                WindowScreenHelper.SetWindowPosition(DataContext.ListWindows[0].Handle, (int)screenDest.WpfWorkingArea.X, (int)screenDest.WpfWorkingArea.Y, (int)width, (int)height);
            }
            NativeMethods.GetWindowRect(DataContext.ListWindows[0].Handle, out size);
            int posX = 0, posY = 0;
            if (screenDest.WpfWorkingArea.Width > size.Width)
                posX = (int)(screenDest.WpfWorkingArea.Left * screenDest.ScaleFactor) + (int)((screenDest.WpfWorkingArea.Width * screenDest.ScaleFactor - size.Width) / 2);
            if (screenDest.WpfWorkingArea.Height > size.Height)
                posY = (int)(screenDest.WpfWorkingArea.Top * screenDest.ScaleFactor) + (int)((screenDest.WpfWorkingArea.Height * screenDest.ScaleFactor - size.Height) / 2);
            WindowScreenHelper.SetWindowPosition(DataContext.ListWindows[0].Handle, posX, posY, size.Width, size.Height);
        }
    }

    private void MoveToVirtualDesktop_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is VirtualDesktop.Models.VirtualDesktop vd && DataContext?.ListWindows?.Count > 0)
        {
            VirtualDesktopManager.MoveToDesktop(DataContext.ListWindows[0].Handle, vd);
            DataContext.ListWindows[0].VirtualDesktopId = vd.Id;
            TaskbarParent.MyTaskList.DataContext.ForceRefresh();
        }
    }

    private void MoveWithMouse_Click(object sender, RoutedEventArgs e)
    {
        DataContext?.Move();
    }

    #endregion

    #region Context menu JumpList

    private void MainContextMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        try
        {
            if (e.NewValue is bool visible && visible && ConfigManager.UseJumpList && DataContext?.ListWindows != null)
            {
                _mouseOver = false;
                _thumb?.Close();
                JumpListContextMenu.Items.Clear();
                AutomaticDestination autoDest = ExtensionsJumpList.GetAutomaticJumpList(DataContext.ListWindows[0].Handle);
                CustomDestination customDest = ExtensionsJumpList.GetCustomJumpList(DataContext.ListWindows[0].Handle);
                if (autoDest?.DestListEntries?.Count > 0)
                {
                    MenuItem category = new()
                    {
                        Header = Constants.Localization.PINNED_JUMPLIST,
                        Style = (Style)FindResource("MenuItemWithSubMenuStyle"),
                    };
                    List<MenuItem> otherItems = [];
                    foreach (AutoDestList adl in autoDest?.DestListEntries)
                    {
                        MenuItem mi = MakeMenuItemEntry(adl.Lnk);
                        mi.Click += JumpList_Click;
                        if (adl.Pinned)
                            category.Items.Add(mi);
                        else
                            otherItems.Add(mi);
                    }
                    if (category.Items.Count > 0)
                    {
                        JumpListContextMenu.Items.Add(category);
                        if (otherItems.Count > 0)
                        {
                            category = new MenuItem()
                            {
                                Header = Constants.Localization.TASK_JUMPLIST,
                                Style = (Style)FindResource("MenuItemWithSubMenuStyle"),
                            };
                            otherItems.ForEach(mi => category.Items.Add(mi));
                            JumpListContextMenu.Items.Add(category);
                        }
                    }
                    else
                        JumpListContextMenu.Items.Add(otherItems);
                }
                if (customDest?.Entries?.Count > 0)
                {
                    foreach (Entry entry in customDest.Entries)
                    {
                        if (entry.LnkFiles?.Count > 0)
                        {
                            MenuItem category = new()
                            {
                                Header = string.IsNullOrWhiteSpace(entry.Name) ? Constants.Localization.TASK_JUMPLIST : entry.Name,
                                Style = (Style)FindResource("MenuItemWithSubMenuStyle"),
                            };
                            JumpListContextMenu.Items.Add(category);
                            foreach (Shortcut lnk in entry.LnkFiles)
                            {
                                MenuItem mi = MakeMenuItemEntry(lnk);
                                mi.Click += JumpList_Click;
                                category.Items.Add(mi);
                            }
                        }
                    }
                }

                JumpListContextMenu.Visibility = JumpListContextMenu.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                SeparatorJumpListContextMenu.Visibility = JumpListContextMenu.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        catch (Exception)
        {
            // Ignore errors
        }
    }

    private MenuItem MakeMenuItemEntry(Shortcut lnk)
    {
        string iconPath = lnk.IconPath;
        Image @is = new();
        string name = lnk.Name;
        if (string.IsNullOrWhiteSpace(name))
            name = lnk.Target;
        if (Path.GetExtension(iconPath).ToLower() == ".exe" || Path.GetExtension(iconPath).ToLower() == ".dll")
            @is.Source = IconManager.Convert(IconManager.Extract(iconPath, lnk.IconIndex, false));
        else if (iconPath.ToLower().StartsWith("ms-appx:/"))
        {
            string appUserModelId = DataContext.AppUserModelID;
            if (appUserModelId.EndsWith("!App"))
                appUserModelId = appUserModelId.Substring(0, appUserModelId.Length - 4);
            string uwpPath = UwpPackageManager.PathOfUwp(appUserModelId);
            if (!string.IsNullOrWhiteSpace(uwpPath))
            {
                iconPath = Path.Combine(uwpPath, iconPath.Replace("ms-appx:", "").TrimStart('/').Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
                if (!File.Exists(iconPath))
                    iconPath = Path.Combine(Path.GetDirectoryName(iconPath), Path.GetFileNameWithoutExtension(iconPath) + ".targetsize-16_contrast-" + (WindowsSettings.IsWindowsApplicationInDarkMode() ? "black" : "white") + Path.GetExtension(iconPath));
                if (File.Exists(iconPath))
                    @is.Source = new BitmapImage(new Uri(iconPath));
            }
        }
        else if (iconPath.Contains("JumpListIcon") && Path.GetExtension(iconPath).ToLower() == ".tmp" &&
            lnk.StringData?.CommandLineArguments != null && lnk.StringData.CommandLineArguments.Contains("http"))
        {
            string[] args = lnk.StringData.CommandLineArguments.Split(' ');
            string website = args.FirstOrDefault(s => s.StartsWith("http"));
            if (!string.IsNullOrWhiteSpace(website))
                @is.Source = IconManager.GetWebsiteFavIcon(website);
        }
        else if (Uri.TryCreate(name, UriKind.RelativeOrAbsolute, out Uri dir) && dir.IsFile)
        {
            if (Directory.Exists(name))
            {
                ShellFolder sf = new(name, IntPtr.Zero);
                name = sf.DisplayName;
                ImageSource icon = sf.LargeIcon;
                @is.Source = icon;
                sf.Dispose();
            }
            else if (File.Exists(name))
            {
                ShellItem si = new(name);
                name = si.DisplayName;
                ImageSource icon = si.LargeIcon;
                @is.Source = icon;
                si.Dispose();
            }
        }
        if (name?.StartsWith("ms-resource://") == true)
        {
            string appUserModelId = DataContext?.AppUserModelID ?? string.Empty;
            if (appUserModelId.EndsWith("!App"))
                appUserModelId = appUserModelId.Substring(0, appUserModelId.Length - 4);
            name = UwpPackageManager.StringOfUwp(appUserModelId, name);
        }
        MenuItem mi = new()
        {
            Header = name,
            Tag = lnk,
            Icon = @is,
        };
        return mi;
    }

    private void JumpList_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is Shortcut sc)
            sc.Start();
    }

    #endregion
}
