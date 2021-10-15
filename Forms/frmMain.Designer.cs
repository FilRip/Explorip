namespace Explorip.Forms
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.filRipTabControl1 = new Explorip.ComposantsWinForm.FilRipTabControl.FilRipTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.TreeRepertoire = new ComposantsWinForm.DirectoryTreeView();
            this.lvFichiers = new Explorip.ComposantsWinForm.FichiersListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.filRipTabControl2 = new Explorip.ComposantsWinForm.FilRipTabControl.FilRipTabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.filRipTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.filRipTabControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.filRipTabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.filRipTabControl2);
            this.splitContainer1.Size = new System.Drawing.Size(945, 433);
            this.splitContainer1.SplitterDistance = 439;
            this.splitContainer1.TabIndex = 4;
            // 
            // filRipTabControl1
            // 
            this.filRipTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filRipTabControl1.BackgroundColorDebut = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.filRipTabControl1.BackgroundColorFin = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.filRipTabControl1.BackgroundColorHotDebut = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.filRipTabControl1.BackgroundColorHotFin = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(217)))), ((int)(((byte)(245)))));
            this.filRipTabControl1.BackgroundColorSelectedDebut = System.Drawing.SystemColors.ControlLight;
            this.filRipTabControl1.BackgroundColorSelectedFin = System.Drawing.SystemColors.Window;
            this.filRipTabControl1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.filRipTabControl1.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.filRipTabControl1.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.filRipTabControl1.Controls.Add(this.tabPage1);
            this.filRipTabControl1.Controls.Add(this.tabPage3);
            this.filRipTabControl1.FocusColor = System.Drawing.Color.Orange;
            this.filRipTabControl1.FocusTrack = true;
            this.filRipTabControl1.HotTrack = true;
            this.filRipTabControl1.ItemSize = new System.Drawing.Size(58, 18);
            this.filRipTabControl1.Location = new System.Drawing.Point(3, 3);
            this.filRipTabControl1.Name = "filRipTabControl1";
            this.filRipTabControl1.Opacity = 1F;
            this.filRipTabControl1.Overlap = 0;
            this.filRipTabControl1.Radius = 2;
            this.filRipTabControl1.SelectedIndex = 0;
            this.filRipTabControl1.Size = new System.Drawing.Size(433, 427);
            this.filRipTabControl1.StyleText = System.Drawing.FontStyle.Regular;
            this.filRipTabControl1.StyleTextSelected = System.Drawing.FontStyle.Regular;
            this.filRipTabControl1.TabIndex = 0;
            this.filRipTabControl1.TextColor = System.Drawing.SystemColors.ControlText;
            this.filRipTabControl1.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.filRipTabControl1.TextColorSelected = System.Drawing.SystemColors.ControlText;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(425, 400);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(6, 6);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.TreeRepertoire);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvFichiers);
            this.splitContainer2.Size = new System.Drawing.Size(413, 388);
            this.splitContainer2.SplitterDistance = 137;
            this.splitContainer2.TabIndex = 4;
            // 
            // TreeRepertoire
            // 
            this.TreeRepertoire.AllowDrop = true;
            this.TreeRepertoire.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeRepertoire.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TreeRepertoire.ImageIndex = 6;
            this.TreeRepertoire.Location = new System.Drawing.Point(3, 3);
            this.TreeRepertoire.Name = "TreeRepertoire";
            this.TreeRepertoire.SelectedImageIndex = 0;
            this.TreeRepertoire.Size = new System.Drawing.Size(131, 382);
            this.TreeRepertoire.TabIndex = 3;
            // 
            // lvFichiers
            // 
            this.lvFichiers.ActiverCouleurAlternee = true;
            this.lvFichiers.AddMenuShowColumns = true;
            this.lvFichiers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFichiers.CouleurAlternee1 = System.Drawing.Color.White;
            this.lvFichiers.CouleurAlternee2 = System.Drawing.Color.LightGray;
            this.lvFichiers.HideSelection = false;
            this.lvFichiers.Location = new System.Drawing.Point(3, 3);
            this.lvFichiers.Name = "lvFichiers";
            this.lvFichiers.OwnerDraw = true;
            this.lvFichiers.Size = new System.Drawing.Size(266, 382);
            this.lvFichiers.TabIndex = 0;
            this.lvFichiers.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(425, 400);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // filRipTabControl2
            // 
            this.filRipTabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filRipTabControl2.BackgroundColorDebut = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.filRipTabControl2.BackgroundColorFin = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.filRipTabControl2.BackgroundColorHotDebut = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.filRipTabControl2.BackgroundColorHotFin = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(217)))), ((int)(((byte)(245)))));
            this.filRipTabControl2.BackgroundColorSelectedDebut = System.Drawing.SystemColors.ControlLight;
            this.filRipTabControl2.BackgroundColorSelectedFin = System.Drawing.SystemColors.Window;
            this.filRipTabControl2.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.filRipTabControl2.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.filRipTabControl2.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.filRipTabControl2.Controls.Add(this.tabPage2);
            this.filRipTabControl2.FocusColor = System.Drawing.Color.Orange;
            this.filRipTabControl2.FocusTrack = true;
            this.filRipTabControl2.HotTrack = true;
            this.filRipTabControl2.ItemSize = new System.Drawing.Size(58, 18);
            this.filRipTabControl2.Location = new System.Drawing.Point(3, 3);
            this.filRipTabControl2.Name = "filRipTabControl2";
            this.filRipTabControl2.Opacity = 1F;
            this.filRipTabControl2.Overlap = 0;
            this.filRipTabControl2.Radius = 2;
            this.filRipTabControl2.SelectedIndex = 0;
            this.filRipTabControl2.Size = new System.Drawing.Size(496, 427);
            this.filRipTabControl2.StyleText = System.Drawing.FontStyle.Regular;
            this.filRipTabControl2.StyleTextSelected = System.Drawing.FontStyle.Regular;
            this.filRipTabControl2.TabIndex = 0;
            this.filRipTabControl2.TextColor = System.Drawing.SystemColors.ControlText;
            this.filRipTabControl2.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.filRipTabControl2.TextColorSelected = System.Drawing.SystemColors.ControlText;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(488, 400);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 457);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "Explorateur de fichier";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.filRipTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.filRipTabControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComposantsWinForm.DirectoryTreeView TreeRepertoire;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ComposantsWinForm.FilRipTabControl.FilRipTabControl filRipTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private ComposantsWinForm.FilRipTabControl.FilRipTabControl filRipTabControl2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ComposantsWinForm.FichiersListView lvFichiers;
        private System.Windows.Forms.TabPage tabPage3;
    }
}

