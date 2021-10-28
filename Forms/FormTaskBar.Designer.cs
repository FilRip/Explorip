namespace Explorip.Forms
{
    partial class FormTaskBar
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
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.VerrouillerLaBarreDesTachesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvTaches2 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvTaches = new Explorip.ComposantsWinForm.FilRipListView.FilRipListView();
            this.filRipColumnHeader1 = ((Explorip.ComposantsWinForm.FilRipListView.FilRipColumnHeader)(new Explorip.ComposantsWinForm.FilRipListView.FilRipColumnHeader()));
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 500;
            this.timerRefresh.Tick += new System.EventHandler(this.TimerRefresh_Tick);
            // 
            // button1
            // 
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VerrouillerLaBarreDesTachesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.quitterToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(228, 54);
            // 
            // VerrouillerLaBarreDesTachesToolStripMenuItem
            // 
            this.VerrouillerLaBarreDesTachesToolStripMenuItem.Checked = true;
            this.VerrouillerLaBarreDesTachesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VerrouillerLaBarreDesTachesToolStripMenuItem.Name = "VerrouillerLaBarreDesTachesToolStripMenuItem";
            this.VerrouillerLaBarreDesTachesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.VerrouillerLaBarreDesTachesToolStripMenuItem.Text = "Vérrouiller la barre des tâches";
            this.VerrouillerLaBarreDesTachesToolStripMenuItem.Click += new System.EventHandler(this.VerrouillerLaBarreDesTachesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(224, 6);
            // 
            // quitterToolStripMenuItem
            // 
            this.quitterToolStripMenuItem.Name = "quitterToolStripMenuItem";
            this.quitterToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.quitterToolStripMenuItem.Text = "Quitter";
            this.quitterToolStripMenuItem.Click += new System.EventHandler(this.QuitterToolStripMenuItem_Click);
            // 
            // lvTaches2
            // 
            this.lvTaches2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvTaches2.HideSelection = false;
            this.lvTaches2.Location = new System.Drawing.Point(81, 12);
            this.lvTaches2.Name = "lvTaches2";
            this.lvTaches2.Size = new System.Drawing.Size(410, 376);
            this.lvTaches2.TabIndex = 2;
            this.lvTaches2.UseCompatibleStateImageBehavior = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 564;
            // 
            // lvTaches
            // 
            this.lvTaches.ActiverCouleurAlternee = true;
            this.lvTaches.AddMenuShowColumns = false;
            this.lvTaches.Columns.AddRange(new Explorip.ComposantsWinForm.FilRipListView.FilRipColumnHeader[] {
            this.filRipColumnHeader1});
            this.lvTaches.CouleurAlternee1 = System.Drawing.Color.White;
            this.lvTaches.CouleurAlternee2 = System.Drawing.Color.LightGray;
            this.lvTaches.HideSelection = false;
            this.lvTaches.Location = new System.Drawing.Point(497, 12);
            this.lvTaches.Name = "lvTaches";
            this.lvTaches.OwnerDraw = true;
            this.lvTaches.Size = new System.Drawing.Size(405, 376);
            this.lvTaches.TabIndex = 3;
            this.lvTaches.UseCompatibleStateImageBehavior = false;
            this.lvTaches.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LvTaches_MouseUp);
            // 
            // filRipColumnHeader1
            // 
            this.filRipColumnHeader1.ActiveCouleur = false;
            this.filRipColumnHeader1.ActiveCouleurOk = false;
            this.filRipColumnHeader1.CouleurArrierePlan = System.Drawing.SystemColors.Control;
            this.filRipColumnHeader1.CouleurOk = System.Drawing.Color.Black;
            this.filRipColumnHeader1.CouleurSinon = System.Drawing.Color.Red;
            this.filRipColumnHeader1.CouleurTexte = System.Drawing.SystemColors.ControlText;
            this.filRipColumnHeader1.ExpressionOK = "";
            // 
            // FormTaskBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 450);
            this.ControlBox = false;
            this.Controls.Add(this.lvTaches);
            this.Controls.Add(this.lvTaches2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormTaskBar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem VerrouillerLaBarreDesTachesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem quitterToolStripMenuItem;
        private System.Windows.Forms.ListView lvTaches2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private ComposantsWinForm.FilRipListView.FilRipListView lvTaches;
        private ComposantsWinForm.FilRipListView.FilRipColumnHeader filRipColumnHeader1;
    }
}