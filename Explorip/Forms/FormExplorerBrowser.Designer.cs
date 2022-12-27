
namespace Explorip.Forms
{
    partial class FormExplorerBrowser
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
            this.MainSplitter = new System.Windows.Forms.SplitContainer();
            this.TabGauche = new System.Windows.Forms.CustomTabControl();
            this.TabPageGauche = new System.Windows.Forms.TabPage();
            this.ExplorerGauche = new Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser();
            this.TabDroite = new System.Windows.Forms.CustomTabControl();
            this.TabPageDroite = new System.Windows.Forms.TabPage();
            this.ExplorerDroite = new Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitter)).BeginInit();
            this.MainSplitter.Panel1.SuspendLayout();
            this.MainSplitter.Panel2.SuspendLayout();
            this.MainSplitter.SuspendLayout();
            this.TabGauche.SuspendLayout();
            this.TabPageGauche.SuspendLayout();
            this.TabDroite.SuspendLayout();
            this.TabPageDroite.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitter
            // 
            this.MainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitter.Location = new System.Drawing.Point(0, 0);
            this.MainSplitter.Name = "MainSplitter";
            // 
            // MainSplitter.Panel1
            // 
            this.MainSplitter.Panel1.Controls.Add(this.TabGauche);
            // 
            // MainSplitter.Panel2
            // 
            this.MainSplitter.Panel2.Controls.Add(this.TabDroite);
            this.MainSplitter.Size = new System.Drawing.Size(1039, 488);
            this.MainSplitter.SplitterDistance = 492;
            this.MainSplitter.TabIndex = 0;
            // 
            // TabGauche
            // 
            this.TabGauche.Controls.Add(this.TabPageGauche);
            this.TabGauche.DisplayStyle = System.Windows.Forms.TabStyle.VS2010;
            // 
            // 
            // 
            this.TabGauche.DisplayStyleProvider.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(195)))), ((int)(((byte)(101)))));
            this.TabGauche.DisplayStyleProvider.BorderColor = System.Drawing.Color.Transparent;
            this.TabGauche.DisplayStyleProvider.BorderColorHot = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(167)))), ((int)(((byte)(183)))));
            this.TabGauche.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.TabGauche.DisplayStyleProvider.CloserColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(99)))), ((int)(((byte)(61)))));
            this.TabGauche.DisplayStyleProvider.CloserColorActive = System.Drawing.Color.Red;
            this.TabGauche.DisplayStyleProvider.FocusTrack = false;
            this.TabGauche.DisplayStyleProvider.HotTrack = true;
            this.TabGauche.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TabGauche.DisplayStyleProvider.Opacity = 1F;
            this.TabGauche.DisplayStyleProvider.Overlap = 0;
            this.TabGauche.DisplayStyleProvider.Padding = new System.Drawing.Point(6, 5);
            this.TabGauche.DisplayStyleProvider.Radius = 3;
            this.TabGauche.DisplayStyleProvider.ShowTabCloser = true;
            this.TabGauche.DisplayStyleProvider.TextColor = System.Drawing.Color.White;
            this.TabGauche.DisplayStyleProvider.TextColorDisabled = System.Drawing.Color.WhiteSmoke;
            this.TabGauche.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.TabGauche.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabGauche.HotTrack = true;
            this.TabGauche.Location = new System.Drawing.Point(0, 0);
            this.TabGauche.Name = "TabGauche";
            this.TabGauche.SelectedIndex = 0;
            this.TabGauche.Size = new System.Drawing.Size(492, 488);
            this.TabGauche.TabIndex = 0;
            // 
            // TabPageGauche
            // 
            this.TabPageGauche.Controls.Add(this.ExplorerGauche);
            this.TabPageGauche.Location = new System.Drawing.Point(4, 27);
            this.TabPageGauche.Name = "TabPageGauche";
            this.TabPageGauche.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageGauche.Size = new System.Drawing.Size(484, 457);
            this.TabPageGauche.TabIndex = 0;
            this.TabPageGauche.Text = "tabPage1";
            this.TabPageGauche.UseVisualStyleBackColor = true;
            // 
            // ExplorerGauche
            // 
            this.ExplorerGauche.BackColor = System.Drawing.SystemColors.Control;
            this.ExplorerGauche.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExplorerGauche.Location = new System.Drawing.Point(3, 3);
            this.ExplorerGauche.Name = "ExplorerGauche";
            this.ExplorerGauche.PropertyBagName = "Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser";
            this.ExplorerGauche.Size = new System.Drawing.Size(478, 451);
            this.ExplorerGauche.TabIndex = 0;
            // 
            // TabDroite
            // 
            this.TabDroite.Controls.Add(this.TabPageDroite);
            this.TabDroite.DisplayStyle = System.Windows.Forms.TabStyle.VS2010;
            // 
            // 
            // 
            this.TabDroite.DisplayStyleProvider.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(195)))), ((int)(((byte)(101)))));
            this.TabDroite.DisplayStyleProvider.BorderColor = System.Drawing.Color.Transparent;
            this.TabDroite.DisplayStyleProvider.BorderColorHot = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(167)))), ((int)(((byte)(183)))));
            this.TabDroite.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.TabDroite.DisplayStyleProvider.CloserColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(99)))), ((int)(((byte)(61)))));
            this.TabDroite.DisplayStyleProvider.CloserColorActive = System.Drawing.Color.Red;
            this.TabDroite.DisplayStyleProvider.FocusTrack = false;
            this.TabDroite.DisplayStyleProvider.HotTrack = true;
            this.TabDroite.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TabDroite.DisplayStyleProvider.Opacity = 1F;
            this.TabDroite.DisplayStyleProvider.Overlap = 0;
            this.TabDroite.DisplayStyleProvider.Padding = new System.Drawing.Point(6, 5);
            this.TabDroite.DisplayStyleProvider.Radius = 3;
            this.TabDroite.DisplayStyleProvider.ShowTabCloser = true;
            this.TabDroite.DisplayStyleProvider.TextColor = System.Drawing.Color.White;
            this.TabDroite.DisplayStyleProvider.TextColorDisabled = System.Drawing.Color.WhiteSmoke;
            this.TabDroite.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.TabDroite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabDroite.HotTrack = true;
            this.TabDroite.Location = new System.Drawing.Point(0, 0);
            this.TabDroite.Name = "TabDroite";
            this.TabDroite.SelectedIndex = 0;
            this.TabDroite.Size = new System.Drawing.Size(543, 488);
            this.TabDroite.TabIndex = 0;
            // 
            // TabPageDroite
            // 
            this.TabPageDroite.Controls.Add(this.ExplorerDroite);
            this.TabPageDroite.Location = new System.Drawing.Point(4, 27);
            this.TabPageDroite.Name = "TabPageDroite";
            this.TabPageDroite.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageDroite.Size = new System.Drawing.Size(535, 457);
            this.TabPageDroite.TabIndex = 0;
            this.TabPageDroite.Text = "tabPage2";
            this.TabPageDroite.UseVisualStyleBackColor = true;
            // 
            // ExplorerDroite
            // 
            this.ExplorerDroite.BackColor = System.Drawing.SystemColors.Control;
            this.ExplorerDroite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExplorerDroite.Location = new System.Drawing.Point(3, 3);
            this.ExplorerDroite.Name = "ExplorerDroite";
            this.ExplorerDroite.PropertyBagName = "Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser";
            this.ExplorerDroite.Size = new System.Drawing.Size(529, 451);
            this.ExplorerDroite.TabIndex = 0;
            // 
            // FormExplorerBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 488);
            this.Controls.Add(this.MainSplitter);
            this.Name = "FormExplorerBrowser";
            this.Text = "Explorateur de fichiers";
            this.Shown += new System.EventHandler(this.FormExplorerBrowser_Shown);
            this.MainSplitter.Panel1.ResumeLayout(false);
            this.MainSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitter)).EndInit();
            this.MainSplitter.ResumeLayout(false);
            this.TabGauche.ResumeLayout(false);
            this.TabPageGauche.ResumeLayout(false);
            this.TabDroite.ResumeLayout(false);
            this.TabPageDroite.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainSplitter;
        private System.Windows.Forms.CustomTabControl TabGauche;
        private System.Windows.Forms.TabPage TabPageGauche;
        private System.Windows.Forms.CustomTabControl TabDroite;
        private System.Windows.Forms.TabPage TabPageDroite;
        private Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser ExplorerGauche;
        private Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser ExplorerDroite;
    }
}