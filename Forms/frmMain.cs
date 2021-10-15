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
            TreeRepertoire.Init("Desktop");
            TreeRepertoire.LiensFichiers = lvFichiers;
        }
    }
}