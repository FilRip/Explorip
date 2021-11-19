using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Explorip.ExploripEventArgs;
using Explorip.Helpers;
using Explorip.Localization;
using Explorip.Sorters;
using Explorip.WinAPI;

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
            Nodes.Clear();
            if (path == "My Computer")
            {
                _noeudMyComputer = new TreeNode(Environment.SpecialFolder.MyComputer.NomTraduit())
                {
                    Name = "My Computer"
                };
                IntPtr tempPidl = IntPtr.Zero;
                Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DRIVES, ref tempPidl);
                _listeIcones.Images.Add(Icones.GetIcone(tempPidl, true));
                _noeudMyComputer.ImageIndex = _listeIcones.Images.Count - 1;
                _noeudMyComputer.SelectedImageIndex = _listeIcones.Images.Count - 1;
                _noeudMyComputer.Tag = "My Computer";
                Nodes.Add(_noeudMyComputer);

                // TODO : voir https://github.com/aybe/Windows-API-Code-Pack-1.1
                TreeNode noeudDesktop = new TreeNode(Environment.SpecialFolder.Desktop.NomTraduit(), 2, 2);
                _noeudMyComputer.Nodes.Add(noeudDesktop);
                noeudDesktop.Name = "Desktop";
                noeudDesktop.Tag = new DirectoryInfo(Environment.SpecialFolder.Desktop.Repertoire());
                _listeIcones.Images.Add(Icones.GetFileIcon(Environment.SpecialFolder.Desktop.Repertoire(), false, true, Shell32.SHIL.SMALL));
                noeudDesktop.ImageIndex = _listeIcones.Images.Count - 1;
                noeudDesktop.SelectedImageIndex = _listeIcones.Images.Count - 1;
                if ((Directory.GetFiles(Environment.SpecialFolder.Desktop.Repertoire()).Length > 0) || (Directory.GetDirectories(Environment.SpecialFolder.Desktop.Repertoire()).Length > 0))
                {
                    noeudDesktop.Nodes.Add("");
                }

                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyDocuments.GetTreeNode(_listeIcones));
                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyPictures.GetTreeNode(_listeIcones));
                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyMusic.GetTreeNode(_listeIcones));
                Shell32.SHGetKnownFolderPath(ref Commun.KnownFolder.Objects3D, Shell32.KnownFolderFlags.DontVerify, IntPtr.Zero, out string repertoire);
                AjouteRepertoire(repertoire, _noeudMyComputer);
                Shell32.SHGetKnownFolderPath(ref Commun.KnownFolder.Downloads, Shell32.KnownFolderFlags.DontVerify, IntPtr.Zero, out repertoire);
                AjouteRepertoire(repertoire, _noeudMyComputer);
                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyVideos.GetTreeNode(_listeIcones));

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    try
                    {
                        string nomVolume = drive.VolumeLabel;
                        if (drive.DriveType == DriveType.Network)
                        {
                            System.Text.StringBuilder remotePath = new System.Text.StringBuilder();
                            int taille = 250;
                            uint connRes = Mpr.WNetGetConnection(drive.Name.Substring(0, 2), remotePath, ref taille);
                            if (connRes == 0)
                                nomVolume = remotePath.ToString();
                        }

                        _listeIcones.Images.Add(Icones.GetFileIcon(drive.Name, false, true, Shell32.SHIL.SMALL));
                        TreeNode driveNode = new TreeNode($"{nomVolume} ({drive.Name})")
                        {
                            Name = drive.Name,
                            ImageIndex = _listeIcones.Images.Count - 1,
                            SelectedImageIndex = _listeIcones.Images.Count - 1,
                            Tag = new DirectoryInfo(drive.Name)
                        };
                        // drive.DriveType
                        driveNode.Nodes.Add("");
                        _noeudMyComputer.Nodes.Add(driveNode);
                    }
                    catch (Exception) { }
                }
                SelectedNode = Nodes[0];
            }
            else
            {
                RafraichirRepertoire(new DirectoryInfo(path));
            }
        }

        private void AjouteRepertoire(string path, TreeNode parent)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            _listeIcones.Images.Add(Icones.GetFileIcon(dirInfo.FullName, dirInfo.IsShortcut(), true, Shell32.SHIL.SMALL));
            TreeNode nouveauNoeud;
            nouveauNoeud = new TreeNode(dirInfo.Name)
            {
                Name = dirInfo.Name,
                ImageIndex = _listeIcones.Images.Count - 1,
                SelectedImageIndex = _listeIcones.Images.Count - 1,
                Tag = dirInfo
            };
            nouveauNoeud.Nodes.Add("");
            parent.Nodes.Add(nouveauNoeud);
        }

        private void TreeRepertoire_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != _noeudMyComputer)
                EnumerateDirectory(e.Node);
        }

        private void TreeRepertoire_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name != "My Computer")
            {
                if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(DirectoryInfo))
                {
                    DirectoryInfo dirInfo = (DirectoryInfo)e.Node.Tag;
                    if (SelectionneRepertoire != null)
                        SelectionneRepertoire.BeginInvoke(this, new SelectionneRepertoireEventArgs(dirInfo), null, null);
                    if (_liensFichiers != null) _liensFichiers.Rafraichir(dirInfo);
                    if (!e.Node.IsExpanded)
                    {
                        try
                        {
                            if (dirInfo.GetDirectories().Length == 0)
                                throw new Exception("Répertoire vide");
                        }
                        catch (Exception)
                        {
                            e.Node.Nodes.Clear();
                        }
                    }
                }
            }
            else
            {
                _liensFichiers.Initialise(e.Node);
                if (SelectionneRepertoire != null)
                    SelectionneRepertoire.BeginInvoke(this, new SelectionneRepertoireEventArgs(null), null, null);
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
                _listeIcones.Images.Add(Icones.GetFileIcon(dirI.FullName, dirI.IsShortcut(), true, Shell32.SHIL.SMALL));
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

        private string GetFolderPath(TreeNode parentNode)
        {
            if (parentNode.Tag != null)
            {
                if (parentNode.Tag.GetType() == typeof(DirectoryInfo))
                {
                    if (((DirectoryInfo)(parentNode.Tag)).FullName == Environment.SpecialFolder.Desktop.Repertoire())
                        return "Desktop";
                    else
                        return ((DirectoryInfo)(parentNode.Tag)).FullName;
                }
                if (parentNode == _noeudMyComputer)
                    return "My Computer";
            }

            return "";
        }

        public void RafraichirRepertoire(DirectoryInfo directoryInfo)
        {
            // TODO : RafraichirRepertoire (apres suppression, ajout, renommer, etc...)
            if ((directoryInfo != null) && (directoryInfo.Exists))
            {
                TreeNode noeudCourant = _noeudMyComputer;
                foreach (string rep in directoryInfo.FullName.Split('\\'))
                {
                    foreach (TreeNode treeNode in noeudCourant.Nodes)
                    {
                        if (treeNode.Tag.GetType() == typeof(DirectoryInfo))
                        {
                            if (((DirectoryInfo)treeNode.Tag).Name.ToLower().Trim().Trim('\\') == rep.ToLower().Trim())
                            {
                                treeNode.Expand();
                                noeudCourant = treeNode;
                            }
                        }
                    }
                }
                SelectedNode = noeudCourant;
            }
        }
        // TODO : Implémenter couper/copier/coller par drag & drop
    }
}
