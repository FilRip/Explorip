namespace Explorip.ComposantsWinForm
{
    public class TabExplorer : FilRipTabControl.FilRipTabControl
    {
        public TabExplorer(string repertoire) : base()
        {
            this.TabPages.Add(new TabPageExplorer(repertoire));
        }

        public TabExplorer() : this("")
        {
        }

        public void Initialise(string repertoire)
        {
            ((TabPageExplorer)TabPages[0]).Initialise(repertoire);
        }
    }
}
