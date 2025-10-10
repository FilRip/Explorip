namespace CoolBytes.ScriptInterpreter.Controls;

partial class ScriptInterpreter
{
    private System.ComponentModel.IContainer components = null;

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
            this.gbScript = new System.Windows.Forms.GroupBox();
            this.ChkOtherThread = new System.Windows.Forms.CheckBox();
            this.LblCommentaryColor = new System.Windows.Forms.LinkLabel();
            this.cbDataContract = new System.Windows.Forms.ComboBox();
            this.lblDataContract = new System.Windows.Forms.Label();
            this.chkSerializationDataContract = new System.Windows.Forms.CheckBox();
            this.btnAddUsing = new System.Windows.Forms.Button();
            this.txtListUsing = new System.Windows.Forms.TextBox();
            this.lbLanguage = new System.Windows.Forms.ListBox();
            this.LblNamespaceColor = new System.Windows.Forms.LinkLabel();
            this.chkYAXProperties = new System.Windows.Forms.CheckBox();
            this.chkYAXFields = new System.Windows.Forms.CheckBox();
            this.LblMethodsColor = new System.Windows.Forms.LinkLabel();
            this.LblPropertiesColor = new System.Windows.Forms.LinkLabel();
            this.LblFieldsColor = new System.Windows.Forms.LinkLabel();
            this.lbListFieldsScript = new System.Windows.Forms.ListBox();
            this.chkIntellisenseScript = new System.Windows.Forms.CheckBox();
            this.btnLoadXml = new System.Windows.Forms.Button();
            this.lblMaxRecursiveScript = new System.Windows.Forms.Label();
            this.numMaxRecursiveScript = new System.Windows.Forms.NumericUpDown();
            this.chkInheritsScript = new System.Windows.Forms.CheckBox();
            this.chkOpenFormScriptReturn = new System.Windows.Forms.CheckBox();
            this.btnLoadScript = new System.Windows.Forms.Button();
            this.btnSaveScript = new System.Windows.Forms.Button();
            this.btnExecuteScript = new System.Windows.Forms.Button();
            this.txtScriptResult = new System.Windows.Forms.TextBox();
            this.lblExecuteScriptReturn = new System.Windows.Forms.Label();
            this.txtScript = new System.Windows.Forms.RichTextBox();
            this.ContextMenuScript = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.gbScript.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRecursiveScript)).BeginInit();
            this.ContextMenuScript.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbScript
            // 
            this.gbScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbScript.Controls.Add(this.ChkOtherThread);
            this.gbScript.Controls.Add(this.LblCommentaryColor);
            this.gbScript.Controls.Add(this.cbDataContract);
            this.gbScript.Controls.Add(this.lblDataContract);
            this.gbScript.Controls.Add(this.chkSerializationDataContract);
            this.gbScript.Controls.Add(this.btnAddUsing);
            this.gbScript.Controls.Add(this.txtListUsing);
            this.gbScript.Controls.Add(this.lbLanguage);
            this.gbScript.Controls.Add(this.LblNamespaceColor);
            this.gbScript.Controls.Add(this.chkYAXProperties);
            this.gbScript.Controls.Add(this.chkYAXFields);
            this.gbScript.Controls.Add(this.LblMethodsColor);
            this.gbScript.Controls.Add(this.LblPropertiesColor);
            this.gbScript.Controls.Add(this.LblFieldsColor);
            this.gbScript.Controls.Add(this.lbListFieldsScript);
            this.gbScript.Controls.Add(this.chkIntellisenseScript);
            this.gbScript.Controls.Add(this.btnLoadXml);
            this.gbScript.Controls.Add(this.lblMaxRecursiveScript);
            this.gbScript.Controls.Add(this.numMaxRecursiveScript);
            this.gbScript.Controls.Add(this.chkInheritsScript);
            this.gbScript.Controls.Add(this.chkOpenFormScriptReturn);
            this.gbScript.Controls.Add(this.btnLoadScript);
            this.gbScript.Controls.Add(this.btnSaveScript);
            this.gbScript.Controls.Add(this.btnExecuteScript);
            this.gbScript.Controls.Add(this.txtScriptResult);
            this.gbScript.Controls.Add(this.lblExecuteScriptReturn);
            this.gbScript.Controls.Add(this.txtScript);
            this.gbScript.Location = new System.Drawing.Point(0, 0);
            this.gbScript.Name = "gbScript";
            this.gbScript.Size = new System.Drawing.Size(692, 389);
            this.gbScript.TabIndex = 15;
            this.gbScript.TabStop = false;
            this.gbScript.Text = " Script ";
            // 
            // ChkOtherThread
            // 
            this.ChkOtherThread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ChkOtherThread.AutoSize = true;
            this.ChkOtherThread.Location = new System.Drawing.Point(108, 217);
            this.ChkOtherThread.Name = "ChkOtherThread";
            this.ChkOtherThread.Size = new System.Drawing.Size(148, 17);
            this.ChkOtherThread.TabIndex = 28;
            this.ChkOtherThread.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_OTHERTHREAD;
            this.ChkOtherThread.UseVisualStyleBackColor = true;
            // 
            // LblCommentaryColor
            // 
            this.LblCommentaryColor.ActiveLinkColor = System.Drawing.Color.LightGray;
            this.LblCommentaryColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblCommentaryColor.AutoSize = true;
            this.LblCommentaryColor.DisabledLinkColor = System.Drawing.Color.LightGray;
            this.LblCommentaryColor.LinkColor = System.Drawing.Color.LightGray;
            this.LblCommentaryColor.Location = new System.Drawing.Point(598, 216);
            this.LblCommentaryColor.Name = "LblCommentaryColor";
            this.LblCommentaryColor.Size = new System.Drawing.Size(56, 13);
            this.LblCommentaryColor.TabIndex = 27;
            this.LblCommentaryColor.TabStop = true;
            this.LblCommentaryColor.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.LINK_COMMENTARY;
            this.LblCommentaryColor.VisitedLinkColor = System.Drawing.Color.LightGray;
            this.LblCommentaryColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CommentaryColor_LinkClicked);
            // 
            // cbDataContract
            // 
            this.cbDataContract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDataContract.FormattingEnabled = true;
            this.cbDataContract.Location = new System.Drawing.Point(451, 356);
            this.cbDataContract.Name = "cbDataContract";
            this.cbDataContract.Size = new System.Drawing.Size(137, 21);
            this.cbDataContract.TabIndex = 26;
            this.cbDataContract.Visible = false;
            this.cbDataContract.VisibleChanged += new System.EventHandler(this.CbDataContrat_VisibleChanged);
            // 
            // lblDataContract
            // 
            this.lblDataContract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDataContract.AutoSize = true;
            this.lblDataContract.Location = new System.Drawing.Point(326, 359);
            this.lblDataContract.Name = "lblDataContract";
            this.lblDataContract.Size = new System.Drawing.Size(105, 13);
            this.lblDataContract.TabIndex = 25;
            this.lblDataContract.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.LBL_DATACONTRACT_NAME;
            this.lblDataContract.Visible = false;
            // 
            // chkSerializationDataContract
            // 
            this.chkSerializationDataContract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSerializationDataContract.AutoSize = true;
            this.chkSerializationDataContract.Location = new System.Drawing.Point(164, 244);
            this.chkSerializationDataContract.Name = "chkSerializationDataContract";
            this.chkSerializationDataContract.Size = new System.Drawing.Size(162, 17);
            this.chkSerializationDataContract.TabIndex = 24;
            this.chkSerializationDataContract.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_SERIALIZATION_DATACONTACT;
            this.chkSerializationDataContract.UseVisualStyleBackColor = true;
            this.chkSerializationDataContract.CheckedChanged += new System.EventHandler(this.ChkSerializationDataContract_CheckedChanged);
            // 
            // btnAddUsing
            // 
            this.btnAddUsing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddUsing.Location = new System.Drawing.Point(437, 213);
            this.btnAddUsing.Name = "btnAddUsing";
            this.btnAddUsing.Size = new System.Drawing.Size(151, 23);
            this.btnAddUsing.TabIndex = 23;
            this.btnAddUsing.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.BTN_MODITY_LIST_USINGS;
            this.btnAddUsing.UseVisualStyleBackColor = true;
            this.btnAddUsing.Click += new System.EventHandler(this.BtnAddUsing_Click);
            // 
            // txtListUsing
            // 
            this.txtListUsing.AcceptsReturn = true;
            this.txtListUsing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtListUsing.Location = new System.Drawing.Point(377, 115);
            this.txtListUsing.Multiline = true;
            this.txtListUsing.Name = "txtListUsing";
            this.txtListUsing.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtListUsing.Size = new System.Drawing.Size(211, 96);
            this.txtListUsing.TabIndex = 22;
            this.txtListUsing.Visible = false;
            this.txtListUsing.Leave += new System.EventHandler(this.TxtListUsing_Leave);
            // 
            // lbLanguage
            // 
            this.lbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLanguage.FormattingEnabled = true;
            this.lbLanguage.Location = new System.Drawing.Point(595, 11);
            this.lbLanguage.Name = "lbLanguage";
            this.lbLanguage.Size = new System.Drawing.Size(91, 30);
            this.lbLanguage.TabIndex = 21;
            this.lbLanguage.SelectedIndexChanged += new System.EventHandler(this.LbLanguage_SelectedIndexChanged);
            // 
            // LblNamespaceColor
            // 
            this.LblNamespaceColor.ActiveLinkColor = System.Drawing.Color.Green;
            this.LblNamespaceColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblNamespaceColor.AutoSize = true;
            this.LblNamespaceColor.DisabledLinkColor = System.Drawing.Color.Green;
            this.LblNamespaceColor.LinkColor = System.Drawing.Color.Green;
            this.LblNamespaceColor.Location = new System.Drawing.Point(598, 194);
            this.LblNamespaceColor.Name = "LblNamespaceColor";
            this.LblNamespaceColor.Size = new System.Drawing.Size(64, 13);
            this.LblNamespaceColor.TabIndex = 20;
            this.LblNamespaceColor.TabStop = true;
            this.LblNamespaceColor.Text = Properties.Resources.LINK_NAMESPACE;
            this.LblNamespaceColor.VisitedLinkColor = System.Drawing.Color.Green;
            this.LblNamespaceColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NamespaceColor_LinkClicked);
            // 
            // chkYAXProperties
            // 
            this.chkYAXProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkYAXProperties.AutoSize = true;
            this.chkYAXProperties.Checked = true;
            this.chkYAXProperties.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkYAXProperties.Location = new System.Drawing.Point(456, 244);
            this.chkYAXProperties.Name = "chkYAXProperties";
            this.chkYAXProperties.Size = new System.Drawing.Size(132, 17);
            this.chkYAXProperties.TabIndex = 19;
            this.chkYAXProperties.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_READ_PUBLIC_PROPERTY;
            this.chkYAXProperties.UseVisualStyleBackColor = true;
            // 
            // chkYAXFields
            // 
            this.chkYAXFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkYAXFields.AutoSize = true;
            this.chkYAXFields.Checked = true;
            this.chkYAXFields.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkYAXFields.Location = new System.Drawing.Point(352, 244);
            this.chkYAXFields.Name = "chkYAXFields";
            this.chkYAXFields.Size = new System.Drawing.Size(79, 17);
            this.chkYAXFields.TabIndex = 18;
            this.chkYAXFields.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_READ_FIELDS;
            this.chkYAXFields.UseVisualStyleBackColor = true;
            // 
            // LblMethodsColor
            // 
            this.LblMethodsColor.ActiveLinkColor = System.Drawing.Color.Magenta;
            this.LblMethodsColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblMethodsColor.AutoSize = true;
            this.LblMethodsColor.DisabledLinkColor = System.Drawing.Color.Magenta;
            this.LblMethodsColor.LinkColor = System.Drawing.Color.Magenta;
            this.LblMethodsColor.Location = new System.Drawing.Point(598, 172);
            this.LblMethodsColor.Name = "LblMethodsColor";
            this.LblMethodsColor.Size = new System.Drawing.Size(48, 13);
            this.LblMethodsColor.TabIndex = 17;
            this.LblMethodsColor.TabStop = true;
            this.LblMethodsColor.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.LINK_METHOD;
            this.LblMethodsColor.VisitedLinkColor = System.Drawing.Color.Magenta;
            this.LblMethodsColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MethodsColor_LinkClicked);
            // 
            // LblPropertiesColor
            // 
            this.LblPropertiesColor.ActiveLinkColor = System.Drawing.Color.Blue;
            this.LblPropertiesColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblPropertiesColor.AutoSize = true;
            this.LblPropertiesColor.DisabledLinkColor = System.Drawing.Color.Blue;
            this.LblPropertiesColor.LinkColor = System.Drawing.Color.Blue;
            this.LblPropertiesColor.Location = new System.Drawing.Point(598, 148);
            this.LblPropertiesColor.Name = "LblPropertiesColor";
            this.LblPropertiesColor.Size = new System.Drawing.Size(54, 13);
            this.LblPropertiesColor.TabIndex = 16;
            this.LblPropertiesColor.TabStop = true;
            this.LblPropertiesColor.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.LINK_PROPERTY;
            this.LblPropertiesColor.VisitedLinkColor = System.Drawing.Color.Blue;
            this.LblPropertiesColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PropertiesColor_LinkClicked);
            // 
            // LblFieldsColor
            // 
            this.LblFieldsColor.ActiveLinkColor = System.Drawing.Color.Gray;
            this.LblFieldsColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblFieldsColor.AutoSize = true;
            this.LblFieldsColor.DisabledLinkColor = System.Drawing.Color.Gray;
            this.LblFieldsColor.LinkColor = System.Drawing.Color.Gray;
            this.LblFieldsColor.Location = new System.Drawing.Point(598, 125);
            this.LblFieldsColor.Name = "LblFieldsColor";
            this.LblFieldsColor.Size = new System.Drawing.Size(34, 13);
            this.LblFieldsColor.TabIndex = 15;
            this.LblFieldsColor.TabStop = true;
            this.LblFieldsColor.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.LINK_FIELD;
            this.LblFieldsColor.VisitedLinkColor = System.Drawing.Color.Gray;
            this.LblFieldsColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FieldsColor_LinkClicked);
            // 
            // lbListFieldsScript
            // 
            this.lbListFieldsScript.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbListFieldsScript.FormattingEnabled = true;
            this.lbListFieldsScript.HorizontalScrollbar = true;
            this.lbListFieldsScript.Location = new System.Drawing.Point(377, 129);
            this.lbListFieldsScript.Name = "lbListFieldsScript";
            this.lbListFieldsScript.ScrollAlwaysVisible = true;
            this.lbListFieldsScript.Size = new System.Drawing.Size(211, 82);
            this.lbListFieldsScript.TabIndex = 14;
            this.lbListFieldsScript.Visible = false;
            this.lbListFieldsScript.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LbListeChampsScript_DrawItem);
            this.lbListFieldsScript.VisibleChanged += new System.EventHandler(this.LbListFieldsScript_VisibleChanged);
            this.lbListFieldsScript.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LbListFieldsScript_MouseDoubleClick);
            // 
            // chkIntellisenseScript
            // 
            this.chkIntellisenseScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkIntellisenseScript.AutoSize = true;
            this.chkIntellisenseScript.Checked = true;
            this.chkIntellisenseScript.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIntellisenseScript.Location = new System.Drawing.Point(6, 217);
            this.chkIntellisenseScript.Name = "chkIntellisenseScript";
            this.chkIntellisenseScript.Size = new System.Drawing.Size(78, 17);
            this.chkIntellisenseScript.TabIndex = 11;
            this.chkIntellisenseScript.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_INTELLISENSE;
            this.chkIntellisenseScript.UseVisualStyleBackColor = true;
            this.chkIntellisenseScript.CheckedChanged += new System.EventHandler(this.ChkActiveIntellisense_CheckedChanged);
            // 
            // btnLoadXml
            // 
            this.btnLoadXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadXml.Location = new System.Drawing.Point(6, 356);
            this.btnLoadXml.Name = "btnLoadXml";
            this.btnLoadXml.Size = new System.Drawing.Size(242, 23);
            this.btnLoadXml.TabIndex = 10;
            this.btnLoadXml.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.BTN_LOAD_STATE;
            this.btnLoadXml.UseVisualStyleBackColor = true;
            this.btnLoadXml.Click += new System.EventHandler(this.BtnLoadXml_Click);
            // 
            // lblMaxRecursiveScript
            // 
            this.lblMaxRecursiveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaxRecursiveScript.AutoSize = true;
            this.lblMaxRecursiveScript.Location = new System.Drawing.Point(591, 285);
            this.lblMaxRecursiveScript.Name = "lblMaxRecursiveScript";
            this.lblMaxRecursiveScript.Size = new System.Drawing.Size(76, 13);
            this.lblMaxRecursiveScript.TabIndex = 9;
            this.lblMaxRecursiveScript.Text = Properties.Resources.LBL_MAX_RECURSIVE;
            // 
            // numMaxRecursiveScript
            // 
            this.numMaxRecursiveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numMaxRecursiveScript.Location = new System.Drawing.Point(594, 302);
            this.numMaxRecursiveScript.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxRecursiveScript.Name = "numMaxRecursiveScript";
            this.numMaxRecursiveScript.Size = new System.Drawing.Size(92, 20);
            this.numMaxRecursiveScript.TabIndex = 8;
            this.numMaxRecursiveScript.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // chkInheritsScript
            // 
            this.chkInheritsScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInheritsScript.AutoSize = true;
            this.chkInheritsScript.Checked = true;
            this.chkInheritsScript.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInheritsScript.Location = new System.Drawing.Point(602, 265);
            this.chkInheritsScript.Name = "chkInheritsScript";
            this.chkInheritsScript.Size = new System.Drawing.Size(84, 17);
            this.chkInheritsScript.TabIndex = 7;
            this.chkInheritsScript.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_WITH_INHERITS;
            this.chkInheritsScript.UseVisualStyleBackColor = true;
            // 
            // chkOpenFormScriptReturn
            // 
            this.chkOpenFormScriptReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOpenFormScriptReturn.Checked = true;
            this.chkOpenFormScriptReturn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOpenFormScriptReturn.Location = new System.Drawing.Point(594, 324);
            this.chkOpenFormScriptReturn.Name = "chkOpenFormScriptReturn";
            this.chkOpenFormScriptReturn.Size = new System.Drawing.Size(92, 30);
            this.chkOpenFormScriptReturn.TabIndex = 6;
            this.chkOpenFormScriptReturn.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.CHK_OPEN_WINDOW;
            this.chkOpenFormScriptReturn.UseVisualStyleBackColor = true;
            // 
            // btnLoadScript
            // 
            this.btnLoadScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadScript.Location = new System.Drawing.Point(594, 71);
            this.btnLoadScript.Name = "btnLoadScript";
            this.btnLoadScript.Size = new System.Drawing.Size(92, 23);
            this.btnLoadScript.TabIndex = 5;
            this.btnLoadScript.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.BTN_LOADSCRIPT;
            this.btnLoadScript.UseVisualStyleBackColor = true;
            this.btnLoadScript.Click += new System.EventHandler(this.BtnLoadScript_Click);
            // 
            // btnSaveScript
            // 
            this.btnSaveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveScript.Location = new System.Drawing.Point(594, 97);
            this.btnSaveScript.Name = "btnSaveScript";
            this.btnSaveScript.Size = new System.Drawing.Size(92, 23);
            this.btnSaveScript.TabIndex = 4;
            this.btnSaveScript.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.BTN_SAVESCRIPT;
            this.btnSaveScript.UseVisualStyleBackColor = true;
            this.btnSaveScript.Click += new System.EventHandler(this.BtnSaveScript_Click);
            // 
            // btnExecuteScript
            // 
            this.btnExecuteScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteScript.Location = new System.Drawing.Point(594, 43);
            this.btnExecuteScript.Name = "btnExecuteScript";
            this.btnExecuteScript.Size = new System.Drawing.Size(92, 23);
            this.btnExecuteScript.TabIndex = 3;
            this.btnExecuteScript.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.BTN_EXECUTE;
            this.btnExecuteScript.UseVisualStyleBackColor = true;
            this.btnExecuteScript.Click += new System.EventHandler(this.BtnExecuteScript_Click);
            // 
            // txtScriptResult
            // 
            this.txtScriptResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptResult.Location = new System.Drawing.Point(6, 266);
            this.txtScriptResult.Multiline = true;
            this.txtScriptResult.Name = "txtScriptResult";
            this.txtScriptResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtScriptResult.Size = new System.Drawing.Size(582, 84);
            this.txtScriptResult.TabIndex = 2;
            // 
            // lblExecuteScriptReturn
            // 
            this.lblExecuteScriptReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblExecuteScriptReturn.AutoSize = true;
            this.lblExecuteScriptReturn.Location = new System.Drawing.Point(6, 250);
            this.lblExecuteScriptReturn.Name = "lblExecuteScriptReturn";
            this.lblExecuteScriptReturn.Size = new System.Drawing.Size(45, 13);
            this.lblExecuteScriptReturn.TabIndex = 1;
            this.lblExecuteScriptReturn.Text = global::CoolBytes.ScriptInterpreter.Properties.Resources.LBL_RESULT;
            // 
            // txtScript
            // 
            this.txtScript.AcceptsTab = true;
            this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScript.ContextMenuStrip = this.ContextMenuScript;
            this.txtScript.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScript.Location = new System.Drawing.Point(6, 19);
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(582, 192);
            this.txtScript.TabIndex = 0;
            this.txtScript.Text = "";
            this.txtScript.WordWrap = false;
            this.txtScript.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtScript_KeyDown);
            this.txtScript.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtScript_KeyPress);
            this.txtScript.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtScript_KeyUp);
            // 
            // ContextMenuScript
            // 
            this.ContextMenuScript.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemSelectAll,
            this.toolStripSeparator1,
            this.MenuItemCut,
            this.MenuItemCopy,
            this.MenuItemPaste});
            this.ContextMenuScript.Name = "ContextMenuScript";
            this.ContextMenuScript.Size = new System.Drawing.Size(165, 98);
            this.ContextMenuScript.VisibleChanged += new System.EventHandler(this.ContextMenuScript_VisibleChanged);
            // 
            // MenuItemSelectAll
            // 
            this.MenuItemSelectAll.Name = "MenuItemSelectAll";
            this.MenuItemSelectAll.Size = new System.Drawing.Size(164, 22);
            this.MenuItemSelectAll.Text = Properties.Resources.MENUITEM_SELECT_ALL;
            this.MenuItemSelectAll.Click += new System.EventHandler(this.MenuItemSelectAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
            // 
            // MenuItemCut
            // 
            this.MenuItemCut.Name = "MenuItemCut";
            this.MenuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.MenuItemCut.Size = new System.Drawing.Size(164, 22);
            this.MenuItemCut.Text = Properties.Resources.MENUITEM_CUT;
            this.MenuItemCut.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);
            // 
            // MenuItemCopy
            // 
            this.MenuItemCopy.Name = "MenuItemCopy";
            this.MenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.MenuItemCopy.Size = new System.Drawing.Size(164, 22);
            this.MenuItemCopy.Text = Properties.Resources.MENUITEM_COPY;
            this.MenuItemCopy.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // MenuItemPaste
            // 
            this.MenuItemPaste.Name = "MenuItemPaste";
            this.MenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.MenuItemPaste.Size = new System.Drawing.Size(164, 22);
            this.MenuItemPaste.Text = Properties.Resources.MENUITEM_PASTE;
            this.MenuItemPaste.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // ScriptInterpreter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbScript);
            this.Name = "ScriptInterpreter";
            this.Size = new System.Drawing.Size(694, 390);
            this.gbScript.ResumeLayout(false);
            this.gbScript.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRecursiveScript)).EndInit();
            this.ContextMenuScript.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.GroupBox gbScript;
    private System.Windows.Forms.ListBox lbLanguage;
    internal System.Windows.Forms.LinkLabel LblNamespaceColor;
    internal System.Windows.Forms.CheckBox chkYAXProperties;
    internal System.Windows.Forms.CheckBox chkYAXFields;
    internal System.Windows.Forms.LinkLabel LblMethodsColor;
    internal System.Windows.Forms.LinkLabel LblPropertiesColor;
    internal System.Windows.Forms.LinkLabel LblFieldsColor;
    internal System.Windows.Forms.ListBox lbListFieldsScript;
    internal System.Windows.Forms.CheckBox chkIntellisenseScript;
    internal System.Windows.Forms.Button btnLoadXml;
    internal System.Windows.Forms.Label lblMaxRecursiveScript;
    internal System.Windows.Forms.NumericUpDown numMaxRecursiveScript;
    internal System.Windows.Forms.CheckBox chkInheritsScript;
    internal System.Windows.Forms.CheckBox chkOpenFormScriptReturn;
    internal System.Windows.Forms.Button btnLoadScript;
    internal System.Windows.Forms.Button btnSaveScript;
    internal System.Windows.Forms.Button btnExecuteScript;
    internal System.Windows.Forms.TextBox txtScriptResult;
    internal System.Windows.Forms.Label lblExecuteScriptReturn;
    internal System.Windows.Forms.RichTextBox txtScript;
    private System.Windows.Forms.ColorDialog colorPicker;
    private System.Windows.Forms.TextBox txtListUsing;
    private System.Windows.Forms.Button btnAddUsing;
    internal System.Windows.Forms.CheckBox chkSerializationDataContract;
    private System.Windows.Forms.ComboBox cbDataContract;
    private System.Windows.Forms.Label lblDataContract;
    internal System.Windows.Forms.LinkLabel LblCommentaryColor;
    private System.Windows.Forms.ContextMenuStrip ContextMenuScript;
    private System.Windows.Forms.ToolStripMenuItem MenuItemCut;
    private System.Windows.Forms.ToolStripMenuItem MenuItemCopy;
    private System.Windows.Forms.ToolStripMenuItem MenuItemPaste;
    private System.Windows.Forms.ToolStripMenuItem MenuItemSelectAll;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    internal System.Windows.Forms.CheckBox ChkOtherThread;
}
