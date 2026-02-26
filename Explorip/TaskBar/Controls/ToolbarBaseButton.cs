using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Explorip.Constants;
using Explorip.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

using Securify.ShellLink;

namespace Explorip.TaskBar.Controls;

public class ToolbarBaseButton : UserControl
{
    private bool _startDrag;
    private Timer _timer;
    protected bool? _isFolder = null;
    protected string _folderPath;
    private ShellFolder _shellFolder;
    private bool _ignoreReload;

    public bool DisableFolderPreview { get; set; }

    public ToolbarBaseButton() : base()
    {
        PreviewMouseDown += DragMouseDown;
        PreviewMouseUp += DragMouseUp;
        DragEnter += OnDragEnter;
        Drop += OnDrop;
        MouseEnter += ToolbarBaseButton_MouseEnter;
        MouseLeave += ToolbarBaseButton_MouseLeave;
        GiveFeedback += ToolbarBaseButton_GiveFeedback;
        Unloaded += ToolbarBaseButton_Unloaded;
        IsVisibleChanged += ToolbarBaseButton_IsVisibleChanged;
    }

    private void ToolbarBaseButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ignoreReload = true;
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            await Task.Delay(1000);
            _ignoreReload = false;
        });
    }

    private void ToolbarBaseButton_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_ignoreReload)
            return;

        PreviewMouseDown -= DragMouseDown;
        PreviewMouseUp -= DragMouseUp;
        DragEnter -= OnDragEnter;
        Drop -= OnDrop;
        MouseEnter -= ToolbarBaseButton_MouseEnter;
        MouseLeave -= ToolbarBaseButton_MouseLeave;
        GiveFeedback -= ToolbarBaseButton_GiveFeedback;
        Unloaded -= ToolbarBaseButton_Unloaded;
    }

    public new ShellFile DataContext
    {
        get { return (ShellFile)base.DataContext; }
        set { base.DataContext = value; }
    }

    private void ToolbarBaseButton_MouseLeave(object sender, MouseEventArgs e)
    {
        _timer?.Dispose();
    }

    private void ToolbarBaseButton_MouseEnter(object sender, MouseEventArgs e)
    {
        _timer?.Dispose();
        if (!DisableFolderPreview)
            _timer = new Timer(PreviewFolder, null, ConfigManager.TaskbarDelayBeforeShowThumbnail, Timeout.Infinite);
    }

    private void PreviewFolder(object userData)
    {
        if (DisableFolderPreview)
            return;
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            if (!_isFolder.HasValue && base.DataContext is ShellFile sf)
            {
                _isFolder = false;
                if (sf.IsFolder)
                {
                    _folderPath = sf.Path;
                    _isFolder = true;
                }
                else if (Path.GetExtension(sf.FileName).ToLower() == ".lnk")
                {
                    try
                    {
                        Shortcut sc = Shortcut.ReadFromFile(sf.Path);
                        string newPath = sc.Target;
                        if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                        {
                            _isFolder = true;
                            _folderPath = newPath;
                        }
                    }
                    catch (Exception)
                    {
                        _isFolder = false;
                    }
                }
            }
            if (_isFolder.Value)
            {
                Popup myPopup = ((Taskbar)Window.GetWindow(this)).MyPopup;
                if (myPopup != null)
                {
                    ((ItemsControl)((Border)myPopup.Child).Child).Items.Clear();
                    if (myPopup.IsOpen)
                        myPopup.IsOpen = false;
                }
                myPopup.PlacementTarget = this;
                _shellFolder ??= new ShellFolder(_folderPath, IntPtr.Zero, false);
                foreach (ShellFile file in _shellFolder.Files)
                    AddMenuItem(file);
                myPopup.IsOpen = true;
            }
            else
                DisableFolderPreview = true;
        });
    }

    private void AddMenuItem(ShellFile item)
    {
        MenuItem mi = CreateMenuItem(item);
        ((ItemsControl)((Border)((Taskbar)Window.GetWindow(this)).MyPopup.Child).Child).Items.Add(mi);
        if (Path.GetExtension(item.FileName) == ".lnk")
        {
            try
            {
                Shortcut sc = Shortcut.ReadFromFile(item.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    ShellFolder sf = new(newPath, IntPtr.Zero);
                    mi.Style = (Style)Application.Current.FindResource(WindowsConstants.StyleMenuItemWithSubMenu);
                    ExpandFolder(mi, sf);
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }
        else if (item.IsFolder)
        {
            mi.Style = (Style)Application.Current.FindResource(WindowsConstants.StyleMenuItemWithSubMenu);
            ExpandFolder(mi, new ShellFolder(item.Path, IntPtr.Zero));
        }
    }

    private void ExpandFolder(MenuItem mi, ShellFolder folder, int nbRecursive = 1)
    {
        if (nbRecursive > ConfigManager.MaxRecursiveSubFolderInToolbar || folder == null)
            return;
        foreach (ShellFile sf in folder.Files)
        {
            MenuItem subMenu = CreateMenuItem(sf);
            mi.Items.Add(subMenu);
            if (sf.IsFolder)
            {
                subMenu.Style = (Style)Application.Current.FindResource(WindowsConstants.StyleMenuItemWithSubMenu);
                ExpandFolder(subMenu, new ShellFolder(sf.Path, IntPtr.Zero), nbRecursive++);
            }
            else if (Path.GetExtension(sf.FileName) == ".lnk")
            {
                Shortcut sc = Shortcut.ReadFromFile(sf.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    mi.Style = (Style)Application.Current.FindResource(WindowsConstants.StyleMenuItemWithSubMenu);
                    ExpandFolder(subMenu, new ShellFolder(newPath, IntPtr.Zero), nbRecursive++);
                }
            }
        }
    }

    private MenuItem CreateMenuItem(ShellFile item)
    {
        item.AllowAsync = true;
        MenuItem mi = new()
        {
            Header = item.DisplayName,
            Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
            Icon = new Image()
            {
                Source = item.SmallIcon,
            },
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0),
            Tag = item,
            IsCheckable = false,
        };
        item.IconLoadedArgument = mi;
        item.IconLoaded = new Action<ShellItem, object>((item, menuItem) => Application.Current.Dispatcher.Invoke(() =>
        {
            ((MenuItem)menuItem).Icon = new Image() { Source = item.SmallIcon };
        }));
        mi.PreviewMouseLeftButtonUp += Mi_PreviewMouseLeftButtonUp;
        mi.PreviewMouseRightButtonUp += Mi_PreviewMouseRightButtonUp;
        return mi;
    }

    private void Mi_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is MenuItem mi && mi.Tag is ShellFile sf && InvokeContextMenu(sf, true, new ShellFolder(Path.GetDirectoryName(sf.Path), IntPtr.Zero)))
            e.Handled = true;
    }

    private void Mi_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is MenuItem mi && mi.Tag is ShellFile sf && InvokeContextMenu(sf, false, new ShellFolder(Path.GetDirectoryName(sf.Path), IntPtr.Zero)))
            e.Handled = true;
    }

    public bool InvokeContextMenu(ShellFile file, bool isInteractive, ShellFolder parentFolder)
    {
        if (file == null)
            return false;

        _ = new ShellItemContextMenu([file], parentFolder, IntPtr.Zero, HandleFileAction, isInteractive, false, new ShellMenuCommandBuilder(), GetFileCommandBuilder(file));
        return true;
    }

    private enum MenuItemId : uint
    {
        OpenParentFolder = CommonContextMenuItem.Paste + 1,
    }

    private static bool HandleFileAction(string action, ShellItem[] items, bool allFolders)
    {
        if (action == ((uint)MenuItemId.OpenParentFolder).ToString())
        {
            ShellHelper.StartProcess(items[0].Path);
            return true;
        }

        return false;
    }

    private ShellMenuCommandBuilder GetFileCommandBuilder(ShellFile file)
    {
        if (file == null)
        {
            return new ShellMenuCommandBuilder();
        }

        ShellMenuCommandBuilder builder = new();

        builder.AddSeparator();
        builder.AddCommand(new ShellMenuCommand()
        {
            Flags = MenuFlagsTypes.BYCOMMAND,
            Label = Constants.Localization.OPEN_FOLDER,
            UID = (uint)MenuItemId.OpenParentFolder,
        });

        return builder;
    }

    #region Drag'n Drop

    protected void DragMouseDown(object sender, MouseButtonEventArgs e)
    {
        _startDrag = true;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        StartDrag(e);
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
    }

    public void StopDrag()
    {
        _startDrag = false;
    }

    protected void DragMouseUp(object sender, MouseButtonEventArgs e)
    {
        StopDrag();
    }

    protected void OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data && data.GetDataPresent(typeof(ShellFile)))
        {
            ShellFile shellFile = (ShellFile)data.GetData(typeof(ShellFile));
            if (shellFile != base.DataContext)
            {
                (DataContext.Position, shellFile.Position) = (shellFile.Position, DataContext.Position);
                this.FindVisualParent<Toolbar>().DataContext.RefreshMyCollectionView();
            }
        }
    }

    protected void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data && data.GetFileDropList()?.Count > 0)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string toolbarPath = Path.GetDirectoryName(DataContext.Path);
                foreach (string file in data.GetFileDropList())
                {
                    if (Path.GetExtension(file) == ".lnk")
                        File.Copy(file, Path.Combine(toolbarPath, Path.GetFileName(file)));
                    else
                    {
                        Shortcut sc = Shortcut.CreateShortcut(file);
                        sc.WriteToFile(Path.Combine(toolbarPath, Path.GetFileNameWithoutExtension(file) + ".lnk"));
                    }
                }
            }
            else
            {
                StringBuilder sb = new();
                foreach (string file in data.GetFileDropList())
                {
                    if (sb.Length > 0)
                        sb.Append(' ');
                    sb.Append("\"" + file + "\"");
                }
                ShellHelper.StartProcess(DataContext.Path, sb.ToString());
            }
        }
    }

    private async Task StartDrag(MouseEventArgs e)
    {
        await Task.Delay(WindowsConstants.DelayIgnoreDrag);
        if (_startDrag)
        {
            DataObject data = new();
            data.SetData(DataContext);
            DragGhostAdorner.StartDragGhost(this, e);
            DragDrop.DoDragDrop(this, base.DataContext, DragDropEffects.Move);
            DragGhostAdorner.StopDragGhost();
            _startDrag = true;
        }
    }

    private void ToolbarBaseButton_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        DragGhostAdorner.UpdateDragGhost();
        e.UseDefaultCursors = false;
        e.Handled = true;
    }

    #endregion
}
