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
        private readonly IntPtr _windowHandle;

        public MainWindow()
        {
            InitializeComponent();

            _windowHandle = new WindowInteropHelper(this).EnsureHandle();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(_windowHandle, true);
                _ = Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
            }

            DataContext = new WpfExplorerViewModel(_windowHandle, this);
        }

        public WpfExplorerViewModel MyDataContext
        {
            get { return (WpfExplorerViewModel)DataContext; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyDataContext.Refresh();
        }

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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private void Scroll_PreviewDragOver(object sender, DragEventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
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

        private void FileLV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            foreach (OneFileSystem item in FileLV.Items)
            {
                ListViewItem container = (ListViewItem)FileLV.ItemContainerGenerator.ContainerFromItem(item);
                if (container != null && container.DataContext is OneFileSystem file)
                    file.IsItemVisible = IsElementInViewport(container);
            }
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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            if (sender is ListViewItem item && item.DataContext is OneFileSystem file)
                file.DoubleClickFileCommand.Execute(null);
        }
    }
}
