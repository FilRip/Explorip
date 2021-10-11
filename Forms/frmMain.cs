using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Filexplorip.Helpers;
using Filexplorip.Sorters;

namespace Filexplorip.Forms
{
    public partial class FrmMain : Form
    {
        //private readonly FormTaskBar _barreDesTaches;
        
        public FrmMain()
        {
            InitializeComponent();

            /*_barreDesTaches = new FormTaskBar();
            _barreDesTaches.Show();*/

            TreeNode rootDesktop = new TreeNode("Desktop", 2, 2);
            this.treeMain.Nodes.Add(rootDesktop);
            rootDesktop.Name = "Desktop";
            rootDesktop.Nodes.Add("");
            TreeNode myComputer = new TreeNode("My Computer", 4, 4)
            {
                Name = "My Computer"
            };
            this.treeMain.Nodes.Add(myComputer);
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeNode driveNode = new TreeNode(drive.Name)
                {
                    Name = drive.Name
                };
                switch (drive.DriveType)
                {
                    case DriveType.CDRom:
                        driveNode.SelectedImageIndex = 1;
                        driveNode.ImageIndex = 1;
                        break;
                    case DriveType.Network:
                        driveNode.SelectedImageIndex = 5;
                        driveNode.ImageIndex = 5;
                        break;
                    case DriveType.Removable:
                        driveNode.SelectedImageIndex = 0;
                        driveNode.ImageIndex = 0;
                        break;
                    default:
                        driveNode.SelectedImageIndex = 3;
                        driveNode.ImageIndex = 3;
                        break;
                }
                driveNode.Nodes.Add("");
                myComputer.Nodes.Add(driveNode);
            }

            this.treeMain.BeforeExpand += new TreeViewCancelEventHandler(TreeMain_BeforeExpand);
            this.treeMain.MouseDown += new MouseEventHandler(TreeMain_MouseDown);
        }

        void TreeMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.treeMain.SelectedNode = this.treeMain.GetNodeAt(e.X, e.Y);
                if (this.treeMain.SelectedNode.Tag != null)
                {
                    ShellContextMenu ctxMnu = new ShellContextMenu();
                    FileInfo[] arrFI = new FileInfo[1];
                    arrFI[0] = new FileInfo(this.treeMain.SelectedNode.Tag.ToString());
                    ctxMnu.ShowContextMenu(arrFI, this.PointToScreen(new Point(e.X, e.Y)), cmsMonMenu);
                }
                else
                {
                    ShellContextMenu ctxMnu = new ShellContextMenu();
                    DirectoryInfo[] dir = new DirectoryInfo[1];
                    dir[0] = new DirectoryInfo(GetFolderPath(this.treeMain.SelectedNode));
                    ctxMnu.ShowContextMenu(dir, this.PointToScreen(new Point(e.X, e.Y)), cmsMonMenu);
                }
            }
        }

        private void TreeMain_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Text != "My Computer")
            {
                this.EnumerateDirectory(e.Node);
            }
        }

        private void EnumerateDirectory(TreeNode parentNode)
        {
            DirectoryInfo dirInfo;
            string path = GetFolderPath(parentNode);
            parentNode.Nodes.Clear();

            dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            Array.Sort(dirs, new DirectorySorter());
            foreach (DirectoryInfo dirI in dirs)
            {
                TreeNode node = new TreeNode(dirI.Name, 6, 6)
                {
                    Name = dirI.Name
                };
                this.imgMain.Images.Add(Helpers.Icones.GetFileIcon(dirI.FullName, EstUnRaccourcis(dirI.FullName), true, true));
                int imgIndex = this.imgMain.Images.Count - 1;
                node.ImageIndex = imgIndex;
                node.SelectedImageIndex = imgIndex;
                node.Nodes.Add("");
                parentNode.Nodes.Add(node);
            }

            FileInfo[] files = dirInfo.GetFiles();
            Array.Sort(files, new FileSorter());
            foreach (FileInfo file in files)
            {
                TreeNode node = new TreeNode(file.Name, 6, 6)
                {
                    Name = file.Name
                };

                this.imgMain.Images.Add(Helpers.Icones.GetFileIcon(file.FullName, EstUnRaccourcis(file.FullName), false, true));

                int imgIndex = this.imgMain.Images.Count - 1;
                node.Tag = file.FullName;
                node.ImageIndex = imgIndex;
                node.SelectedImageIndex = imgIndex;
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
            else if ((pathSplit.Length == 1) && (pathSplit[0] == "My Computer"))
            {
                path = "My Computer";
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
        private bool EstUnRaccourcis(string filename)
        {
            // TODO : Détecter les raccourcis de répertoire
            if (Path.GetExtension(filename).ToLower().Trim() == ".lnk")
                return true;
            else
                return false;
        }
    }
}