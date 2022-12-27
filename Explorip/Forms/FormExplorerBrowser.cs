using System;
using System.Drawing;
using System.Windows.Forms;

using Explorip.ComposantsWinForm.FilRipListView;
using Explorip.Helpers;
using Explorip.WinAPI;

using Microsoft.WindowsAPICodePack.Shell;

namespace Explorip.Forms
{
    public partial class FormExplorerBrowser : Form
    {
        private string _repertoireDemarrage;

        public FormExplorerBrowser(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() != "explorer")
                _repertoireDemarrage = args[0];
            else if (args.Length > 1 && args[0].ToLower() == "explorer")
                _repertoireDemarrage = args[1];
            InitializeComponent();
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                WindowsSettings.UseImmersiveDarkMode(this.Handle, true);
                Uxtheme.SetPreferredAppMode(Uxtheme.PreferredAppMode.APPMODE_ALLOWDARK);
                ChangeThemeRecursif(this, true);
            }
            ExplorerGauche.NavigationComplete += ExplorerGauche_NavigationComplete;
            ExplorerDroite.NavigationComplete += ExplorerDroite_NavigationComplete;
            TabGauche.TabClosing += TabGauche_TabClosing;
            TabDroite.TabClosing += TabDroite_TabClosing;
            Icon = Properties.Resources.IconeExplorateur;
            TabGauche.DisplayStyleProvider.BackgroundColor = WindowsSettings.GetWindowsAccentColor();
            TabDroite.DisplayStyleProvider.BackgroundColor = WindowsSettings.GetWindowsAccentColor();
        }

        private void TabDroite_TabClosing(object sender, TabControlCancelEventArgs e)
        {
            if (TabDroite.TabCount == 1)
                MainSplitter.Panel2Collapsed = true;
        }

        private void TabGauche_TabClosing(object sender, TabControlCancelEventArgs e)
        {
            if (TabGauche.TabCount == 1)
                e.Cancel = true;
        }

        private void ExplorerGauche_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            TabPageGauche.Text = e.NewLocation.Name;
        }

        private void ExplorerDroite_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            TabPageDroite.Text = e.NewLocation.Name;
        }

        private void FormExplorerBrowser_Shown(object sender, EventArgs e)
        {
            ShellObject sfDemarrage = (ShellObject)KnownFolders.Desktop;
            if (!string.IsNullOrWhiteSpace(_repertoireDemarrage))
            {
                sfDemarrage = ShellObject.FromParsingName(_repertoireDemarrage);
            }
            ExplorerGauche.Navigate(sfDemarrage);
            ExplorerDroite.Navigate((ShellObject)KnownFolders.Desktop);
        }

        private void ChangeThemeRecursif(Control control, bool sombre)
        {
            if (sombre)
            {
                control.BackColor = Color.Black;
                if (control is Form)
                {
                    control.ForeColor = Color.White;
                    control.BackColor = Color.Black;
                }
                else if (control is FilRipListView lv)
                {
                    control.ForeColor = Color.White;
                    lv.CouleurAlternee1 = Color.Black;
                    lv.CouleurAlternee2 = Color.FromArgb(0x30, 0x30, 0x30);
                    foreach (FilRipColumnHeader col in lv.Columns)
                    {
                        col.ActiveCouleur = true;
                        col.CouleurArrierePlan = Color.Black;
                        col.CouleurTexte = Color.White;
                        if (col.ActiveCouleurOk)
                        {
                            if (col.CouleurOk == Color.Black)
                                col.CouleurOk = Color.White;
                            if (col.CouleurSinon == Color.Black)
                                col.CouleurSinon = Color.White;
                        }
                    }
                }
                else if (control is Button)
                {
                    control.BackColor = Color.LightGray;
                    control.ForeColor = Color.Black;
                }
                else if (control is ButtonBase || control is TabPage || control is GroupBox || control is Label)
                {
                    control.BackColor = Color.Black;
                    control.ForeColor = Color.White;
                }
                else if (control is CustomTabControl tabControl)
                {
                    tabControl.DisplayStyleProvider.TextColorSelected = Color.White;
                    tabControl.DisplayStyleProvider.TextColor = Color.White;
                }
                else if (control is not ProgressBar)
                {
                    control.ForeColor = Color.White;
                    // ElseIf (control.GetType().EstTypeOuSousTypeDe(GetType(FilRipTabControl))) Then
                    // CType(control, FilRipTabControl).CouleurFondNonSelect = Color.Black
                    // CType(control, FilRipTabControl).CouleurFondSelect = Color.White
                    // CType(control, FilRipTabControl).CouleurTextNonSelect = Color.White
                    // CType(control, FilRipTabControl).CouleurTextSelect = Color.Black
                }
            }
            else
            {
                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.WindowText;
                if (control is Form)
                {
                    control.ForeColor = SystemColors.WindowText;
                    control.BackColor = SystemColors.Window;
                }
                else if (control is FilRipListView lv)
                {
                    control.ForeColor = Color.Black;
                    lv.CouleurAlternee1 = Color.White;
                    lv.CouleurAlternee2 = Color.FromArgb(0xEF, 0xEF, 0xEF);
                    foreach (FilRipColumnHeader col in lv.Columns)
                    {
                        col.ActiveCouleur = false;
                        col.CouleurArrierePlan = SystemColors.Control;
                        col.CouleurTexte = SystemColors.ControlText;
                        if (col.ActiveCouleurOk)
                        {
                            if (col.CouleurOk == Color.White)
                                col.CouleurOk = Color.Black;
                            if (col.CouleurSinon == Color.White)
                                col.CouleurSinon = Color.Black;
                        }
                    }
                }
                else if (control is ButtonBase || control is TabPage || control is GroupBox || control is Label)
                {
                    control.ForeColor = SystemColors.ControlText;
                    control.BackColor = SystemColors.Control;
                }
                else if (control is ProgressBar)
                {
                    control.BackColor = SystemColors.Control;
                    control.ForeColor = Color.Red;
                    // ElseIf (control.GetType().EstTypeOuSousTypeDe(GetType(FilRipTabControl))) Then
                    // CType(control, FilRipTabControl).CouleurFondNonSelect = Color.FromKnownColor(KnownColor.Control)
                    // CType(control, FilRipTabControl).CouleurFondSelect = Color.White
                    // CType(control, FilRipTabControl).CouleurTextNonSelect = Color.FromKnownColor(KnownColor.WindowText)
                    // CType(control, FilRipTabControl).CouleurTextSelect = Color.FromKnownColor(KnownColor.WindowText)
                }
                else if (control is CustomTabControl tabControl)
                {
                    tabControl.DisplayStyleProvider.TextColorSelected = Color.Black;
                    tabControl.DisplayStyleProvider.TextColor = Color.Black;
                }
            }

            if (control.ContextMenuStrip != null)
            {
                ChangeThemeMenu(control.ContextMenuStrip.Items, sombre);
            }

            if (control.HasChildren)
            {
                foreach (Control sousControle in control.Controls)
                    ChangeThemeRecursif(sousControle, sombre);
            }
        }

        private void ChangeThemeMenu(ToolStripItemCollection sousMenu, bool sombre)
        {
            foreach (ToolStripItem item in sousMenu)
            {
                if (sombre)
                {
                    item.BackColor = Color.FromArgb(60, 60, 60);
                    item.ForeColor = Color.White;
                }
                else
                {
                    item.BackColor = Color.White;
                    item.ForeColor = Color.Black;
                }

                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    ChangeThemeMenu(((ToolStripMenuItem)item).DropDownItems, sombre);
                }
            }
        }
    }
}
