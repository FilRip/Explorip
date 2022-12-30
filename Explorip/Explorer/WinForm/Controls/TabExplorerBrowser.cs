using System;
using System.Windows.Forms;

using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.ComposantsWinForm
{
    public partial class TabExplorerBrowser : CustomTabControl
    {
        private readonly bool _autoriseFermerDernierTab;

        public TabExplorerBrowser() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="repertoireDemarrage"></param>
        /// <param name="autoriseFermerDernierTab"></param>
        public TabExplorerBrowser(ShellObject repertoireDemarrage, bool autoriseFermerDernierTab) : this()
        {
            if (!DesignMode)
            {
                _autoriseFermerDernierTab = autoriseFermerDernierTab;
                ShowBorder = false;
                Margin = new Padding(0);
                TabPages.Add(NouvelOnglet(repertoireDemarrage));
                TabClosing += TabExplorerBrowser_TabClosing;
                MouseClick += TabExplorerBrowser_MouseClick;
                DisplayStyleProvider.BackgroundColor = WindowsSettings.GetWindowsAccentColor();
                DisplayStyleProvider.BackgroundEndColor = WindowsSettings.GetWindowsAccentColor();
                Themes.AutoTheme.ChangeThemeMenu(ContextMenuTab.Items, WindowsSettings.IsWindowsApplicationInDarkMode());
                AllowDrop = true;
            }
        }

        private void TabExplorerBrowser_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuTab.Show(Cursor.Position);
            }
        }

        private void TabExplorerBrowser_TabClosing(object sender, TabControlCancelEventArgs e)
        {
            if (!_autoriseFermerDernierTab && TabCount == 1)
                e.Cancel = true;
            else if (TabCount == 1)
                RetourneSplitContainer().Panel2Collapsed = true;
        }

        private void FermerTousLesOngletsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = TabCount - 1; i >= 0; i--)
                if (i != SelectedTab.TabIndex || _autoriseFermerDernierTab)
                    TabPages.RemoveAt(i);
            if (_autoriseFermerDernierTab)
                RetourneSplitContainer().Panel2Collapsed = true;
        }

        private void OuvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellObject repDemarrage;
            TabPage tab = SelectedTab;
            repDemarrage = ((TabPageExplorerBrowser)tab).RepertoireCourant;
            tab = NouvelOnglet(repDemarrage);
            if (_autoriseFermerDernierTab)
            {
                ((TabExplorerBrowser)RetourneSplitContainer().Panel1.Controls[0]).TabPages.Add(tab);
                ((TabExplorerBrowser)RetourneSplitContainer().Panel1.Controls[0]).ForceSelectedTab(((TabExplorerBrowser)RetourneSplitContainer().Panel1.Controls[0]).TabCount - 1);
            }
            else
            {
                RetourneSplitContainer().Panel2Collapsed = false;
                ((TabExplorerBrowser)RetourneSplitContainer().Panel2.Controls[0]).TabPages.Add(tab);
                ((TabExplorerBrowser)RetourneSplitContainer().Panel2.Controls[0]).ForceSelectedTab(((TabExplorerBrowser)RetourneSplitContainer().Panel2.Controls[0]).TabCount - 1);
            }
        }

        private SplitContainer RetourneSplitContainer()
        {
            return (SplitContainer)Parent.Parent;
        }

        private TabPageExplorerBrowser NouvelOnglet(ShellObject repDemarrage)
        {
            TabPageExplorerBrowser newTab = new(repDemarrage);
            newTab.Margin = new Padding(0);
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
                Themes.AutoTheme.ChangeThemeRecursif(newTab, true);
            return newTab;
        }

        private void OuvrirUnNouvelOngletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellObject repDemarrage;
            TabPageExplorerBrowser tab = (TabPageExplorerBrowser)SelectedTab;
            repDemarrage = tab.RepertoireCourant;
            tab = NouvelOnglet(repDemarrage);
            TabPages.Add(tab);
            ForceSelectedTab(TabCount - 1);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (drgevent.Data != null && drgevent.Data.GetData("FileDrop") != null)
            {
                if (ActiveTab != null && ActiveTab != SelectedTab)
                    SelectedTab = ActiveTab;
            }
            base.OnDragOver(drgevent);
        }
    }
}
