using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorip.Helpers
{
    public class DragDropHelper
    {
        private Form _formInterception;

        public static DragDropEffects effetDragDrop = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

        public DragDropHelper()
        {
            _formInterception = new Form();
            _formInterception.ShowInTaskbar = false;
            _formInterception.Text = "";
            _formInterception.Visible = false;
        }
    }
}
