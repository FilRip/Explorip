namespace Explorip.Forms
{
    partial class FormExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExplorer));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabExplorer2 = new Explorip.ComposantsWinForm.TabExplorer();
            this.tabExplorer1 = new Explorip.ComposantsWinForm.TabExplorer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(24, 23);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabExplorer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabExplorer1);
            this.splitContainer1.Size = new System.Drawing.Size(1890, 833);
            this.splitContainer1.SplitterDistance = 878;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabExplorer2
            // 
            this.tabExplorer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabExplorer2.BackgroundColorDebut = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.tabExplorer2.BackgroundColorFin = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.tabExplorer2.BackgroundColorHotDebut = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.tabExplorer2.BackgroundColorHotFin = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(217)))), ((int)(((byte)(245)))));
            this.tabExplorer2.BackgroundColorSelectedDebut = System.Drawing.SystemColors.ControlLight;
            this.tabExplorer2.BackgroundColorSelectedFin = System.Drawing.SystemColors.Window;
            this.tabExplorer2.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.tabExplorer2.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.tabExplorer2.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.tabExplorer2.FocusColor = System.Drawing.Color.Orange;
            this.tabExplorer2.FocusTrack = true;
            this.tabExplorer2.HotTrack = true;
            this.tabExplorer2.ItemSize = new System.Drawing.Size(42, 18);
            this.tabExplorer2.Location = new System.Drawing.Point(6, 6);
            this.tabExplorer2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabExplorer2.Name = "tabExplorer2";
            this.tabExplorer2.Opacity = 1F;
            this.tabExplorer2.Overlap = 0;
            this.tabExplorer2.Radius = 2;
            this.tabExplorer2.SelectedIndex = 0;
            this.tabExplorer2.Size = new System.Drawing.Size(866, 821);
            this.tabExplorer2.StyleText = System.Drawing.FontStyle.Regular;
            this.tabExplorer2.StyleTextSelected = System.Drawing.FontStyle.Regular;
            this.tabExplorer2.TabIndex = 0;
            this.tabExplorer2.TextColor = System.Drawing.SystemColors.ControlText;
            this.tabExplorer2.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.tabExplorer2.TextColorSelected = System.Drawing.SystemColors.ControlText;
            // 
            // tabExplorer1
            // 
            this.tabExplorer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabExplorer1.BackgroundColorDebut = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.tabExplorer1.BackgroundColorFin = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.tabExplorer1.BackgroundColorHotDebut = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.tabExplorer1.BackgroundColorHotFin = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(217)))), ((int)(((byte)(245)))));
            this.tabExplorer1.BackgroundColorSelectedDebut = System.Drawing.SystemColors.ControlLight;
            this.tabExplorer1.BackgroundColorSelectedFin = System.Drawing.SystemColors.Window;
            this.tabExplorer1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.tabExplorer1.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.tabExplorer1.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.tabExplorer1.FocusColor = System.Drawing.Color.Orange;
            this.tabExplorer1.FocusTrack = true;
            this.tabExplorer1.HotTrack = true;
            this.tabExplorer1.ItemSize = new System.Drawing.Size(42, 18);
            this.tabExplorer1.Location = new System.Drawing.Point(6, 6);
            this.tabExplorer1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabExplorer1.Name = "tabExplorer1";
            this.tabExplorer1.Opacity = 1F;
            this.tabExplorer1.Overlap = 0;
            this.tabExplorer1.Radius = 2;
            this.tabExplorer1.SelectedIndex = 0;
            this.tabExplorer1.Size = new System.Drawing.Size(992, 821);
            this.tabExplorer1.StyleText = System.Drawing.FontStyle.Regular;
            this.tabExplorer1.StyleTextSelected = System.Drawing.FontStyle.Regular;
            this.tabExplorer1.TabIndex = 0;
            this.tabExplorer1.TextColor = System.Drawing.SystemColors.ControlText;
            this.tabExplorer1.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.tabExplorer1.TextColorSelected = System.Drawing.SystemColors.ControlText;
            // 
            // FormExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1938, 879);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FormExplorer";
            this.Text = "Explorateur de fichier";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ComposantsWinForm.TabExplorer tabExplorer2;
        private ComposantsWinForm.TabExplorer tabExplorer1;
    }
}

