using System;
using System.Windows;
using System.Windows.Interop;

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

            DataContext = new ExplorerViewModel(_windowHandle);
        }

        public ExplorerViewModel MyDataContext
        {
            get { return (ExplorerViewModel)DataContext; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyDataContext.Refresh();
        }
    }
}
