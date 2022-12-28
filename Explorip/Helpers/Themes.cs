using System.Drawing;
using System.Windows.Forms;

using Explorip.ComposantsWinForm;

namespace Explorip.Helpers
{
    public static class Themes
    {
        public static void ChangeThemeRecursif(Control control, bool sombre)
        {
            if (sombre)
            {
                control.BackColor = Color.Black;
                if (control is Form)
                {
                    control.ForeColor = Color.White;
                    control.BackColor = Color.Black;
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
                else if (control is TabExplorerBrowser tabControl)
                {
                    tabControl.DisplayStyleProvider.TextColorSelected = Color.White;
                    tabControl.DisplayStyleProvider.TextColor = Color.White;
                }
                else if (control is not ProgressBar)
                {
                    control.ForeColor = Color.White;
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
                else if (control is ButtonBase || control is TabPage || control is GroupBox || control is Label)
                {
                    control.ForeColor = SystemColors.ControlText;
                    control.BackColor = SystemColors.Control;
                }
                else if (control is ProgressBar)
                {
                    control.BackColor = SystemColors.Control;
                    control.ForeColor = Color.Red;
                }
                else if (control is TabExplorerBrowser tabControl)
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

        public static void ChangeThemeMenu(ToolStripItemCollection sousMenu, bool sombre)
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
