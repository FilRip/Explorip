using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Explorip.Helpers;
using Explorip.TaskBar.ViewModels;

using ManagedShell.Common.Helpers;
using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        private enum MenuItem : uint
        {
            OpenParentFolder = CommonContextMenuItem.Paste + 1
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
        }

        private void SetupFolder(string path)
        {
            Folder?.Dispose();
            Folder = new ShellFolder(Environment.ExpandEnvironmentVariables(path), IntPtr.Zero, true);
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

            _ = Mouse.Capture(null);

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

            _ = new ShellItemContextMenu(new ShellItem[] { file }, Folder, IntPtr.Zero, HandleFileAction, isInteractive, false, new ShellMenuCommandBuilder(), GetFileCommandBuilder(file));
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

        private double _startX/*, _startY*/;
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!TaskbarViewModel.Instance.ResizeOn)
                return;

            Grid myGrid = this.FindParent<Grid>();
            _startX = Mouse.GetPosition(myGrid).X - ((TranslateTransform)RenderTransform).X;
            //_startY = Mouse.GetPosition(QuickLaunchGrid).Y - ((TranslateTransform)QuickLaunchToolbar.RenderTransform).Y;

            Mouse.OverrideCursor = Cursors.Cross;
            CaptureMouse();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMouseCaptured)
                return;

            Grid myGrid = this.FindParent<Grid>();
            ((TranslateTransform)RenderTransform).X = Math.Max(0, Mouse.GetPosition(myGrid).X - _startX);
            //((TranslateTransform)QuickLaunchToolbar.RenderTransform).Y = Mouse.GetPosition(QuickLaunchGrid).Y - _startY;
            HitTestResult result = VisualTreeHelper.HitTest(myGrid, e.GetPosition(myGrid));
            if (result?.VisualHit != null)
            {
                Toolbar parent = result.VisualHit.FindParent<Toolbar>();
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
        }

        #endregion
    }
}
