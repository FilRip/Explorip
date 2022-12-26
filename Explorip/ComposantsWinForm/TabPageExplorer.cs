using System.Windows.Forms;

using Explorip.Localization;

namespace Explorip.ComposantsWinForm
{
    public class TabPageExplorer : TabPage
    {
        private readonly PanelExplorer _panelExplorer;

        public TabPageExplorer(string repertoire) : base()
        {
            _panelExplorer = new PanelExplorer();
            Controls.Add(_panelExplorer);
            this.Resize += TabExplorer_Resize;
            _panelExplorer.Initialise(repertoire);
            _panelExplorer.Repertoires.SelectionneRepertoire += Repertoires_SelectionneRepertoire;
            Text = System.Environment.SpecialFolder.MyComputer.NomTraduit();
        }

        private delegate void delegateChangeRepertoire(object sender, ExploripEventArgs.SelectionneRepertoireEventArgs e);
        private void Repertoires_SelectionneRepertoire(object sender, ExploripEventArgs.SelectionneRepertoireEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new delegateChangeRepertoire(Repertoires_SelectionneRepertoire), sender, e);
                return;
            }
            if (e.DirInfo != null)
                this.Text = e.DirInfo.Name;
            else
                Text = System.Environment.SpecialFolder.MyComputer.NomTraduit();
        }

        private void TabExplorer_Resize(object sender, System.EventArgs e)
        {
            if (_panelExplorer != null)
            {
                _panelExplorer.Width = Width;
                _panelExplorer.Height = Height;
            }
        }

        public void Initialise(string repertoire)
        {
            _panelExplorer.Initialise(repertoire);
        }
    }
}
