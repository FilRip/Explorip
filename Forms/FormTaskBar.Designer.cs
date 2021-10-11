namespace Filexplorip.Forms
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
            this.lvTaches = new System.Windows.Forms.ListView();
            this.colWindow = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 500;
            this.timerRefresh.Tick += new System.EventHandler(this.TimerRefresh_Tick);
            // 
            // lvTaches
            // 
            this.lvTaches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTaches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colWindow});
            this.lvTaches.HideSelection = false;
            this.lvTaches.Location = new System.Drawing.Point(1, 0);
            this.lvTaches.Name = "lvTaches";
            this.lvTaches.Size = new System.Drawing.Size(405, 449);
            this.lvTaches.TabIndex = 0;
            this.lvTaches.UseCompatibleStateImageBehavior = false;
            this.lvTaches.View = System.Windows.Forms.View.Details;
            // 
            // colWindow
            // 
            this.colWindow.Text = "Fenetre";
            this.colWindow.Width = 385;
            // 
            // FormTaskBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 450);
            this.Controls.Add(this.lvTaches);
            this.Name = "FormTaskBar";
            this.Text = "Barre des tâches";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.ListView lvTaches;
        private System.Windows.Forms.ColumnHeader colWindow;
    }
}