using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CoolBytes.Helpers;
using CoolBytes.Scripting.Enums;
using CoolBytes.Scripting.Helpers;
using CoolBytes.Scripting.Models;
using CoolBytes.ScriptInterpreter.Enums;
using CoolBytes.ScriptInterpreter.Helpers;
using CoolBytes.ScriptInterpreter.Interfaces;
using CoolBytes.ScriptInterpreter.Models;

using Microsoft.CodeAnalysis;

namespace CoolBytes.ScriptInterpreter.Controls;

public partial class ScriptInterpreter : UserControl, IScriptReturn, IIntellisense
{
    private object _startClass;
    private bool _executeOnProcess;

    public event Delegates.DelegateChangeLanguage ChangeLanguage;

    public ScriptInterpreter()
    {
        InitializeComponent();

        if (ModeDesign())
            return;

        Initialise();
    }

    private bool ModeDesign()
    {
        if (DesignMode)
            return true;
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            return true;
        if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
            return true;
        return false;
    }

    private void Initialise()
    {
        txtScript.LostFocus += TxtScript_LostFocus;
        lbLanguage.Items.Clear();
        foreach (object item in Enum.GetValues(typeof(SupportedLanguage)))
            lbLanguage.Items.Add(ExtensionsEnum.GetEnumDescription(item));
        lbLanguage.SelectedIndex = 0;
        txtListUsing.Text = Properties.Resources.DEFAULTS_USING;
        AutoAddAllUsings = true;
        ListReferences = [];
        ListAssembly = [];
        ListAssemblyInMemory = null;
        if (!DesignMode)
        {
            ReloadAssembly();
            AssemblyDirectory = Environment.CurrentDirectory;
        }
        _executeOnProcess = true;
    }

    public void ExecuteOnProcess(bool surProcess)
    {
        _executeOnProcess = surProcess;
    }

    public void ReloadAssembly()
    {
        ListAssembly.AddRange(ExecuteScript.ListDefaultAssembly());
    }

    public void SetDefaultLanguage(SupportedLanguage language)
    {
        lbLanguage.SelectedItem = ExtensionsEnum.GetEnumDescription<SupportedLanguage>(language);
    }

    public void SetStartClass(object demarrage)
    {
        _startClass = demarrage;
    }

    public void AddUsing(string[] listeUsing)
    {
        if ((listeUsing != null) && (listeUsing.Length > 0))
        {
            StringBuilder sb = new();
            sb.AppendLine();
            foreach (string use in listeUsing.Where(use => !txtListUsing.Text.Split(Environment.NewLine).Contains(use, StringComparison.OrdinalIgnoreCase)))
                sb.AppendLine(use);
            txtListUsing.Text += sb.ToString();
        }
    }

    public void RemoveUsing(string[] listUsings)
    {
        if ((listUsings != null && listUsings.Length > 0))
        {
            listUsings = listUsings.RemoveAllNull();
            txtListUsing.Lines = txtListUsing.Lines.RemoveAll(ligne => listUsings.Contains(ligne, StringComparer.OrdinalIgnoreCase)).RemoveAll(ligne => string.IsNullOrWhiteSpace(ligne));
        }
    }

    public string[] ListUsings
    {
        get { return txtListUsing.Lines; }
        set { txtListUsing.Lines = value; }
    }

    public string StartClassType
    {
        get
        {
            if (_startClass != null)
            {
                Type typeClasse = _startClass.GetType();
                if (typeClasse == typeof(string))
                    return _startClass.ToString();
                return typeClasse.ToString();
            }
            return "";
        }
    }

    public bool SearchInReflection { get; set; }

    public List<Assembly> ListAssemblyInMemory { get; set; }

    public List<Assembly> ListAssembly { get; set; }

    public List<string> ListReferences { get; set; }

    public string AssemblyDirectory { get; set; }

    private delegate string DelegateTxtScript();
    public string TxtScript()
    {
        if (txtScript.InvokeRequired)
            return (string)txtScript.Invoke(new DelegateTxtScript(TxtScript));
        else
            return txtScript.Text;
    }

