using System.Windows.Forms;

using Explorip.Helpers;

namespace Explorip.Forms
{
    public partial class FormExplorer : Form
    {
        public FormExplorer()
        {
            InitializeComponent();

            if (WindowsSettings.IsWindowsApplicationInDarkMode())
                WindowsSettings.UseImmersiveDarkMode(Handle, true);
            TreeRepertoire.Init("My Computer");
            TreeRepertoire.LiensFichiers = lvFichiers;
            lvFichiers.LiensRepertoires = TreeRepertoire;
        }
    }
}
