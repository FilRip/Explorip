using System;
using System.Diagnostics;
using System.Windows;

using Explorip.Explorer.WPF.ViewModels;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemConsoleCommand.xaml
    /// </summary>
    public partial class TabItemConsoleCommand : TabItemExplorip
    {
        private readonly Process _cmdProcess;

        public TabItemConsoleCommand() : base()
        {
            InitializeComponent();
            InitializeExplorip();

            _cmdProcess = Process.Start(new ProcessStartInfo("cmd")
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Console.OutputEncoding,
                StandardErrorEncoding = Console.OutputEncoding,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            _cmdProcess.OutputDataReceived += CmdProcess_OutputDataReceived;
            _cmdProcess.ErrorDataReceived += CmdProcess_ErrorDataReceived;
            _cmdProcess.BeginOutputReadLine();
            _cmdProcess.BeginErrorReadLine();
            _cmdProcess.Exited += CmdProcess_Exited;

            SetTitle("Console");
        }

        public TabItemConsoleCommandViewModel MyDataContext
        {
            get { return (TabItemConsoleCommandViewModel)DataContext; }
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => ConsoleCommand.Text += Environment.NewLine + "Console has exited");
        }

        private void CmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => ConsoleCommand.Text += e.Data + Environment.NewLine);
        }

        private void CmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => ConsoleCommand.Text += e.Data + Environment.NewLine);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                try
                {
                    _cmdProcess.Kill();
                }
                catch (Exception) { /* Ignore errors */ }
            }
        }

        private void ConsoleCommand_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
                _cmdProcess.StandardInput.Write((char)13);
            else
                _cmdProcess.StandardInput.Write(e.Key);
            e.Handled = true;
        }
    }
}
