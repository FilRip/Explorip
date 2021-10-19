using System.Windows.Forms;

using Explorip.Helpers;

namespace Explorip.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
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