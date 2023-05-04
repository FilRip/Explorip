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
            OnSelecting += TabItemConsoleCommand_OnSelecting;
            OnDeSelecting += TabItemConsoleCommand_OnDeSelecting;
        }

        private void TabItemConsoleCommand_OnDeSelecting()
        {
            MyConsoleControl.Hide();
        }

        private void TabItemConsoleCommand_OnSelecting()
        {
            MyConsoleControl.Show();
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
    }
}
