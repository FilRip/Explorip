using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Explorip.ExploripEventArgs;
using Explorip.Helpers;
using Explorip.Localization;
using Explorip.Sorters;

namespace Explorip.ComposantsWinForm
{
    // https://www.codeproject.com/Articles/18811/Customizing-Folders-in-C
    public class DirectoryTreeView : TreeView
    {
        private ContextMenuStrip _cms;
        private readonly ImageList _listeIcones;
        private FichiersListView _liensFichiers;

        public delegate void DelegateCliqueRepertoire(object sender, SelectionneRepertoireEventArgs e);
        public event DelegateCliqueRepertoire SelectionneRepertoire;

        private TreeNode _noeudMyComputer;

        public DirectoryTreeView() : base()
        {
            _cms = new ContextMenuStrip();
            AfterExpand += TreeRepertoire_AfterExpand;
            AfterSelect += TreeRepertoire_AfterSelect;
            MouseUp += new MouseEventHandler(TreeMain_MouseUp);
            _listeIcones = new ImageList();
            ImageList = _listeIcones;
        }

        public ContextMenuStrip MenuContextuel
        {
            get { return _cms; }
            set { _cms = value; }
        }

        public FichiersListView LiensFichiers
        {
            get { return _liensFichiers; }
            set { _liensFichiers = value; }
        }

        public void Init(string path)
        {
            if (path == "Desktop")
            {
                // TODO : Nom localisé des répertoires connus, voir https://github.com/aybe/Windows-API-Code-Pack-1.1
                TreeNode rootDesktop = new TreeNode(Environment.SpecialFolder.Desktop.Localized(), 2, 2);
                Nodes.Add(rootDesktop);
                rootDesktop.Name = "Desktop";
                rootDesktop.Tag = new DirectoryInfo(Environment.SpecialFolder.Desktop.Repertoire());
                if ((Directory.GetFiles(Environment.SpecialFolder.Desktop.Repertoire()).Length > 0) || (Directory.GetDirectories(Environment.SpecialFolder.Desktop.Repertoire()).Length > 0))
                {
                    rootDesktop.Nodes.Add("");
                }
                _noeudMyComputer = new TreeNode(Environment.SpecialFolder.MyComputer.Localized(), 4, 4)
                {
                    Name = "My Computer"
                };
                Nodes.Add(_noeudMyComputer);
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    Icon iconeLecteur = Icones.GetFileIcon(drive.Name, false, true, false, true);
                    Bitmap imageIcone = iconeLecteur.ToBitmap();
                    iconeLecteur.Dispose();
                    imageIcone.MakeTransparent();
                    _listeIcones.Images.Add(imageIcone);
                    TreeNode driveNode = new TreeNode(drive.Name)
                    {
                        Name = drive.Name,
                        ImageIndex = _listeIcones.Images.Count - 1,
                        Tag = new DirectoryInfo(drive.Name)
                    };
                    // drive.DriveType
                    driveNode.Nodes.Add("");
                    _noeudMyComputer.Nodes.Add(driveNode);
                }
            }
            else
            {
                // TODO : Initialise un répertoire en ouvrant tous les parents
            }
        }

        private void TreeRepertoire_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != _noeudMyComputer)
                EnumerateDirectory(e.Node);
        }

        private void TreeRepertoire_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text != "My Computer")
            {
                if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(DirectoryInfo))
                {
                    DirectoryInfo dirInfo = (DirectoryInfo)e.Node.Tag;
                    if (SelectionneRepertoire != null)
                        SelectionneRepertoire.BeginInvoke(this, new SelectionneRepertoireEventArgs(dirInfo), null, null);
                    if (_liensFichiers != null) _liensFichiers.Rafraichir(dirInfo);
                    if (!e.Node.IsExpanded)
                    {
                        if (dirInfo.GetDirectories().Length == 0)
                            e.Node.Nodes.Clear();
                    }
                }
            }
        }

        private void TreeMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode noeudClicke;
                noeudClicke = this.GetNodeAt(e.X, e.Y);
                ShellContextMenu ctxMnu = new ShellContextMenu();
                DirectoryInfo[] dir = new DirectoryInfo[1];
                dir[0] = new DirectoryInfo(GetFolderPath(noeudClicke));
                ctxMnu.ShowContextMenu(dir, this.PointToScreen(new Point(e.X, e.Y)), _cms);
            }
        }

        private void EnumerateDirectory(TreeNode parentNode, bool msgbox = false)
        {
            DirectoryInfo dirInfo;
            string path = GetFolderPath(parentNode);
            parentNode.Nodes.Clear();

            dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] dirs;
            try
            {
                dirs = dirInfo.GetDirectories();
            }
            catch (Exception ex)
            {
                if (msgbox)
                    MessageBox.Show(this, ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Array.Sort(dirs, new TriAlphabetique());
            foreach (DirectoryInfo dirI in dirs)
            {
                TreeNode node = new TreeNode(dirI.Name, 6, 6)
                {
                    Name = dirI.Name
                };
                _listeIcones.Images.Add(Icones.GetFileIcon(dirI.FullName, dirI.IsShortcut(), true, true, true));
                int imgIndex = _listeIcones.Images.Count - 1;
                node.ImageIndex = imgIndex;
                node.SelectedImageIndex = imgIndex;
                node.Tag = dirI;
                try
                {
                    if (dirI.GetDirectories().Length > 0)
                        node.Nodes.Add("");
                }
                catch (Exception) { }
                parentNode.Nodes.Add(node);
            }
        }

        private static string GetFolderPath(TreeNode parentNode)
        {
            string path = "";
            string[] pathSplit = parentNode.FullPath.Split('\\');
            if (pathSplit[0] == "Desktop")
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
                for (int a = 1; a < pathSplit.Length; a++)
                {
                    if (pathSplit[a].Trim() != string.Empty)
                    {
                        path += pathSplit[a] + "\\";
                    }
                }
            }
            else if ((pathSplit.Length == 1) && (parentNode.Name == "My Computer"))
            {
                path = "My Computer";
            }
            else if ((pathSplit.Length == 1) && (parentNode.Name == "Desktop"))
            {
                path = Environment.SpecialFolder.Desktop.Repertoire();
            }
            else
            {
                for (int a = 1; a < pathSplit.Length; a++)
                {
                    if (pathSplit[a].Trim() != string.Empty)
                    {
                        path += pathSplit[a] + "\\";
                    }
                }

            }
            return path;
        }

        public void RafraichirRepertoire(string chemin)
        {
            // TODO : RafraichirRepertoire (apres suppression, ajout, renommer, etc...)
            if (!string.IsNullOrWhiteSpace(chemin))
            {
            }
        }
    }
}
