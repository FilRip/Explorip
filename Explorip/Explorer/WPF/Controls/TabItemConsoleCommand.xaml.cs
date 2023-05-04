using System;
using System.Threading;
using System.Windows;

using Explorip.Explorer.WPF.ViewModels;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemConsoleCommand.xaml
    /// </summary>
    public partial class TabItemConsoleCommand : TabItemExplorip
    {
        private readonly string _commandLine;

        public TabItemConsoleCommand(string commandLine) : base()
        {
            InitializeComponent();
            InitializeExplorip();
            _commandLine = commandLine;
        }

        public TabItemConsoleCommandViewModel MyDataContext
        {
            get { return (TabItemConsoleCommandViewModel)DataContext; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                try
                {
                    MyConsoleControl.Dispose();
                }
                catch (Exception) { /* Ignore errors */ }
            }
        }

        private void TabItemExplorip_Loaded(object sender, RoutedEventArgs e)
        {
            SetTitle("Console");
            MyConsoleControl.StartProcess(_commandLine);
        }

        private void TabItemExplorip_OnSelecting()
        {
            MyConsoleControl.Show();
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Thread.Sleep(100);
                MyConsoleControl.SetFocus();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void TabItemExplorip_OnDeSelecting()
        {
            MyConsoleControl.Hide();
        }
    }
}