    public bool AutoAddAllUsings { get; set; }

    private bool LanguageIgnoreCase
    {
        get
        {
            if ((lbLanguage.SelectedItem == null) || (ExtensionsEnum.GetValueFromDescription<SupportedLanguage>(lbLanguage.SelectedItem.ToString()) == SupportedLanguage.VBNET))
                return true;
            else
                return false;
        }
    }

    private void TxtScript_LostFocus(object sender, EventArgs e)
    {
        if (lbListFieldsScript.Visible && !lbListFieldsScript.Focused)
            lbListFieldsScript.Visible = false;
    }

    public bool InternalDebug { get; set; }

    public event Delegates.DelegateExecuteScript ExecuterScript;

    private void BtnExecuteScript_Click(object sender, EventArgs e)
    {
        try
        {
            txtScriptResult.Text = "";
            ScriptingResult scriptResult = null;

            SupportedLanguage language;
            if (lbLanguage.SelectedItem == null)
                language = SupportedLanguage.VBNET;
            else
                language = ExtensionsEnum.GetValueFromDescription<SupportedLanguage>(lbLanguage.SelectedItem.ToString());

            if (language == SupportedLanguage.CSHARP && txtScript.Text.IndexOf(Environment.NewLine) < 0 && txtScript.Text.Trim()[txtScript.Text.Trim().Length - 1] != ';')
                txtScript.Text += ";";

            string[] listAssembly = [];
            if (ListAssembly != null)
                listAssembly = [.. ListAssembly.Select(ass => ass.FullName)];
            if (ListAssemblyInMemory != null)
                listAssembly = [.. listAssembly, .. ListAssemblyInMemory.Select(ass => ass.FullName).ToArray()];

            ExecuterScript?.Invoke(this, new ExecuteScriptEventArgs() { Inherits = WithInherits, Language = language, ListAssemblies = listAssembly, ListUsings = ListUsings, MaxRecursion = MaxRecursiveInherits, Script = txtScript.SelectionLength > 0 ? txtScript.SelectedText : txtScript.Text, SerializeDataContract = SerializationDataContract, DataContractName = DataContractName, MemberToSerialize = (WithFields ? YAXLib.YAXSerializationFields.AllFields : YAXLib.YAXSerializationFields.None) | (WithProperties ? YAXLib.YAXSerializationFields.PublicProperties : YAXLib.YAXSerializationFields.None) });

            if (!_executeOnProcess)
                return;

            string script = txtScript.SelectionLength > 0 ? txtScript.SelectedText : txtScript.Text;
            if (ChkOtherThread.Checked)
            {
                if (language == SupportedLanguage.CSHARP)
                    script = "System.Threading.Tasks.Task.Run(() => {" + Environment.NewLine + script + Environment.NewLine + "});";
                else if (language == SupportedLanguage.VBNET)
                    script = "System.Threading.Tasks.Task.Run(Sub()" + Environment.NewLine + script + Environment.NewLine + "End Sub)";
            }

            scriptResult = ExecuteScript.ExecuteScripts(script, [.. txtListUsing.Lines], language, ListAssembly, ListReferences, AutoAddAllUsings, AssemblyDirectory, ListAssemblyInMemory?.ToArray());

            InterpreteResult(scriptResult);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace, Properties.Resources.ERROR_EXECUTE_SCRIPT, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void InterpreteResult(ScriptingResult scriptResult)
    {
        if (scriptResult != null)
        {
            if (scriptResult.Success && scriptResult.ExceptionThrowed == null)
            {
                txtScriptResult.Text = Properties.Resources.SCRIPT_OK_RETURN;
                txtScriptResult.Text += scriptResult.Result == null ? Properties.Resources.SCRIPT_RETURN_VOID : scriptResult.Result.ToString();
                if (scriptResult.Result != null && !scriptResult.Result.GetType().IsPrimitive && scriptResult.Result is not string && scriptResult.Result is not Enum && scriptResult.Result is not DateTime && scriptResult.Result is not TimeSpan && chkOpenFormScriptReturn.Checked)
                {
                    Filename = null;
                    ObjectToSerialize = scriptResult.Result;
                    Thread monThread = new(new ThreadStart(OpenWindow));
                    monThread.Start();
                }
            }
            else
            {
                StringBuilder sb = new();
                sb.AppendLine(Properties.Resources.ERRORS);
                if (scriptResult.Errors != null)
                    foreach (Diagnostic erreur in scriptResult.Errors)
                        sb.AppendLine(erreur.ToString());
                if (scriptResult.ExceptionThrowed != null)
                    sb.AppendLine(scriptResult.ExceptionThrowed?.ToString());
                txtScriptResult.Text = sb.ToString();
            }
        }
    }

    private delegate void DelegateWriteScriptResult(string texte);
    private void WriteScriptResult(string texte)
    {
        if (txtScriptResult.InvokeRequired)
            txtScriptResult.Invoke(new DelegateWriteScriptResult(WriteScriptResult), texte);
        else
            txtScriptResult.Text += texte;
    }

    private delegate string DelegateReadDataContract();
    private string ReadDataContract()
    {
        if (cbDataContract.InvokeRequired)
            return cbDataContract.Invoke(new DelegateReadDataContract(ReadDataContract)).ToString();
        else
            return cbDataContract.Text;
    }

    public string DataContractName
    {
        get { return ReadDataContract(); }
    }

    public string TxtResult
    {
        get { return txtScriptResult.Text; }
        set { WriteScriptResult(value); }
    }

    public object ObjectToSerialize { get; set; }

    public string Filename { get; set; }

    public PlatformType CurrentPlatform
    {
        get { return PlatformType.WINFORM; }
    }

    public bool SerializationDataContract
    {
        get { return chkSerializationDataContract.Checked; }
    }

    public bool WithFields
    {
        get { return chkYAXFields.Checked; }
    }

    public bool WithProperties
    {
        get { return chkYAXProperties.Checked; }
    }

    public int MaxRecursiveInherits
    {
        get { return (int)numMaxRecursiveScript.Value; }
    }

    public bool WithInherits
    {
        get { return chkInheritsScript.Checked; }
    }

    public void OpenWindow()
    {
        Serialization.ShowResult(this);
    }

    public bool OpenInNewWindow
    {
        get { return chkOpenFormScriptReturn.Checked; }
        set { CheckedOpenInNewWindowChanged(value); }
    }

    private void CheckedOpenInNewWindowChanged(bool cochee)
    {
        if (chkOpenFormScriptReturn.InvokeRequired)
            chkOpenFormScriptReturn.Invoke(() => CheckedOpenInNewWindowChanged(cochee));
        else
            chkOpenFormScriptReturn.Checked = cochee;
    }

    private void BtnLoadScript_Click(object sender, EventArgs e)
    {
        try
        {
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new();
                string[] lines;
                lines = File.ReadAllLines(dialog.FileName);
                foreach (string line in lines)
                    sb.AppendLine(line);
                txtScript.Text = sb.ToString();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSaveScript_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dialog.FileName))
                    File.Delete(dialog.FileName);
                File.AppendAllText(dialog.FileName, txtScript.Text);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ChangeColorLink(LinkLabel lien, Color couleur)
    {
        lien.LinkColor = couleur;
        lien.ActiveLinkColor = couleur;
        lien.DisabledLinkColor = couleur;
        lien.VisitedLinkColor = couleur;
    }

    private void FieldsColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (colorPicker.ShowDialog() == DialogResult.OK)
            ChangeColorLink(LblFieldsColor, colorPicker.Color);
    }

    private void PropertiesColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (colorPicker.ShowDialog() == DialogResult.OK)
            ChangeColorLink(LblPropertiesColor, colorPicker.Color);
    }

