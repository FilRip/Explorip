namespace CoolBytes.ScriptInterpreter.Forms;

/// <summary>
/// Classe gérant l'interprétation/l'exécution de script Net Framework
/// </summary>
partial class FormScriptInterpreter
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
        this.colorPicker = new System.Windows.Forms.ColorDialog();
        this.interpreteScript1 = new CoolBytes.ScriptInterpreter.Controls.ScriptInterpreter();
        this.SuspendLayout();
        // 
        // interpreteScript1
        // 
        this.interpreteScript1.Location = new System.Drawing.Point(1, 1);
        this.interpreteScript1.Name = "interpreteScript1";
        this.interpreteScript1.Size = new System.Drawing.Size(694, 390);
        this.interpreteScript1.TabIndex = 0;
        // 
        // FormInterpreteScript
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(695, 392);
        this.Controls.Add(this.interpreteScript1);
        this.Name = "FormInterpreteScript";
        this.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.WINDOW_TITLE;
        this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ColorDialog colorPicker;
    private Controls.ScriptInterpreter interpreteScript1;
}