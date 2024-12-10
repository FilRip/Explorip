using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using ExploripSharedCopy.Helpers;
using ExploripSharedCopy.WinAPI;

// TODO : Hightlight when drag over https://stackoverflow.com/questions/44731343/change-button-image-on-mouse-over-during-drag-and-drop-operation

namespace ExploripComponents
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly IntPtr _windowHandle;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            _windowHandle = new WindowInteropHelper(this).EnsureHandle();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(_windowHandle, true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }

            DataContext = new WpfExplorerViewModel(this);
        }

        #endregion

        #region Properties

        public WpfExplorerViewModel MyDataContext
        {
            get { return (WpfExplorerViewModel)DataContext; }
        }

        public IntPtr Handle
        {
            get { return _windowHandle; }
        }

        #endregion

        #region Manage selected items from ListView

        private void FileLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyDataContext.SelectedItems = new ObservableCollection<OneFileSystem>(FileLV.SelectedItems.OfType<OneFileSystem>());
        }

        private void FileLV_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit is not FrameworkElement || ((FrameworkElement)r.VisualHit).DataContext is not OneFileSystem)
                FileLV.UnselectAll();
        }

        #endregion

        #region Drag'n Drop

        private void TreeItemGrid_Drop(object sender, DragEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit is FrameworkElement element && element.DataContext is OneDirectory dir)
                dir.Drop(sender, e);
        }

        private void FileViewStackPanel_Drop(object sender, DragEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit is FrameworkElement element && element.DataContext is OneFileSystem fs)
                fs.Drop(sender, e);
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            MyDataContext.SelectedFolder?.Drop(sender, e);
        }

        private void AllowDrop_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                MyDataContext.DragDropKeyStates = e.KeyStates;
                DataObject data = (DataObject)e.Data;
                if (data.GetFileDropList().Count == 1)
                {
                    HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                    if (r.VisualHit is FrameworkElement element && element.DataContext is OneFileSystem fs && fs.FullPath == data.GetFileDropList()[0])
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                }
            }
        }

        private void AllowDrop_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (!MyDataContext.CurrentlyDraging)
                MyDataContext.CurrentlyDraging = true;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private void Scroll_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (sender is not FrameworkElement control)
                return;
            ScrollViewer? scrollViewer = control.FindVisualChild<ScrollViewer>();
            if (scrollViewer == null)
                return;

            double verticalPos = e.GetPosition(control).Y;
            double offset = 32;
            double offsetChange;

            if (verticalPos < offset)
            {
                offsetChange = offset - verticalPos;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offsetChange);
            }
            else if (verticalPos > control.ActualHeight - offset)
            {
                offsetChange = verticalPos - (control.ActualHeight - offset);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offsetChange);
            }
        }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        #endregion

        #region Refresh visible item after scrolling

        public void ForceRefreshVisibleItems()
        {
            foreach (OneFileSystem item in FileLV.Items)
            {
                ListViewItem container = (ListViewItem)FileLV.ItemContainerGenerator.ContainerFromItem(item);
                if (container != null && container.DataContext is OneFileSystem file)
                    file.IsItemVisible = IsElementInViewport(container);
            }
        }

        private void FileLV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ForceRefreshVisibleItems();
        }

        private bool IsElementInViewport(FrameworkElement element)
        {
            if (element != null)
            {
                ListViewItem container = (ListViewItem)element;
                GeneralTransform transform = container.TransformToAncestor(FileLV);
                Rect bounds = transform.TransformBounds(new Rect(new Point(0, 0), container.RenderSize));
                Rect viewport = new(0, 0, FileLV.ActualWidth, FileLV.ActualHeight);
                return viewport.IntersectsWith(bounds);
            }
            return false;
        }

        #endregion

        #region Double click action on item in ListView

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is OneFileSystem file)
                file.DoubleClickFileCommand.Execute(null);
        }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        #endregion

        #region auto focus on item name when rename it

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private void EditBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox editName = (TextBox)sender;
            if (editName.Visibility == Visibility.Visible)
            {
                editName.Focus();
                editName.SelectAll();
            }
        }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        #endregion

        #region Rectangle selection

        private void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FileLV.DrawSelection && SelectInRectangle())
                return;

            if (Mouse.DirectlyOver is FrameworkElement element && element.DataContext is OneFileSystem)
                return;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;
            FileLV.UnselectAll();
        }

        private bool SelectInRectangle()
        {
            FileLV.DrawSelection = false;
            FileLV.InvalidateVisual();
            Rect rect = new(FileLV.DrawSelectionStart, Mouse.GetPosition(FileLV));
            if (rect.Width == 0 && rect.Height == 0)
                return false;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                FileLV.UnselectAll();
            foreach (OneFileSystem item in MyDataContext.FileListView.Where(i => i.IsItemVisible))
            {
                if (FileLV.ItemContainerGenerator.ContainerFromItem(item) is Control control)
                {
                    GeneralTransform transform = control.TransformToAncestor(FileLV);
                    Rect bounds = transform.TransformBounds(new Rect(new Point(0, 0), control.RenderSize));
                    if (rect.IntersectsWith(bounds))
                        FileLV.SelectedItems.Add(item);
                }
            }
            return true;
        }

        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (FileLV.DrawSelection)
                FileLV.InvalidateVisual();
        }

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Mouse.DirectlyOver is not FrameworkElement element || element.DataContext is not OneFileSystem) && !FileLV.DrawSelection)
            {
                FileLV.DrawSelection = true;
                FileLV.DrawSelectionStart = e.GetPosition(FileLV);
            }
        }

        #endregion

        public event EventHandler? EndRedraw;

        private void FileLV_LayoutUpdated(object sender, EventArgs e)
        {
            if (MyDataContext != null &&
                FileLV.Items.Count > 0 &&
                FileLV.ItemContainerGenerator.ContainerFromIndex(FileLV.Items.Count - 1) is ListViewItem)
            {
                EndRedraw?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
