
namespace Explorip.ComposantsWinForm
{
    partial class PanelExplorer
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.TreeRepertoire = new Explorip.ComposantsWinForm.DirectoryTreeView();
            this.lvFichiers = new Explorip.ComposantsWinForm.FichiersListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(6, 6);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.TreeRepertoire);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvFichiers);
            this.splitContainer2.Size = new System.Drawing.Size(1744, 985);
            this.splitContainer2.SplitterDistance = 578;
            this.splitContainer2.SplitterWidth = 8;
            this.splitContainer2.TabIndex = 5;
            // 
            // TreeRepertoire
            // 
            this.TreeRepertoire.AllowDrop = true;
            this.TreeRepertoire.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeRepertoire.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TreeRepertoire.ImageIndex = 0;
            this.TreeRepertoire.LiensFichiers = null;
            this.TreeRepertoire.Location = new System.Drawing.Point(6, 6);
            this.TreeRepertoire.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.TreeRepertoire.Name = "TreeRepertoire";
            this.TreeRepertoire.SelectedImageIndex = 0;
            this.TreeRepertoire.Size = new System.Drawing.Size(566, 973);
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
            this.lvFichiers.LiensRepertoires = null;
            this.lvFichiers.Location = new System.Drawing.Point(6, 6);
            this.lvFichiers.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lvFichiers.Name = "lvFichiers";
            this.lvFichiers.OwnerDraw = true;
            this.lvFichiers.Size = new System.Drawing.Size(1142, 969);
            this.lvFichiers.TabIndex = 0;
            this.lvFichiers.UseCompatibleStateImageBehavior = false;
            // 
            // PanelExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer2);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "PanelExplorer";
            this.Size = new System.Drawing.Size(1756, 996);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private DirectoryTreeView TreeRepertoire;
        private FichiersListView lvFichiers;
    }
}
