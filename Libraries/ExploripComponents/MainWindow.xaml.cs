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

            DataContext = new WpfExplorerViewModel(_windowHandle);
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
            if (r.VisualHit is not FrameworkElement || ((FrameworkElement)r.VisualHit).DataContext is not OneFile)
                FileLV.UnselectAll();
        }
    }
}
