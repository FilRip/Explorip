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
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitter)).BeginInit();
            this.MainSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitter
            // 
            this.MainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitter.Location = new System.Drawing.Point(0, 0);
            this.MainSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.MainSplitter.Name = "MainSplitter";
            this.MainSplitter.Size = new System.Drawing.Size(933, 527);
            this.MainSplitter.SplitterDistance = 466;
            this.MainSplitter.SplitterWidth = 8;
            this.MainSplitter.TabIndex = 0;
            this.MainSplitter.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.MainSplitter.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            // 
            // FormExplorerBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(933, 527);
            this.Controls.Add(this.MainSplitter);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FormExplorerBrowser";
            this.Text = "Explorateur de fichiers";
            this.Shown += new System.EventHandler(this.FormExplorerBrowser_Shown);
            this.SizeChanged += new System.EventHandler(this.FormExplorerBrowser_SizeChanged);
            this.Move += new System.EventHandler(this.FormExplorerBrowser_Move);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitter)).EndInit();
            this.MainSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainSplitter;
    }
}