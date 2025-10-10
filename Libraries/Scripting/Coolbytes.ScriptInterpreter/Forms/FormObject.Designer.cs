namespace CoolBytes.ScriptInterpreter.Forms;

partial class FormObject
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
        this.menuFormObject = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.CopyValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
        this.OpenAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.CloseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
        this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.tvObject = new System.Windows.Forms.TreeView();
        this.menuFormObject.SuspendLayout();
        this.SuspendLayout();
        // 
        // menuFormObjet
        // 
        this.menuFormObject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.CopyValueToolStripMenuItem,
        this.toolStripMenuItem2,
        this.OpenAllToolStripMenuItem,
        this.CloseAllToolStripMenuItem,
        this.ToolStripMenuItem1,
        this.SaveToolStripMenuItem});
        this.menuFormObject.Name = "menuFormObjet";
        this.menuFormObject.Size = new System.Drawing.Size(157, 104);
        // 
        // copierLaValeurToolStripMenuItem
        // 
        this.CopyValueToolStripMenuItem.Name = "copierLaValeurToolStripMenuItem";
        this.CopyValueToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
        this.CopyValueToolStripMenuItem.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.MENUITEM_COPY_VALUE;
        this.CopyValueToolStripMenuItem.Click += new System.EventHandler(this.CopyValueToolStripMenuItem_Click);
        // 
        // toolStripMenuItem2
        // 
        this.toolStripMenuItem2.Name = "toolStripMenuItem2";
        this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 6);
        // 
        // ToutOuvrirToolStripMenuItem
        // 
        this.OpenAllToolStripMenuItem.Name = "ToutOuvrirToolStripMenuItem";
        this.OpenAllToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
        this.OpenAllToolStripMenuItem.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.MENUITEM_EXPAND_ALL;
        this.OpenAllToolStripMenuItem.Click += new System.EventHandler(this.OpenAllToolStripMenuItem_Click);
        // 
        // ToutFermerToolStripMenuItem
        // 
        this.CloseAllToolStripMenuItem.Name = "ToutFermerToolStripMenuItem";
        this.CloseAllToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
        this.CloseAllToolStripMenuItem.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.MENUITEM_COLLAPSE_ALL;
        this.CloseAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
        // 
        // ToolStripMenuItem1
        // 
        this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
        this.ToolStripMenuItem1.Size = new System.Drawing.Size(153, 6);
        // 
        // SauvegarderToolStripMenuItem
        // 
        this.SaveToolStripMenuItem.Name = "SauvegarderToolStripMenuItem";
        this.SaveToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
        this.SaveToolStripMenuItem.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.MENUITEM_SAVE_STATE;
        this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
        // 
        // tvObjet
        // 
        this.tvObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this.tvObject.ContextMenuStrip = this.menuFormObject;
        this.tvObject.Location = new System.Drawing.Point(1, 1);
        this.tvObject.Name = "tvObjet";
        this.tvObject.Size = new System.Drawing.Size(428, 409);
        this.tvObject.TabIndex = 1;
        this.tvObject.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TvObject_MouseDown);
        // 
        // FormObjet
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(430, 411);
        this.Controls.Add(this.tvObject);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
        this.Name = "FormObject";
        this.Text = "FormObject";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormObject_FormClosing);
        this.menuFormObject.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.ContextMenuStrip menuFormObject;
    internal System.Windows.Forms.ToolStripMenuItem OpenAllToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem CloseAllToolStripMenuItem;
    internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
    internal System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
    internal System.Windows.Forms.TreeView tvObject;
    private System.Windows.Forms.ToolStripMenuItem CopyValueToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
}
