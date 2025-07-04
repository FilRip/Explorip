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

    public bool DisableFolderPreview { get; set; }

    public ToolbarBaseButton() : base()
    {
        PreviewMouseDown += DragMouseDown;
        PreviewMouseUp += DragMouseUp;
        DragEnter += OnDragEnter;
        Drop += OnDrop;
        MouseEnter += ToolbarBaseButton_MouseEnter;
        MouseLeave += ToolbarBaseButton_MouseLeave;
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
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (!_isFolder.HasValue && DataContext is ShellFile sf)
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
                Popup myPopup = this.FindVisualParent<Taskbar>().MyPopup;
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
        ((ItemsControl)((Border)this.FindVisualParent<Taskbar>().MyPopup.Child).Child).Items.Add(mi);
        if (Path.GetExtension(item.FileName) == ".lnk")
        {
            try
            {
                Shortcut sc = Shortcut.ReadFromFile(item.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    ShellFolder sf = new(newPath, IntPtr.Zero);
                    mi.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
                    ExpandFolder(mi, sf);
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }
        else if (item.IsFolder)
        {
            mi.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
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
                subMenu.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
                ExpandFolder(subMenu, new ShellFolder(sf.Path, IntPtr.Zero), nbRecursive++);
            }
            else if (Path.GetExtension(sf.FileName) == ".lnk")
            {
                Shortcut sc = Shortcut.ReadFromFile(sf.Path);
                string newPath = sc.Target;
                if (!string.IsNullOrWhiteSpace(newPath) && Directory.Exists(newPath))
                {
                    mi.Style = (Style)Application.Current.FindResource("MenuItemWithSubMenuStyle");
                    ExpandFolder(subMenu, new ShellFolder(newPath, IntPtr.Zero), nbRecursive++);
                }
            }
        }
    }

    private MenuItem CreateMenuItem(ShellFile item)
    {
        item.AllowAsync = false;
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
        {
            return false;
        }

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
            Flags = MFT.BYCOMMAND,
            Label = Constants.Localization.OPEN_FOLDER,
            UID = (uint)MenuItemId.OpenParentFolder,
        });

        return builder;
    }

    protected void DragMouseDown(object sender, MouseButtonEventArgs e)
    {
        _startDrag = true;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        StartDrag();
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
    }

    protected void DragMouseUp(object sender, MouseButtonEventArgs e)
    {
        _startDrag = false;
    }

    public ShellFile MyDataContext
    {
        get { return (ShellFile)DataContext; }
    }

    protected void OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data && data.GetDataPresent(typeof(ShellFile)))
        {
            ShellFile shellFile = (ShellFile)data.GetData(typeof(ShellFile));
            if (shellFile != DataContext)
            {
                (MyDataContext.Position, shellFile.Position) = (shellFile.Position, MyDataContext.Position);
                this.FindVisualParent<Toolbar>().MyDataContext.RefreshMyCollectionView();
            }
        }
    }

    protected void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data is DataObject data && data.GetFileDropList()?.Count > 0)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string toolbarPath = Path.GetDirectoryName(MyDataContext.Path);
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
                ShellHelper.StartProcess(MyDataContext.Path, sb.ToString());
            }
        }
    }

    private async Task StartDrag()
    {
        await Task.Delay(WindowsConstants.DelayIgnoreDrag);
        if (_startDrag)
        {
            DataObject data = new();
            data.SetData(MyDataContext);
            DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
            _startDrag = true;
        }
    }
}
