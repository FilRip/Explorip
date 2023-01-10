using System.Windows.Forms;

namespace Explorip.ComposantsWinForm
{
    partial class TabExplorerBrowser
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
            this.ContextMenuTab = new System.Windows.Forms.ContextMenuStrip();
            this.ouvrirUnNouvelOngletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fermerTousLesOngletsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContextMenuTab
            // 
            this.ContextMenuTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.ouvrirUnNouvelOngletToolStripMenuItem,
                this.fermerTousLesOngletsToolStripMenuItem,
                this.ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem
            });
            this.ContextMenuTab.Name = "ContextMenuTabControl";
            this.ContextMenuTab.Size = new System.Drawing.Size(305, 70);
            // 
            // ouvrirUnNouvelOngletToolStripMenuItem
            // 
            this.ouvrirUnNouvelOngletToolStripMenuItem.Name = "ouvrirUnNouvelOngletToolStripMenuItem";
            this.ouvrirUnNouvelOngletToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.ouvrirUnNouvelOngletToolStripMenuItem.Text = "Ouvrir un nouvel onglet";
            this.ouvrirUnNouvelOngletToolStripMenuItem.Click += new System.EventHandler(this.OuvrirUnNouvelOngletToolStripMenuItem_Click);
            // 
            // fermerTousLesOngletsToolStripMenuItem
            // 
            this.fermerTousLesOngletsToolStripMenuItem.Name = "fermerTousLesOngletsToolStripMenuItem";
            this.fermerTousLesOngletsToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.fermerTousLesOngletsToolStripMenuItem.Text = "Fermer tous les onglets";
            this.fermerTousLesOngletsToolStripMenuItem.Click += new System.EventHandler(this.FermerTousLesOngletsToolStripMenuItem_Click);
            // 
            // ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem
            // 
            this.ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem.Name = "ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem";
            this.ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem.Size = new System.Drawing.Size(304, 22);
            this.ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem.Text = "Ouvrir un nouvel onglet sur l\'autre panneau";
            this.ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem.Click += new System.EventHandler(this.OuvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem_Click);
            // 
            // TabExplorerBrowser
            // 
            this.DisplayStyle = System.Windows.Forms.TabStyle.VS2010;
            this.DisplayStyleProvider.BackgroundColor = System.Drawing.Color.FromArgb(229, 195, 101);
            this.DisplayStyleProvider.BorderColor = System.Drawing.Color.Transparent;
            this.DisplayStyleProvider.BorderColorHot = System.Drawing.Color.FromArgb(155, 167, 183);
            this.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(127, 157, 185);
            this.DisplayStyleProvider.CloserColor = System.Drawing.Color.FromArgb(117, 99, 61);
            this.DisplayStyleProvider.CloserColorActive = System.Drawing.Color.Red;
            this.DisplayStyleProvider.FocusTrack = false;
            this.DisplayStyleProvider.HotTrack = true;
            this.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DisplayStyleProvider.Opacity = 1F;
            this.DisplayStyleProvider.Overlap = 0;
            this.DisplayStyleProvider.Padding = new System.Drawing.Point(6, 5);
            this.DisplayStyleProvider.Radius = 3;
            this.DisplayStyleProvider.ShowTabCloser = true;
            this.DisplayStyleProvider.TextColor = System.Drawing.Color.White;
            this.DisplayStyleProvider.TextColorDisabled = System.Drawing.Color.WhiteSmoke;
            this.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HotTrack = true;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "TabExplorerBrowser";
            this.SelectedIndex = 0;
            this.Size = new System.Drawing.Size(492, 488);
            this.TabIndex = 0;

            this.ContextMenuTab.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip ContextMenuTab;
        private System.Windows.Forms.ToolStripMenuItem ouvrirUnNouvelOngletToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fermerTousLesOngletsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ouvrirUnNouvelOngletSurLautrePanneauToolStripMenuItem;
    }
}
