using System;
using System.Windows.Controls;
using System.Windows.Markup;

using Explorip.Helpers;

using ManagedShell.Common.Helpers;

namespace Explorip.StartMenu.Window
{
    /// <summary>
    /// Logique d'interaction pour StartMenuWindow.xaml
    /// </summary>
    public partial class StartMenuWindow : System.Windows.Window
    {
        private readonly ContextMenu _cmUser, _cmStart;

        public StartMenuWindow()
        {
            InitializeComponent();

            string xaml = "<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:colors='clr-namespace:ExploripSharedCopy.Constants;assembly=ExploripSharedCopy'><StackPanel Margin=\"-20,0,0,0\" Background=\"{x:Static colors:Colors.BackgroundColorBrush}\"/></ItemsPanelTemplate>";
            ItemsPanelTemplate itp = XamlReader.Parse(xaml) as ItemsPanelTemplate;

            _cmUser = new()
            {
                Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
                ItemsPanel = itp,
            };
            _cmUser.AddEntry(Constants.Localization.LOCK, ShellHelper.Lock);
            _cmUser.AddEntry(Constants.Localization.DISCONNECT, ShellHelper.Logoff);

            _cmStart = new()
            {
                Foreground = ExploripSharedCopy.Constants.Colors.ForegroundColorBrush,
                Background = ExploripSharedCopy.Constants.Colors.BackgroundColorBrush,
                ItemsPanel = itp,
            };
            _cmStart.AddEntry(Constants.Localization.PUT_HYBERNATE, Hybernate);
            _cmStart.AddEntry(Constants.Localization.SHUTDOWN, Shutdown);
            _cmStart.AddEntry(Constants.Localization.RESTART, Restart);
        }

        private static void Hybernate()
        {
            ShellHelper.StartProcess("shutdown /h", hidden: true);
        }

        private static void Shutdown()
        {
            ShellHelper.StartProcess("shutdown /s /t 0", hidden: true);
        }

        private static void Restart()
        {
            ShellHelper.StartProcess("shutdown /r /t 0", hidden: true);
        }

        private void StartMenuWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ScrollViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScrollViewer)sender).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void ScrollViewer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScrollViewer)sender).VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void ParamButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShellHelper.ShowConfigPanel();
        }

        private void StopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _cmStart.IsOpen = true;
        }

        private void UserButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _cmUser.IsOpen = true;
        }
    }
}
