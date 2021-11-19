using System;
using System.Windows.Forms;

using Explorip.Helpers;

namespace Explorip.Forms
{
    public partial class FormExplorer : Form
    {
        public FormExplorer(string[] args)
        {
            InitializeComponent();

            if (WindowsSettings.IsWindowsApplicationInDarkMode())
                WindowsSettings.UseImmersiveDarkMode(Handle, true);
            string repertoire = "";
            if (args.Length > 0)
                repertoire = args[0];
            tabExplorer1.Initialise(repertoire);
            tabExplorer2.Initialise("");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Themes.AutoTheme.AppliqueThemeWindows(this);
        }
    }
}
