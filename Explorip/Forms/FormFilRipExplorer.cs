using System;
using System.Windows.Forms;

using Explorip.Helpers;

namespace Explorip.Forms
{
    public partial class FormFilRipExplorer : Form
    {
        public FormFilRipExplorer(string[] args)
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponent();

            if (WindowsSettings.IsWindowsApplicationInDarkMode())
                WindowsSettings.UseImmersiveDarkMode(Handle, true);
            string repertoire = "";
            if (args.Length > 0 && args[0].ToLower() != "explorer")
                repertoire = args[0];
            else if (args.Length > 1 && args[0].ToLower() == "explorer")
                repertoire = args[1];
            tabExplorer1.Initialise(repertoire);
            tabExplorer2.Initialise(repertoire);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Themes.AutoTheme.AppliqueThemeWindows(this);
        }
    }
}
