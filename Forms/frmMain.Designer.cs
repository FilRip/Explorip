namespace Filexplorip.Forms
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.imgMain = new System.Windows.Forms.ImageList(this.components);
            this.treeMain = new System.Windows.Forms.TreeView();
            this.cmsMonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.monMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsMonMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgMain
            // 
            this.imgMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgMain.ImageStream")));
            this.imgMain.TransparentColor = System.Drawing.Color.Transparent;
            this.imgMain.Images.SetKeyName(0, "RemovableDrive.png");
            this.imgMain.Images.SetKeyName(1, "CDRom.png");
            this.imgMain.Images.SetKeyName(2, "Desktop.png");
            this.imgMain.Images.SetKeyName(3, "Drive.png");
            this.imgMain.Images.SetKeyName(4, "MyComputer.png");
            this.imgMain.Images.SetKeyName(5, "NetworkDrive.png");
            this.imgMain.Images.SetKeyName(6, "Folder.png");
            // 
            // treeMain
            // 
            this.treeMain.AllowDrop = true;
            this.treeMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMain.ImageIndex = 6;
            this.treeMain.ImageList = this.imgMain;
            this.treeMain.Location = new System.Drawing.Point(0, 0);
            this.treeMain.Name = "treeMain";
            this.treeMain.SelectedImageIndex = 0;
            this.treeMain.Size = new System.Drawing.Size(403, 457);
            this.treeMain.TabIndex = 3;
            // 
            // cmsMonMenu
            // 
            this.cmsMonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monMenuToolStripMenuItem,
            this.toolStripMenuItem1});
            this.cmsMonMenu.Name = "cmsMonMenu";
            this.cmsMonMenu.Size = new System.Drawing.Size(181, 54);
            // 
            // monMenuToolStripMenuItem
            // 
            this.monMenuToolStripMenuItem.Name = "monMenuToolStripMenuItem";
            this.monMenuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.monMenuToolStripMenuItem.Text = "Mon Menu";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 457);
            this.Controls.Add(this.treeMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "Explorateur de fichier";
            this.cmsMonMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imgMain;
        private System.Windows.Forms.TreeView treeMain;
        private System.Windows.Forms.ContextMenuStrip cmsMonMenu;
        private System.Windows.Forms.ToolStripMenuItem monMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}