    private void MethodsColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (colorPicker.ShowDialog() == DialogResult.OK)
            ChangeColorLink(LblMethodsColor, colorPicker.Color);
    }

    private void NamespaceColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (colorPicker.ShowDialog() == DialogResult.OK)
            ChangeColorLink(LblNamespaceColor, colorPicker.Color);
    }

    private void ChkActiveIntellisense_CheckedChanged(object sender, EventArgs e)
    {
        if ((!chkIntellisenseScript.Checked) && lbListFieldsScript.Visible)
            lbListFieldsScript.Visible = false;
    }

    public void KeyboardInputIntellisense(KeyEventArgs e = null, System.Windows.Input.KeyEventArgs eWPF = null)
    {
        if (Enum.GetNames(typeof(System.Windows.Input.Key)).Any(nomTouche => nomTouche == e.KeyCode.ToString("G") && InterpreteCode.KeyboardInputIntellisense(this, (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), nomTouche))))
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
    }

    private void TxtScript_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            KeyboardInputIntellisense(e, null);
            if ((e.KeyCode == Keys.V && e.Modifiers.HasFlag(Keys.Control)) ||
                (e.KeyCode == Keys.Insert && e.Modifiers.HasFlag(Keys.Shift)))
            {
                string plainText = Clipboard.GetText();
                Clipboard.Clear();
                Clipboard.SetText(plainText);
            }
        }
        catch (Exception ex)
        {
            if (InternalDebug)
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    private delegate int DelegateTxtScriptSelectionStart();
    private int TxtScriptSelectionStart()
    {
        try
        {
            if (txtScript.InvokeRequired)
                return (int)txtScript.Invoke(new DelegateTxtScriptSelectionStart(TxtScriptSelectionStart));
            else
                return txtScript.SelectionStart;
        }
        catch (Exception) { return 0; }
    }

    private delegate int DelegatetxtScriptGetLineFromCharIndex(int charPos);
    private int TxtScriptGetLineFromCharIndex(int charPos)
    {
        try
        {
            if (txtScript.InvokeRequired)
                return (int)txtScript.Invoke(new DelegatetxtScriptGetLineFromCharIndex(TxtScriptGetLineFromCharIndex), charPos);
            else
                return txtScript.GetLineFromCharIndex(charPos);
        }
        catch (Exception) { return 0; }
    }

    private delegate string DelegatetxtScriptLignes(int numLigne);
    private string TxtScriptLignes(int numLigne)
    {
        try
        {
            if (txtScript.InvokeRequired)
                return (string)txtScript.Invoke(new DelegatetxtScriptLignes(TxtScriptLignes), numLigne);
            else
                return txtScript.Lines[numLigne];
        }
        catch (Exception) { return ""; }
    }

    private delegate int DelegatetxtScriptGetFirstCharIndexFromLine(int numLigne);
    private int TxtScriptGetFirstCharIndexFromLine(int numLigne)
    {
        try
        {
            if (txtScript.InvokeRequired)
                return (int)txtScript.Invoke(new DelegatetxtScriptGetFirstCharIndexFromLine(TxtScriptGetFirstCharIndexFromLine), numLigne);
            else
                return txtScript.GetFirstCharIndexFromLine(numLigne);
        }
        catch (Exception) { return 0; }
    }

    private delegate bool DelegateIgnoreCasse();
    private bool IgnoreCaseInternal()
    {
        if (lbLanguage.InvokeRequired)
            return (bool)lbLanguage.Invoke(new DelegateIgnoreCasse(IgnoreCaseInternal));
        else
            return LanguageIgnoreCase;
    }

    public bool IgnoreCase
    {
        get { return IgnoreCaseInternal(); }
    }

    private delegate void DelegatelbListeChampsScriptVisible(bool visible);
    private void LbListFieldsScriptVisible(bool visible)
    {
        if (lbListFieldsScript.InvokeRequired)
            lbListFieldsScript.Invoke(new DelegatelbListeChampsScriptVisible(LbListFieldsScriptVisible), visible);
        else
            lbListFieldsScript.Visible = visible;
    }

    private delegate bool DelegatelbListeChampsScriptEstVisible();
    private bool LbListFieldsScriptIsVisible()
    {
        if (lbListFieldsScript.InvokeRequired)
            return (bool)lbListFieldsScript.Invoke(new DelegatelbListeChampsScriptEstVisible(LbListFieldsScriptIsVisible));
        else
            return lbListFieldsScript.Visible;
    }

    private delegate void DelegateSetPosIntellisense();
    public void ResetPosIntellisense()
    {
        try
        {
            if (txtScript.InvokeRequired)
                txtScript.Invoke(new DelegateSetPosIntellisense(ResetPosIntellisense));
            else
            {
                Point cursorPosition;
                cursorPosition = txtScript.GetPositionFromCharIndex(TxtScriptSelectionStart() - 1);
                SizeF mesure;
                mesure = txtScript.CreateGraphics().MeasureString(".", txtScript.Font);
                lbListFieldsScript.Location = new Point(txtScript.Location.X + cursorPosition.X + (int)(mesure.Width * 2), txtScript.Location.Y + cursorPosition.Y + (int)(mesure.Height));
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private delegate void DelegatelbListFieldsScriptSetDataSource(object source);
    private void LbListFieldsScriptSetDataSource(object source)
    {
        if (lbListFieldsScript.InvokeRequired)
            lbListFieldsScript.Invoke(new DelegatelbListFieldsScriptSetDataSource(LbListFieldsScriptSetDataSource), source);
        else
            lbListFieldsScript.DataSource = source;
    }

    private delegate char DelegatetxtScriptReadChar(int index);
    public char ReadChar(int index)
    {
        try
        {
            if (txtScript.InvokeRequired)
                return (char)txtScript.Invoke(new DelegatetxtScriptReadChar(ReadChar), index);
            else
                return txtScript.Text[index];
        }
        catch (Exception) { return (char)0; }
    }

    private delegate string DelegatetxtScriptReadString(int start, int length);
    public string ReadString(int start, int length)
    {
        try
        {
            if (txtScript.InvokeRequired)
                return (string)txtScript.Invoke(new DelegatetxtScriptReadString(ReadString), start, length);
            else
                return txtScript.Text.Substring(start, length);
        }
        catch (Exception) { return ""; }
    }

    private delegate Color DelegateTxtScriptSelectionColor();
    private Color GetSelectionColor()
    {
        if (txtScript.InvokeRequired)
            return (Color)txtScript.Invoke(new DelegateTxtScriptSelectionColor(GetSelectionColor));
        else
            return txtScript.SelectionColor;
    }

    public Color SelectionColor
    {
        get { return GetSelectionColor(); }
    }

    private delegate void DelegateSetTxtScriptSelectionStart(int index);
    private void SetTxtScriptSelectionStart(int index)
    {
        if (txtScript.InvokeRequired)
            txtScript.Invoke(new DelegateSetTxtScriptSelectionStart(SetTxtScriptSelectionStart), index);
        else
            txtScript.SelectionStart = index;
    }

    private delegate void DelegateSetTxtScriptSelectionLength(int length);
    private void SetTxtScriptSelectionLength(int length)
    {
        if (txtScript.InvokeRequired)
            txtScript.Invoke(new DelegateSetTxtScriptSelectionLength(SetTxtScriptSelectionLength), length);
        else
            txtScript.SelectionLength = length;
    }

    private delegate void DelegateSetTxtScriptSelectionColor(Color couleur);
    public void SetSelectionColor(Color newColor)
    {
        if (txtScript.InvokeRequired)
            txtScript.Invoke(new DelegateSetTxtScriptSelectionColor(SetSelectionColor), newColor);
        else
            txtScript.SelectionColor = newColor;
    }

    private void TxtScript_KeyPress(object sender, KeyPressEventArgs e)
    {
        KeyboardInput(e.KeyChar);
        if ((e.KeyChar == (char)13) && ListVisible)
        {
            e.Handled = true;
            ResetPosIntellisense();
        }
        else if (e.KeyChar == (char)9 && !ListVisible)
        {
            e.Handled = true;
            int oldPosCurseur = PosCursor;
            txtScript.Text = txtScript.Text.Insert(txtScript.SelectionStart, new string(' ', 4));
            PosCursor = oldPosCurseur + 4;
        }
    }

    public void KeyboardInput(char key)
    {
        InterpreteCode.KeyboardInput(this, key);
    }

    private void TxtScript_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5)
        {
            BtnExecuteScript_Click(null, null);
            return;
        }

        if (!lbListFieldsScript.Visible)
        {
            if (e.KeyCode == Keys.Delete)
                KeyboardInput('\b');
            return;
        }

        if ((e.KeyCode == Keys.Escape) ||
            (e.KeyCode == Keys.Space) ||
            (e.KeyCode == Keys.Enter) ||
            (e.KeyCode == Keys.Return) ||
            (e.KeyCode == Keys.Tab))
        {
            if ((e.KeyCode == Keys.Enter) ||
                (e.KeyCode == Keys.Return) ||
                (e.KeyCode == Keys.Tab))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                LbListFieldsScript_MouseDoubleClick(sender, null);
            }
            lbListFieldsScript.Visible = false;
        }
    }

    private void BtnLoadXml_Click(object sender, EventArgs e)
    {
        try
        {
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Filename = dialog.FileName;
                ObjectToSerialize = null;
                Thread thread = new(new ThreadStart(OpenWindow));
                thread.Start();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LbListFieldsScript_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        try
        {
            if (lbListFieldsScript.SelectedItem != null)
            {
                int pos = txtScript.SelectionStart;
                int toRemove = txtScript.SelectionLength;
                if (toRemove == 0)
                {
                    while (txtScript.Text[pos - 1] != '.')
                    {
                        pos--;
                        toRemove++;
                    }
                    if (toRemove > 0)
                    {
                        txtScript.SelectionStart = pos;
                        txtScript.SelectionLength = toRemove;
                    }
                }
                if (toRemove > 0)
                    txtScript.SelectedText = "";
                string nom = ((OneElementType)lbListFieldsScript.SelectedItem).Name;
                txtScript.Text = txtScript.Text.Insert(pos, nom);
                txtScript.SelectionStart = pos + nom.Length;
                lbListFieldsScript.Visible = false;
                txtScript.Focus();
            }
        }
        catch { /* Ignore errors */ }
    }

    private delegate void DelegateLbListFieldsScriptVisible(object sender, EventArgs e);
    private void LbListFieldsScript_VisibleChanged(object sender, EventArgs e)
    {
        try
        {
            if (lbListFieldsScript.InvokeRequired)
            {
                lbListFieldsScript.Invoke(new DelegateLbListFieldsScriptVisible(LbListFieldsScript_VisibleChanged), sender, e);
                return;
            }
            if (lbListFieldsScript.Visible)
            {
                SizeF mesure;
                Graphics gfx = lbListFieldsScript.CreateGraphics();
                float maxWidth = 0;
                float sizeWithParam = 0;
                if (lbListFieldsScript.Items.Count > 0)
                    foreach (OneElementType item in lbListFieldsScript.Items.OfType<OneElementType>())
                    {
                        mesure = gfx.MeasureString(item.ToString(), lbListFieldsScript.Font);
                        sizeWithParam = mesure.Width;
                        if (item.ElementType == ElementTypes.PARAMETER)
                            sizeWithParam += 24;
                        maxWidth = Math.Max(sizeWithParam, maxWidth);
                    }
                maxWidth += 24; // Scrollbar size
                lbListFieldsScript.Width = Math.Max(24, (int)maxWidth);
            }
        }
        catch { /* Ignore errors */ }
    }

    private void LbListeChampsScript_DrawItem(object sender, DrawItemEventArgs e)
    {
        try
        {
            OneElementType element = (OneElementType)lbListFieldsScript.Items[e.Index];
            SolidBrush color = new(LblFieldsColor.LinkColor);
            string stringToDisplay = element.ToString();
            switch (element.ElementType)
            {
                case ElementTypes.PROPERTIES:
                    color = new SolidBrush(LblPropertiesColor.LinkColor);
                    break;
                case ElementTypes.METHOD:
                    color = new SolidBrush(LblMethodsColor.LinkColor);
                    break;
                case ElementTypes.NAMESPACE:
                    color = new SolidBrush(LblNamespaceColor.LinkColor);
                    break;
                case ElementTypes.PARAMETER:
                    color = new SolidBrush(Color.LightGray);
                    break;
            }
            e.DrawBackground();
            e.Graphics.DrawString(stringToDisplay, lbListFieldsScript.Font, color, e.Bounds);
            e.DrawFocusRectangle();
        }
        catch { /* Ignore errors */ }
    }

    private void BtnAddUsing_Click(object sender, EventArgs e)
    {
        txtScript.Focus();
        txtListUsing.Visible = !txtListUsing.Visible;
    }

    private void ChkSerializationDataContract_CheckedChanged(object sender, EventArgs e)
    {
        chkYAXFields.Enabled = !chkSerializationDataContract.Checked;
        chkYAXProperties.Enabled = !chkSerializationDataContract.Checked;
        chkInheritsScript.Enabled = !chkSerializationDataContract.Checked;
        numMaxRecursiveScript.Enabled = !chkSerializationDataContract.Checked;
        lblDataContract.Visible = chkSerializationDataContract.Checked;
        cbDataContract.Visible = chkSerializationDataContract.Checked;
    }

    private void CbDataContrat_VisibleChanged(object sender, EventArgs e)
    {
        if (cbDataContract.Visible)
        {
            cbDataContract.Items.Clear();
            cbDataContract.Text = "";
            foreach (Type type in ExtensionsAssembly.ListAllTypes().Where(item => !item.Assembly.GlobalAssemblyCache && !item.IsSpecialName && item.IsInterface && item.GetCustomAttribute<ServiceContractAttribute>() != null).ToList())
                cbDataContract.Items.Add(type.Name);
        }
    }

    private void CommentaryColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (colorPicker.ShowDialog() == DialogResult.OK)
            ChangeColorLink(LblCommentaryColor, colorPicker.Color);
        int lastPos = txtScript.SelectionStart;
        if (txtScript.Lines != null && txtScript.Lines.Length > 0)
            for (int numLigne = 0; numLigne < txtScript.Lines.Length; numLigne++)
                foreach (string commentaire in StartComments.ListStartComments(IgnoreCaseInternal()).Where(commentaire => txtScript.Lines[numLigne].Trim().StartsWith(commentaire)))
                {
                    SetTxtScriptSelectionStart(TxtScriptGetFirstCharIndexFromLine(numLigne));
                    SetTxtScriptSelectionLength(txtScript.Lines[numLigne].Length);
                    SetSelectionColor(colorPicker.Color);
                }
        txtScript.SelectionStart = lastPos;
    }

    private delegate Color DelegateGetCouleur();

    private Color GetCommentaryColor()
    {
        if (LblCommentaryColor.InvokeRequired)
            return (Color)LblCommentaryColor.Invoke(new DelegateGetCouleur(GetCommentaryColor));
        else
            return LblCommentaryColor.LinkColor;
    }

    private Color GetNamespaceColor()
    {
        if (LblNamespaceColor.InvokeRequired)
            return (Color)LblNamespaceColor.Invoke(new DelegateGetCouleur(GetNamespaceColor));
        else
            return LblNamespaceColor.LinkColor;
    }

    private Color GetPropertiesColor()
    {
        if (LblPropertiesColor.InvokeRequired)
            return (Color)LblPropertiesColor.Invoke(new DelegateGetCouleur(GetPropertiesColor));
        else
            return LblPropertiesColor.LinkColor;
    }

    private Color GetFieldsColor()
    {
        if (LblFieldsColor.InvokeRequired)
            return (Color)LblFieldsColor.Invoke(new DelegateGetCouleur(GetFieldsColor));
        else
            return LblFieldsColor.LinkColor;
    }

    private Color GetMethodsColor()
    {
        if (LblMethodsColor.InvokeRequired)
            return (Color)LblMethodsColor.Invoke(new DelegateGetCouleur(GetMethodsColor));
        else
            return LblMethodsColor.LinkColor;
    }

    public Color PropertiesColor
    {
        get { return GetPropertiesColor(); }
    }

    public Color MethodsColor
    {
        get { return GetMethodsColor(); }
    }

    public Color NamespaceColor
    {
        get { return GetNamespaceColor(); }
    }

    public Color CommentaryColor
    {
        get { return GetCommentaryColor(); }
    }

    public Color FieldsColor
    {
        get { return GetFieldsColor(); }
    }

    public int PositionList
    {
        get { return lbListFieldsScript.SelectedIndex; }
        set
        {
            if (ListVisible)
            {
                if (value < 0)
                    value = 0;
                if (value > lbListFieldsScript.Items.Count - 1)
                    value = lbListFieldsScript.Items.Count - 1;
                lbListFieldsScript.SelectedIndex = value;
            }
        }
    }

    public bool ListVisible
    {
        get { return LbListFieldsScriptIsVisible(); }
        set { LbListFieldsScriptVisible(value); }
    }

    public int NumOverrides { get; set; }

    public void SelectText(int start, int end)
    {
        SetTxtScriptSelectionStart(start);
        SetTxtScriptSelectionLength(end - start);
    }

    public int PosCursor
    {
        get { return TxtScriptSelectionStart(); }
        set { SetTxtScriptSelectionStart(value); }
    }

    public MethodInfo[] CurrentMethods { get; set; }

    public bool ActiveIntellisense
    {
        get { return chkIntellisenseScript.Checked; }
    }

    public int ReturnCurrentNumLine
    {
        get { return TxtScriptGetLineFromCharIndex(PosCursor); }
    }

    public string ReturnCurrentLine
    {
        get { return TxtScriptLignes(ReturnCurrentNumLine); }
    }

    public void FillIntellisenseList(List<OneElementType> list)
    {
        LbListFieldsScriptSetDataSource(list);
        LbListFieldsScript_VisibleChanged(null, null);
    }

    public List<OneElementType> ListIntellisense { get; set; }

    public string FilterScript { get; set; }

    private Color GetWriteColor()
    {
        if (txtScript.InvokeRequired)
            return (Color)txtScript.Invoke(new DelegateGetCouleur(GetWriteColor));
        else
            return txtScript.ForeColor;
    }

    public Color WriteColor
    {
        get { return GetWriteColor(); }
    }

    public int PosFirstCharCurrentLine
    {
        get { return TxtScriptGetFirstCharIndexFromLine(ReturnCurrentNumLine); }
    }

    public int PosLastCharCurrentLine
    {
        get { return TxtScriptGetFirstCharIndexFromLine(ReturnCurrentNumLine) + ReturnCurrentLine.Length; }
    }

    private void LbLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lbLanguage.SelectedItem != null && Enum.TryParse(lbLanguage.SelectedItem.ToString(), out SupportedLanguage langageCourant))
            ChangeLanguage?.Invoke(this, new ChangeLanguageEventArgs(langageCourant));
    }

    private void TxtListUsing_Leave(object sender, EventArgs e)
    {
        BtnAddUsing_Click(sender, e);
    }

    private void CutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        txtScript.Cut();
    }

    private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        txtScript.Copy();
    }

    private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
    {
        txtScript.Paste();
    }

    private void MenuItemSelectAll_Click(object sender, EventArgs e)
    {
        txtScript.SelectAll();
    }

    private void ContextMenuScript_VisibleChanged(object sender, EventArgs e)
    {
        MenuItemPaste.Enabled = Clipboard.ContainsText();
    }
}
