using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ExploripConfig.Configuration;

using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

namespace Explorip.TaskBar.Controls;

/// <summary>
/// Interaction logic for Toolbar.xaml
/// </summary>
public partial class Toolbar : UserControl
{
    private enum MenuItem : uint
    {
        OpenParentFolder = CommonContextMenuItem.Paste + 1,
    }

    public readonly static DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(Toolbar), new PropertyMetadata(OnPathChanged));

    public string Path
    {
        get => (string)GetValue(PathProperty);
        set
        {
            SetValue(PathProperty, value);
            SetupFolder(value);
        }
    }

    private static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder", typeof(ShellFolder), typeof(Toolbar));

    private ShellFolder Folder
    {
        get => (ShellFolder)GetValue(FolderProperty);
        set
        {
            SetValue(FolderProperty, value);
            SetItemsSource();
        }
    }

    public Toolbar()
    {
        InitializeComponent();
        RenderTransform = new TranslateTransform();
    }

    public TranslateTransform MyRenderTransform
    {
        get { return (TranslateTransform)RenderTransform; }
    }

    private void SetupFolder(string path)
    {
        Folder?.Dispose();
        if (Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
        {
            Folder = new ShellFolder(Environment.ExpandEnvironmentVariables(path), IntPtr.Zero, true);
            Title.Content = Folder.DisplayName;
            if (!ConfigManager.ToolbarSmallSizeIcon(Path) && !CurrentShowLargeIcon)
                ShowLargeIcon_Click(null, null);
            Title.Visibility = ConfigManager.ToolbarShowTitle(Path) ? Visibility.Visible : Visibility.Collapsed;
            Point point = ConfigManager.ToolbarPosition(Path);
            MyRenderTransform.X = point.X;
            MyRenderTransform.Y = point.Y;
        }
    }

    private void UnloadFolder()
    {
        try
        {
            Folder?.Dispose();
        }
        catch (Exception) { /* Ignore errors */ }
        Folder = null;
    }

    private void SetItemsSource()
    {
        if (Folder != null)
        {
            ToolbarItems.ItemsSource = Folder.Files;
        }
    }

    #region Events
    private static void OnPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Toolbar toolbar)
        {
            toolbar.SetupFolder((string)e.NewValue);
        }
    }

    private void ToolbarIcon_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ToolbarButton icon)
        {
            return;
        }

        Mouse.Capture(null);

        if (icon.DataContext is not ShellFile file || string.IsNullOrWhiteSpace(file.Path))
        {
            return;
        }

        if (InvokeContextMenu(file, false))
        {
            e.Handled = true;
        }
    }

    private void ToolbarIcon_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ToolbarButton icon)
        {
            return;
        }

        ShellFile file = icon.DataContext as ShellFile;

        if (InvokeContextMenu(file, true))
        {
            e.Handled = true;
        }
    }

    private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool visible && e.NewValue != e.OldValue)
        {
            if (visible)
            {
                if (Folder != null)
                {
                    return;
                }

                SetupFolder(Path);
            }
            else
            {
                UnloadFolder();
            }
        }
    }
    #endregion

    #region Context menu
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
            Label = (string)FindResource("open_folder"),
            UID = (uint)MenuItem.OpenParentFolder
        });

        return builder;
    }

    private bool InvokeContextMenu(ShellFile file, bool isInteractive)
    {
        if (file == null)
        {
            return false;
        }

        _ = new ShellItemContextMenu([file], Folder, IntPtr.Zero, HandleFileAction, isInteractive, false, new ShellMenuCommandBuilder(), GetFileCommandBuilder(file));
        return true;
    }

    private bool HandleFileAction(string action, ShellItem[] items, bool allFolders)
    {
        if (action == ((uint)MenuItem.OpenParentFolder).ToString())
        {
            ManagedShell.Common.Helpers.ShellHelper.StartProcess(Folder.Path);
            return true;
        }

        return false;
    }
    #endregion

    #region Move toolbar in taskbar by drag'n drop

    private double _startX, _startY;
    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!TaskbarViewModel.Instance.ResizeOn)
            return;

        Grid myGrid = this.FindVisualParent<Grid>();
        _startX = Mouse.GetPosition(myGrid).X - MyRenderTransform.X;
        _startY = Mouse.GetPosition(myGrid).Y - MyRenderTransform.Y;

        Mouse.OverrideCursor = Cursors.ScrollAll;
        CaptureMouse();
    }

    private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
    {
        ReleaseMouseCapture();
        Mouse.OverrideCursor = null;
        ConfigManager.ToolbarPosition(Path, new Point(MyRenderTransform.X, MyRenderTransform.Y));
    }

    private void UserControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (!IsMouseCaptured)
            return;

        Grid myGrid = this.FindVisualParent<Grid>();
        MyRenderTransform.X = Math.Max(0, Mouse.GetPosition(myGrid).X - _startX);
        HitTestResult result = VisualTreeHelper.HitTest(myGrid, e.GetPosition(myGrid));
        if (result?.VisualHit != null)
        {
            if (MyRenderTransform.X == 0)
            {
                Toolbar parent = result.VisualHit.FindVisualParent<Toolbar>();
                if (parent != null)
                {
                    int previousRow = Grid.GetRow(this);
                    int newRow = Grid.GetRow(parent);
                    if (previousRow != newRow)
                    {
                        Grid.SetRow(parent, previousRow);
                        Grid.SetRow(this, newRow);
                    }
                }
            }
            else
            {
                MyRenderTransform.Y = Mouse.GetPosition(myGrid).Y - _startY;
                Toolbar parent = result.VisualHit.FindVisualParent<Toolbar>();
                if (parent != null)
                {
                    int previousRow = Grid.GetRow(this);
                    int newRow = Grid.GetRow(parent);
                    if (previousRow != newRow)
                    {
                        int previousColumn = Grid.GetColumn(this);
                        int newColumn = Grid.GetColumn(parent);
                        if (previousColumn == newColumn)
                        {
                            Grid.SetRow(this, newRow);
                            myGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            Grid.SetColumn(this, myGrid.ColumnDefinitions.Count - 1);
                            MyRenderTransform.X = 0;
                            _startX = 0;
                        }
                        else
                        {
                            Grid.SetColumn(this, newColumn);
                            Grid.SetColumn(parent, previousColumn);
                        }
                    }
                }
            }
        }
    }

    #endregion

    public void ShowHideTitle_Click(object sender, RoutedEventArgs e)
    {
        if (Title.Visibility == Visibility.Visible)
            Title.Visibility = Visibility.Collapsed;
        else
            Title.Visibility = Visibility.Visible;
        ConfigManager.ToolbarShowTitle(Path, Title.Visibility == Visibility.Visible);
    }

    public bool CurrentShowLargeIcon { get; private set; }

    public void ShowLargeIcon_Click(object sender, RoutedEventArgs e)
    {
        CurrentShowLargeIcon = !CurrentShowLargeIcon;
        DataTemplateSelector dts = ToolbarItems.ItemTemplateSelector;
        ToolbarItems.ItemTemplateSelector = null;
        ToolbarItems.ItemTemplateSelector = dts;
        Taskbar parentTaskbar = this.FindVisualParent<Taskbar>();
        if (parentTaskbar != null)
        {
            double newHeight = parentTaskbar.Height;
            if (CurrentShowLargeIcon)
                newHeight += 16;
            else
                newHeight -= 16;
            parentTaskbar.ChangeDesiredSize(newHeight, parentTaskbar.Width);
        }
        ConfigManager.ToolbarSmallSizeIcon(Path, !CurrentShowLargeIcon);
    }
}

public class IconFileDataTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        FrameworkElement element = container as FrameworkElement;
        if (container.FindVisualParent<Toolbar>().CurrentShowLargeIcon)
            return (DataTemplate)element.FindResource("LargeIconTemplate");
        else
            return (DataTemplate)element.FindResource("SmallIconTemplate");
    }
}
