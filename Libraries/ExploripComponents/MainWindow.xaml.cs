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
        private OneDirectory? _lastSelected;

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
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (OneFileSystem item in FileLV.Items)
                {
                    ListViewItem container = (ListViewItem)FileLV.ItemContainerGenerator.ContainerFromItem(item);
                    if (container != null && container.DataContext is OneFileSystem file)
                        file.IsItemVisible = IsElementInViewport(container);
                }
            });
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

        #region Manage blank TreeViewItem

        private bool _currentlyChangingSelection = false;
        private void FolderTV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_currentlyChangingSelection)
                return;

            if (e.Source is TreeView tv &&
                tv.SelectedItem is OneDirectory dir)
            {
                if (string.IsNullOrWhiteSpace(dir.DisplayText))
                {
                    _currentlyChangingSelection = true;
                    dir.IsSelected = false;
                    _currentlyChangingSelection = false;
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (_lastSelected != null)
                            _lastSelected.IsSelected = true;
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                    _lastSelected = dir;
            }
        }

        #endregion
    }
}
