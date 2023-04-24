using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Messaging;
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
        private readonly ImageList _listeIcones;

        public delegate void DelegateCliqueRepertoire(object sender, SelectionneRepertoireEventArgs e);
        public event DelegateCliqueRepertoire SelectionneRepertoire;

        private TreeNode _noeudMyComputer;

        public bool SelectionChange { get; set; }

        public DirectoryTreeView() : base()
        {
            InitializeComponent();
            MenuContextuel = new ContextMenuStrip();
            AfterExpand += TreeRepertoire_AfterExpand;
            AfterSelect += TreeRepertoire_AfterSelect;
            MouseUp += new MouseEventHandler(TreeMain_MouseUp);
            _listeIcones = new ImageList();
            ImageList = _listeIcones;
            SelectionChange = true;
        }

        public ContextMenuStrip MenuContextuel { get; set; }

        public FichiersListView LiensFichiers { get; set; }

        public void Init(string path)
        {
            Nodes.Clear();
            if (path == "My Computer")
            {
                _noeudMyComputer = new TreeNode(Environment.SpecialFolder.MyComputer.NomTraduit())
                {
                    Name = "My Computer",
                };
                IntPtr tempPidl = IntPtr.Zero;
                Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DRIVES, ref tempPidl);
                _listeIcones.Images.Add(Icones.GetIcone(tempPidl, true));
                _noeudMyComputer.ImageIndex = _listeIcones.Images.Count - 1;
                _noeudMyComputer.SelectedImageIndex = _listeIcones.Images.Count - 1;
                _noeudMyComputer.Tag = "My Computer";
                Nodes.Add(_noeudMyComputer);

                // TODO : voir https://github.com/aybe/Windows-API-Code-Pack-1.1
                TreeNode noeudDesktop = new(Environment.SpecialFolder.Desktop.NomTraduit(), 2, 2)
                {
                    Name = "Desktop",
                };
                _noeudMyComputer.Nodes.Add(noeudDesktop);
                noeudDesktop.Name = "Desktop";
                noeudDesktop.Tag = new DirectoryInfo(Environment.SpecialFolder.Desktop.Repertoire());
                int numIcone = Icones.GetNumIcon(Environment.SpecialFolder.Desktop.Repertoire(), false, true, out IntPtr pidl);
                Bitmap monIcone = Icones.GetFileIcon(pidl, numIcone, Shell32.SHIL.SMALL);
                _listeIcones.Images.Add(monIcone);
                noeudDesktop.ImageIndex = _listeIcones.Images.Count - 1;
                noeudDesktop.SelectedImageIndex = _listeIcones.Images.Count - 1;
                if ((Directory.GetFiles(Environment.SpecialFolder.Desktop.Repertoire()).Length > 0) || (Directory.GetDirectories(Environment.SpecialFolder.Desktop.Repertoire()).Length > 0))
                {
                    noeudDesktop.Nodes.Add("");
                }

                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyDocuments.GetTreeNode(_listeIcones));
                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyPictures.GetTreeNode(_listeIcones));
                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyMusic.GetTreeNode(_listeIcones));
                Guid guidObjet3D = Commun.KnownFolder.Objects3D, guidDownload = Commun.KnownFolder.Downloads;
                Shell32.SHGetKnownFolderPath(ref guidObjet3D, Shell32.KnownFolder.DontVerify, IntPtr.Zero, out string repertoire);
                AjouteRepertoire(repertoire, _noeudMyComputer);
                Shell32.SHGetKnownFolderPath(ref guidDownload, Shell32.KnownFolder.DontVerify, IntPtr.Zero, out repertoire);
                AjouteRepertoire(repertoire, _noeudMyComputer);
                _noeudMyComputer.Nodes.Add(Environment.SpecialFolder.MyVideos.GetTreeNode(_listeIcones));

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    try
                    {
                        string nomVolume = drive.VolumeLabel;
                        if (drive.DriveType == DriveType.Network && drive.IsReady)
                        {
                            System.Text.StringBuilder remotePath = new();
                            int taille = 250;
                            uint connRes = Mpr.WNetGetConnection(drive.Name.Substring(0, 2), remotePath, ref taille);
                            if (connRes == 0)
                                nomVolume = remotePath.ToString();
                        }

                        numIcone = Icones.GetNumIcon(drive.Name, false, true, out pidl);
                        monIcone = Icones.GetFileIcon(pidl, numIcone, Shell32.SHIL.SMALL);
                        _listeIcones.Images.Add(monIcone);
                        TreeNode driveNode = new($"{nomVolume} ({drive.Name})")
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
                    catch (Exception) { /* Ignore errors */ }
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
            DirectoryInfo dirInfo = new(path);
            int numIcone = Icones.GetNumIcon(dirInfo.FullName, dirInfo.IsShortcut(), true, out IntPtr pidl);
            Bitmap monIcone = Icones.GetFileIcon(pidl, numIcone, Shell32.SHIL.SMALL);
            _listeIcones.Images.Add(monIcone);
            TreeNode nouveauNoeud;
            nouveauNoeud = new TreeNode(dirInfo.Name)
            {
                Name = dirInfo.FullName,
                ImageIndex = _listeIcones.Images.Count - 1,
                SelectedImageIndex = _listeIcones.Images.Count - 1,
                Tag = dirInfo
            };
            nouveauNoeud.Nodes.Add("");
            parent.Name = dirInfo.Parent.FullName;
            parent.Nodes.Add(nouveauNoeud);
        }

        private void TreeRepertoire_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != _noeudMyComputer)
                EnumerateDirectory(e.Node);
        }

        private void TreeRepertoire_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!SelectionChange)
                return;

            if (e.Node.Name != "My Computer")
            {
                if (e.Node.Tag is DirectoryInfo dirInfo)
                {
                    SelectionneRepertoire?.BeginInvoke(this, new SelectionneRepertoireEventArgs(dirInfo), new AsyncCallback(SelectionneRepertoire_Termine), null);
                    LiensFichiers?.Rafraichir(dirInfo);
                    if (!e.Node.IsExpanded)
                    {
                        try
                        {
                            if (dirInfo.GetDirectories().Length == 0)
                                throw new Exceptions.ExploripException("Répertoire vide");
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
                LiensFichiers.Initialise(e.Node);
                SelectionneRepertoire?.BeginInvoke(this, new SelectionneRepertoireEventArgs(null), new AsyncCallback(SelectionneRepertoire_Termine), null);
            }
        }

        private void SelectionneRepertoire_Termine(IAsyncResult ar)
        {
            try
            {
                AsyncResult result;
                result = (AsyncResult)ar;
                DelegateCliqueRepertoire caller = (DelegateCliqueRepertoire)result.AsyncDelegate;
                caller.EndInvoke(ar);
            }
            catch (Exception) { /* Les erreurs ici ne sont pas interessantes */ }
        }

        private void TreeMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode noeudClicke;
                noeudClicke = GetNodeAt(e.X, e.Y);
                if (noeudClicke == null)
                    return;
                ShellContextMenu ctxMnu = new();
                DirectoryInfo[] dir = new DirectoryInfo[1];
                dir[0] = new DirectoryInfo(GetFolderPath(noeudClicke));
                ctxMnu.ShowContextMenu(dir, PointToScreen(new Point(e.X, e.Y)), MenuContextuel);
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
                TreeNode node = new(dirI.Name, 6, 6)
                {
                    Name = dirI.Name
                };
                int numIcone = Icones.GetNumIcon(dirI.FullName, dirI.IsShortcut(), true, out IntPtr pidl);
                Bitmap monIcone = Icones.GetFileIcon(pidl, numIcone, Shell32.SHIL.SMALL);
                _listeIcones.Images.Add(monIcone);
                int imgIndex = _listeIcones.Images.Count - 1;
                node.Name = dirI.FullName;
                node.ImageIndex = imgIndex;
                node.SelectedImageIndex = imgIndex;
                node.Tag = dirI;
                try
                {
                    if (dirI.GetDirectories().Length > 0)
                        node.Nodes.Add("");
                }
                catch (Exception) { /* Ignore errors */ }
                parentNode.Nodes.Add(node);
            }
        }

        private string GetFolderPath(TreeNode parentNode)
        {
            if (parentNode?.Tag != null)
            {
                if (parentNode.Tag is DirectoryInfo dirInfo)
                {
                    if (dirInfo.FullName == Environment.SpecialFolder.Desktop.Repertoire())
                        return "Desktop";
                    else
                        return dirInfo.FullName;
                }
                if (parentNode == _noeudMyComputer)
                    return "My Computer";
            }

            return "";
        }

        public TreeNode RafraichirRepertoire(DirectoryInfo directoryInfo)
        {
            TreeNode retour = null;
            // TODO : RafraichirRepertoire (apres suppression, ajout, renommer, etc...)
            if ((directoryInfo != null) && (directoryInfo.Exists))
            {
                TreeNode noeudCourant = _noeudMyComputer;
                foreach (string rep in directoryInfo.FullName.Split('\\'))
                {
                    foreach (TreeNode treeNode in noeudCourant.Nodes)
                    {
                        if (treeNode?.Tag is DirectoryInfo dirInfo &&
                            dirInfo.Name.ToLower().Trim().Trim('\\') == rep.ToLower().Trim().Trim('\\'))
                        {
                            noeudCourant.Expand();
                            treeNode.Expand();
                            TreeRepertoire_AfterExpand(null, new TreeViewEventArgs(treeNode));
                            noeudCourant = treeNode;
                            if (dirInfo.FullName == directoryInfo.FullName)
                                retour = treeNode;
                        }
                    }
                }
                TreeRepertoire_AfterSelect(this, new TreeViewEventArgs(noeudCourant));
            }
            return retour;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // DirectoryTreeView
            // 
            AllowDrop = true;
            ResumeLayout(false);

        }
        // TODO : Implémenter couper/copier/coller par drag & drop
    }
}
