using System.Windows.Forms;

namespace Explorip.ComposantsWinForm
{
    public partial class PanelExplorer : UserControl
    {
        public PanelExplorer()
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponent();
            lvFichiers.LiensRepertoires = TreeRepertoire;
            TreeRepertoire.LiensFichiers = lvFichiers;
            this.Resize += PanelExplorer_Resize;
        }

        private void PanelExplorer_Resize(object sender, System.EventArgs e)
        {
            splitContainer2.SplitterDistance = splitContainer2.Width / 2 - splitContainer2.Location.X * 4;
        }

        public void Initialise(string repertoire)
        {
            if (string.IsNullOrWhiteSpace(repertoire))
            {
                TreeRepertoire.Init("My Computer");
            }
            else
            {
                TreeNode noeud = TreeRepertoire.RafraichirRepertoire(new System.IO.DirectoryInfo(repertoire));
                TreeRepertoire.SelectedNode = noeud;
            }
        }

        public DirectoryTreeView Repertoires
        {
            get { return TreeRepertoire; }
        }

        public FichiersListView Fichiers
        {
            get { return lvFichiers; }
        }
    }
}
