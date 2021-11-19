using System;
using System.Windows.Forms;

using Explorip.ComposantsWinForm;
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
            TreeRepertoire.Init("My Computer");
            if (args.Length > 0)
                TreeRepertoire.RafraichirRepertoire(new System.IO.DirectoryInfo(args[0]));
            TreeRepertoire.LiensFichiers = lvFichiers;
            lvFichiers.LiensRepertoires = TreeRepertoire;
            filRipTabControl1.TabPages.Add(new TabPageExplorer());
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Themes.AutoTheme.AppliqueThemeWindows(this);
        }
    }
}
