using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using Explorip.Explorer.WPF.ViewModels;

namespace Explorip.Explorer.WPF.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemConsoleCommand.xaml
    /// </summary>
    public partial class TabItemConsoleCommand : TabItemExplorip
    {
        public TabItemConsoleCommand(string commandLine) : base()
        {
            InitializeComponent();
            InitializeExplorip();

            SetTitle("Console");
            ProcessStartInfo processStartInfo = new()
            {
                StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage),
                StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage),
                FileName = commandLine,
            };
            MyConsoleControl.StartProcess(processStartInfo);
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
    }
}
