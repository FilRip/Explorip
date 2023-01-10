using System;
using System.Windows.Forms;

using Explorip.ComposantsWinForm;
using Explorip.Helpers;
using Explorip.WinAPI;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Forms
{
    public partial class FormExplorerBrowser : Form
    {
        private readonly string _repertoireDemarrage;
        private TabExplorerBrowser _tabGauche, _tabDroite;

        public FormExplorerBrowser(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() != "explorer")
                _repertoireDemarrage = args[0];
            else if (args.Length > 1 && args[0].ToLower() == "explorer")
                _repertoireDemarrage = args[1];
            InitializeComponent();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(Handle, true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
                Themes.AutoTheme.AppliqueThemeWindows(this);
            }
            Icon = Properties.Resources.IconeExplorateur;
            MainSplitter.SplitterDistance = MainSplitter.Width / 2;
        }

        private void FormExplorerBrowser_Move(object sender, EventArgs e)
        {
            HideEditPath();
        }

        private void HideEditPath()
        {
            if (_tabGauche?.SelectedTab != null)
                ((TabPageExplorerBrowser)_tabGauche.SelectedTab).HideEditPath();
            if (_tabDroite?.SelectedTab != null)
                ((TabPageExplorerBrowser)_tabDroite.SelectedTab).HideEditPath();
        }

        private void FormExplorerBrowser_SizeChanged(object sender, EventArgs e)
        {
            HideEditPath();
        }

        private void FormExplorerBrowser_Shown(object sender, EventArgs e)
        {
            ShellObject sfDemarrage = (ShellObject)KnownFolders.Desktop;
            if (!string.IsNullOrWhiteSpace(_repertoireDemarrage))
            {
                sfDemarrage = ShellObject.FromParsingName(_repertoireDemarrage);
            }
            SuspendLayout();
            MainSplitter.SuspendLayout();
            _tabGauche = new TabExplorerBrowser(sfDemarrage, false);
            _tabDroite = new TabExplorerBrowser(sfDemarrage, true);
            MainSplitter.Panel1.Controls.Add(_tabGauche);
            MainSplitter.Panel2.Controls.Add(_tabDroite);
            _tabGauche.Dock = DockStyle.Fill;
            _tabDroite.Dock = DockStyle.Fill;
            MainSplitter.ResumeLayout(false);
            ResumeLayout(false);
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
                Themes.AutoTheme.AppliqueThemeWindows(this);
        }
    }
}
