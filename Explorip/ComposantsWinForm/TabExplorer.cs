using System.Windows.Forms;

namespace Explorip.ComposantsWinForm
{
    public class TabExplorer : TabPage
    {
        private readonly PanelExplorer _panelExplorer;

        public TabExplorer(string repertoire) : base()
        {
            _panelExplorer = new PanelExplorer();
            Controls.Add(_panelExplorer);
            this.Resize += TabExplorer_Resize;
            _panelExplorer.Initialise(repertoire);
        }

        private void TabExplorer_Resize(object sender, System.EventArgs e)
        {
            if (_panelExplorer != null)
            {
                _panelExplorer.Width = Width;
                _panelExplorer.Height = Height;
            }
        }

        public TabExplorer() : this(null)
        {
        }
    }
}
