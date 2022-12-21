using System.Drawing;
using System.Windows.Forms;

using Explorip.ComposantsWinForm.FilRipListView;
using Explorip.Helpers;

namespace Explorip.Themes
{
    public static class AutoTheme
    {
        public static void AppliqueThemeWindows(Form form)
        {
            bool sombre = WindowsSettings.IsWindowsApplicationInDarkMode();
            ChangeThemeRecursif(form, sombre);
        }

        private static void ChangeThemeRecursif(Control control, bool sombre)
        {
            if ((sombre))
            {
                control.BackColor = Color.Black;
                if ((control.GetType().EstTypeOuSousTypeDe(typeof(Form))))
                {
                    control.ForeColor = Color.White;
                    control.BackColor = Color.Black;
                }
                else if ((control.GetType().EstTypeOuSousTypeDe(typeof(FilRipListView))))
                {
                    control.ForeColor = Color.White;
                    ((FilRipListView)control).CouleurAlternee1 = Color.Black;
                    ((FilRipListView)control).CouleurAlternee2 = Color.FromArgb(0x30, 0x30, 0x30);
                    foreach (FilRipColumnHeader col in ((FilRipListView)control).Columns)
                    {
                        col.ActiveCouleur = true;
                        col.CouleurArrierePlan = Color.Black;
                        col.CouleurTexte = Color.White;
                        if ((col.ActiveCouleurOk))
                        {
                            if ((col.CouleurOk == Color.Black))
                                col.CouleurOk = Color.White;
                            if ((col.CouleurSinon == Color.Black))
                                col.CouleurSinon = Color.White;
                        }
                    }
                }
                else if (((control.GetType().EstTypeOuSousTypeDe(typeof(ButtonBase))) || (control.GetType().EstTypeOuSousTypeDe(typeof(TabPage))) || (control.GetType().EstTypeOuSousTypeDe(typeof(GroupBox))) || (control.GetType().EstTypeOuSousTypeDe(typeof(Label)))))
                {
                    control.BackColor = Color.Black;
                    control.ForeColor = Color.White;
                }
                else if ((!(control.GetType().EstTypeOuSousTypeDe(typeof(ProgressBar)))))
                    control.ForeColor = Color.White;
            }
            else
            {
                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.WindowText;
                if ((control.GetType().EstTypeOuSousTypeDe(typeof(Form))))
                {
                    control.ForeColor = SystemColors.WindowText;
                    control.BackColor = SystemColors.Window;
                }
                else if ((control.GetType().EstTypeOuSousTypeDe(typeof(FilRipListView))))
                {
                    control.ForeColor = Color.Black;
                    ((FilRipListView)control).CouleurAlternee1 = Color.White;
                    ((FilRipListView)control).CouleurAlternee2 = Color.FromArgb(0xEF, 0xEF, 0xEF);
                    foreach (FilRipColumnHeader col in ((FilRipListView)control).Columns)
                    {
                        col.ActiveCouleur = false;
                        col.CouleurArrierePlan = SystemColors.Control;
                        col.CouleurTexte = SystemColors.ControlText;
                        if ((col.ActiveCouleurOk))
                        {
                            if ((col.CouleurOk == Color.White))
                                col.CouleurOk = Color.Black;
                            if ((col.CouleurSinon == Color.White))
                                col.CouleurSinon = Color.Black;
                        }
                    }
                }
                else if (((control.GetType().EstTypeOuSousTypeDe(typeof(ButtonBase))) || (control.GetType().EstTypeOuSousTypeDe(typeof(TabPage))) || (control.GetType().EstTypeOuSousTypeDe(typeof(GroupBox))) || (control.GetType().EstTypeOuSousTypeDe(typeof(Label)))))
                {
                    control.ForeColor = SystemColors.ControlText;
                    control.BackColor = SystemColors.Control;
                }
                else if ((control.GetType().EstTypeOuSousTypeDe(typeof(ProgressBar))))
                {
                    control.BackColor = SystemColors.Control;
                    control.ForeColor = Color.Red;
                }
            }
            if ((control.ContextMenuStrip != null))
                ChangeThemeMenu(control.ContextMenuStrip.Items, sombre);
            if ((control.HasChildren))
            {
                foreach (Control sousControle in control.Controls)
                    ChangeThemeRecursif(sousControle, sombre);
            }
        }

        private static void ChangeThemeMenu(ToolStripItemCollection sousMenu, bool sombre)
        {
            foreach (ToolStripItem item in sousMenu)
            {
                if ((sombre))
                {
                    item.BackColor = Color.FromArgb(60, 60, 60);
                    item.ForeColor = Color.White;
                }
                else
                {
                    item.BackColor = Color.White;
                    item.ForeColor = Color.Black;
                }
                if ((item is ToolStripMenuItem menuItem) && menuItem.HasDropDownItems)
                    ChangeThemeMenu(menuItem.DropDownItems, sombre);
            }
        }
    }
}
