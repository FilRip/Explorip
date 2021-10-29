using System.Drawing;
using System.Windows.Forms;

namespace Explorip.ComposantsWinForm.ContextMenu
{
    /// <summary>
    /// Séparateur utilisé dans les menus contextuels mais avec support des couleurs de fond et d'écriture
    /// </summary>
    public class FilRipToolStripSeparator : ToolStripSeparator
    {
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FilRipToolStripSeparator()
        {
            this.Paint += MonToolStripSeparator_Paint;
        }

        private void MonToolStripSeparator_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            e.Graphics.DrawLine(new Pen(ForeColor), 4, Height / 2, Width - 4, Height / 2);
        }
    }
}
