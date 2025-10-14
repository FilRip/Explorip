using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using CoolBytes.Helpers;
using CoolBytes.Scripting.Enums;
using CoolBytes.Scripting.Models;
using CoolBytes.ScriptInterpreter.Enums;
using CoolBytes.ScriptInterpreter.Helpers;
using CoolBytes.ScriptInterpreter.Interfaces;
using CoolBytes.ScriptInterpreter.Models;

using Microsoft.CodeAnalysis;
using Microsoft.Win32;

namespace CoolBytes.ScriptInterpreter.WPF.ViewModels;

internal class WpfInterpreteScriptViewModel : ViewModelBase, IScriptReturn, IIntellisense
{
    private string _usings;
    private bool _intellisense;
    private bool _withInherits;
    private int _maxRecursion;
    private bool _openWindow;
    private string _txtResult;
    private Brush _fieldColor;
    private Brush _propertyColor;
    private Brush _methodColor;
    private Brush _nameSpaceColor;
    private Brush _commentaryColor;
    private bool _listFields;
    private bool _listProperties;
    private List<string> _listLanguages;
    private string _selectedLanguage;
    private bool _visibleIntellisense;
    private bool _visibleUsings;
    private int _selectionIntellisense;
    private bool _chkSerializationDataContract;
    private string _selectionDataContract;
    private List<string> _listDataContract;
    private bool _executeOnProcess;
    private bool _otherThread;

    public ICommand BtnExecuteScript { get; set; }
    public ICommand BtnAddUsing { get; set; }
    public ICommand BtnLoadScript { get; set; }
    public ICommand BtnSaveScript { get; set; }
    public ICommand ClickField { get; set; }
    public ICommand ClickProperty { get; set; }
    public ICommand ClickMethod { get; set; }
    public ICommand ClickNameSpace { get; set; }
    public ICommand ClickCommentary { get; set; }
    public ICommand BtnLoadObject { get; set; }
    public ICommand CutText { get; set; }
    public ICommand CopyText { get; set; }
    public ICommand PasteText { get; set; }
    public ICommand SelectAll { get; set; }

    private List<OneElementType> _listIntellisense;

    private readonly WpfInterpreteScript _parentWindow;
    private object _startClass;

    internal WpfInterpreteScriptViewModel(WpfInterpreteScript form)
    {
        Usings = Properties.Resources.DEFAULTS_USING;
        ActiveIntellisense = true;
        WithInherits = true;
        NumMaxRecursion = "4";
        ChkOpenWindow = true;
        TxtResult = "";
        FieldColorInternal = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.Gray.A, System.Drawing.Color.Gray.R, System.Drawing.Color.Gray.G, System.Drawing.Color.Gray.B));
        PropertyColor = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.Blue.A, System.Drawing.Color.Blue.R, System.Drawing.Color.Blue.G, System.Drawing.Color.Blue.B));
        MethodColor = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.Magenta.A, System.Drawing.Color.Magenta.R, System.Drawing.Color.Magenta.G, System.Drawing.Color.Magenta.B));
        NamespaceColorInternal = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.Green.A, System.Drawing.Color.Green.R, System.Drawing.Color.Green.G, System.Drawing.Color.Green.B));
        CommentColorInternal = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.LightGray.A, System.Drawing.Color.LightGray.R, System.Drawing.Color.LightGray.G, System.Drawing.Color.LightGray.B));
        WithFields = true;
        WithProperties = true;
        ListVisible = false;
        PopUpUsings = false;
        SerializationDataContract = false;

        BtnExecuteScript = new RelayCommand(sub => ExecuteScript());
        BtnAddUsing = new RelayCommand(sub => AddUsing());
        BtnLoadScript = new RelayCommand(sub => LoadScript());
        BtnSaveScript = new RelayCommand(sub => SaveScript());
        ClickField = new RelayCommand(sub => Click_HyperLink(nameof(FieldColorInternal)));
        ClickProperty = new RelayCommand(sub => Click_HyperLink(nameof(PropertyColor)));
        ClickMethod = new RelayCommand(sub => Click_HyperLink(nameof(MethodColor)));
        ClickNameSpace = new RelayCommand(sub => Click_HyperLink(nameof(NamespaceColorInternal)));
        ClickCommentary = new RelayCommand(sub => Click_HyperLink(nameof(CommentColorInternal)));
        BtnLoadObject = new RelayCommand(sub => LoadObject());

        CutText = new RelayCommand(sub => MenuCutText());
        CopyText = new RelayCommand(sub => MenuCopyText());
        PasteText = new RelayCommand(sub => MenuPasteText());
        SelectAll = new RelayCommand(sub => MenuSelectAll());

        _listLanguages = [];
        foreach (object item in Enum.GetValues(typeof(SupportedLanguage)))
            _listLanguages.Add(ExtensionsEnum.GetEnumDescription(item));
        RaisePropertyChanged(nameof(ListLanguages));
        LanguageSelected = ListLanguages[0];
        ReloadAssembly();
        ListReferences = [];
        AssemblyDirectory = Environment.CurrentDirectory;
        ListAssemblyInMemory = null;

        _parentWindow = form;
        AutoAddAllUsings = true;
        _executeOnProcess = true;
        OtherThread = false;
    }

    public bool SearchInReflection { get; set; }

    internal void ExecuteOnProcess(bool surProcess)
    {
        _executeOnProcess = surProcess;
    }

    internal void ReloadAssembly()
    {
        ListAssembly = Scripting.Helpers.ExecuteScript.ListDefaultAssembly();
    }

    internal void StartClass(object classeDemarrage)
    {
        _startClass = classeDemarrage;
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

    #region Binding

    internal List<Assembly> ListAssemblyInMemory { get; set; }

    internal List<Assembly> ListAssembly { get; set; }

    internal List<string> ListReferences { get; set; }

    internal string AssemblyDirectory { get; set; }

    public bool ListVisible
    {
        get { return _visibleIntellisense; }
        set
        {
            _visibleIntellisense = value;
            Application.Current.Dispatcher.Invoke(() => ResetPosIntellisense());
            RaisePropertyChanged();
        }
    }

    public bool NotCheckedSerialisationDataContract
    {
        get { return !SerializationDataContract; }
    }

    public bool AutoAddAllUsings { get; set; }

    public bool SerializationDataContract
    {
        get { return _chkSerializationDataContract; }
        set
        {
            _chkSerializationDataContract = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(NotCheckedSerialisationDataContract));
            if (value)
            {
                _listDataContract = [];
                foreach (Type type in ExtensionsAssembly.ListAllTypes().Where(item => !item.Assembly.GlobalAssemblyCache && !item.IsSpecialName && item.IsInterface && item.GetCustomAttribute<ServiceContractAttribute>() != null).ToList())
                    _listDataContract.Add(type.Name);
                RaisePropertyChanged(nameof(ListDataContract));
            }
        }
    }

    public string DataContractName
    {
        get { return _selectionDataContract; }
        set
        {
            _selectionDataContract = value;
            RaisePropertyChanged();
        }
    }

    public List<string> ListDataContract
    {
        get { return _listDataContract; }
        set
        {
            _listDataContract = value;
            RaisePropertyChanged();
            DataContractName = "";
        }
    }

    public List<OneElementType> ListFilteredIntellisense
    {
        get { return _listIntellisense; }
        set
        {
            _listIntellisense = value;
            RaisePropertyChanged();
        }
    }

    public List<OneElementType> ListIntellisense { get; set; }

    public bool WithInherits
    {
        get { return _withInherits; }
        set
        {
            _withInherits = value;
            RaisePropertyChanged();
        }
    }

    public string NumMaxRecursion
    {
        get { return _maxRecursion.ToString(); }
        set
        {
            int.TryParse(value, out _maxRecursion);
            if (_maxRecursion <= 0)
                _maxRecursion = 1;
            RaisePropertyChanged();
        }
    }

    public int MaxRecursiveInherits
    {
        get { return _maxRecursion; }
    }

    public List<string> ListLanguages
    {
        get { return _listLanguages; }
        set
        {
            _listLanguages = value;
            RaisePropertyChanged();
        }
    }

    public event Delegates.DelegateChangeLanguage ChangeLangage;

    public string LanguageSelected
    {
        get { return _selectedLanguage; }
        set
        {
            _selectedLanguage = value;
            RaisePropertyChanged();
            SupportedLanguage language;
            foreach (SupportedLanguage item in Enum.GetValues(typeof(SupportedLanguage)))
                if (ExtensionsEnum.GetEnumDescription(item) == value)
                {
                    language = item;
                    ChangeLangage?.Invoke(this, new ChangeLanguageEventArgs(language));
                    break;
                }
        }
    }

    public bool ChkOpenWindow
    {
        get { return _openWindow; }
        set
        {
            _openWindow = value;
            RaisePropertyChanged();
        }
    }

    public string TxtResult
    {
        get { return _txtResult; }
        set
        {
            _txtResult = value;
            RaisePropertyChanged();
        }
    }

    public bool ActiveIntellisense
    {
        get { return _intellisense; }
        set
        {
            _intellisense = value;
            RaisePropertyChanged();
        }
    }

    public int PositionList
    {
        get { return _selectionIntellisense; }
        set
        {
            try
            {
                if (value < 0)
                    value = 0;
                if (ListFilteredIntellisense != null && value > ListFilteredIntellisense.Count - 1)
                    value = ListFilteredIntellisense.Count - 1;
                _selectionIntellisense = value;
            }
            catch (Exception)
            {
                _selectionIntellisense = 0;
            }
            RaisePropertyChanged();
        }
    }

    public string TxtScript()
    {
        return _parentWindow.ScriptText.Text();
    }

    public string Usings
    {
        get { return _usings; }
        set
        {
            _usings = value;
            RaisePropertyChanged();
        }
    }

    public bool PopUpUsings
    {
        get { return _visibleUsings; }
        set
        {
            _visibleUsings = value;
            RaisePropertyChanged();
        }
    }

    public Brush FieldColorInternal
    {
        get { return _fieldColor; }
        set
        {
            _fieldColor = value;
            RaisePropertyChanged();
        }
    }

    public Brush PropertyColor
    {
        get { return _propertyColor; }
        set
        {
            _propertyColor = value;
            RaisePropertyChanged();
        }
    }

    public Brush MethodColor
    {
        get { return _methodColor; }
        set
        {
            _methodColor = value;
            RaisePropertyChanged();
        }
    }

    public Brush NamespaceColorInternal
    {
        get { return _nameSpaceColor; }
        set
        {
            _nameSpaceColor = value;
            RaisePropertyChanged();
        }
    }

    public Brush CommentColorInternal
    {
        get { return _commentaryColor; }
        set
        {
            _commentaryColor = value;
            RaisePropertyChanged();
        }
    }

    public bool WithFields
    {
        get { return _listFields; }
        set
        {
            _listFields = value;
            RaisePropertyChanged();
        }
    }

    public bool OtherThread
    {
        get { return _otherThread; }
        set
        {
            _otherThread = value;
            RaisePropertyChanged();
        }
    }

    public bool WithProperties
    {
        get { return _listProperties; }
        set
        {
            _listProperties = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region ICommand

    private void AddUsing()
    {
        PopUpUsings = !PopUpUsings;
    }

    private void Click_HyperLink(string propertyName)
    {
        PropertyInfo pi = GetType().GetProperty(propertyName);
        if (pi == null)
            return;
        System.Windows.Forms.ColorDialog dialog = new();
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            pi.SetValue(this, new SolidColorBrush(Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B)));
    }

    private void MenuCutText()
    {
        _parentWindow.ScriptText.Cut();
    }

    private void MenuCopyText()
    {
        _parentWindow.ScriptText.Copy();
    }

    private void MenuPasteText()
    {
        _parentWindow.ScriptText.Paste();
    }

    public void MenuSelectAll()
    {
        _parentWindow.ScriptText.SelectAll();
    }

    #endregion

    public bool InternalDebug { get; set; }

    public int PosCursor
    {
        get { return _parentWindow.ScriptText.SelectionStart(); }
        set
        {
            _parentWindow.ScriptText.InvokeIfRequired(() => _parentWindow.ScriptText.CaretPosition = _parentWindow.ScriptText.Document.ContentStart.GetPositionAtOffset(value));
        }
    }

    private int SelectionLength
    {
        get { return _parentWindow.ScriptText.SelectionLength(); }
        set
        {
            _parentWindow.ScriptText.InvokeIfRequired(() => _parentWindow.ScriptText.Selection.Select(_parentWindow.ScriptText.CaretPosition, _parentWindow.ScriptText.Document.ContentStart.GetPositionAtOffset(PosCursor + value)));
        }
    }

    private string SelectedText
    {
        get { return _parentWindow.ScriptText.Selection.Text; }
    }

    public PlatformType CurrentPlatform
    {
        get { return PlatformType.WPF; }
    }

    #region Intellisense

    public void KeyboardInput(char key)
    {
        InterpreteCode.KeyboardInput(this, key);
    }

    public void TextInput(TextCompositionEventArgs e)
    {
        try
        {
            KeyboardInput(e.Text[0]);
        }
        catch (Exception) { /* Ignore les erreurs de l'interpreteur d'aide à la saisie du script */ }
    }

    internal void Script_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            BtnExecuteScript.Execute(null);
            return;
        }

        if (!ListVisible)
            return;

        // Positionne la popup
        ResetPosIntellisense();

        if (e.Key == Key.Escape ||
            e.Key == Key.Space ||
            e.Key == Key.Enter ||
            e.Key == Key.Return ||
            e.Key == Key.Tab)
        {
            if (e.Key == Key.Enter ||
                e.Key == Key.Return ||
                e.Key == Key.Tab)
            {
                e.Handled = true;
                MouseDoubleClick();
            }
            ListVisible = false;
        }
    }

    public void KeyboardInputIntellisense(System.Windows.Forms.KeyEventArgs e = null, KeyEventArgs eWPF = null)
    {
        if (InterpreteCode.KeyboardInputIntellisense(this, eWPF.Key))
        {
            eWPF.Handled = true;
        }
    }

    internal void Script_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                KeyboardInput('\b');
                return;
            }
            else if (e.Key == Key.Tab)
            {
                int oldPosCurseur = PosCursor;
                _parentWindow.ScriptText.CaretPosition.InsertTextInRun(new string(' ', 4));
                PosCursor = oldPosCurseur + 4;
                e.Handled = true;
            }
            else
                KeyboardInputIntellisense(null, e);
        }
        catch (Exception ex)
        {
            if (InternalDebug)
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    #endregion

    public void MouseDoubleClick()
    {
        try
        {
            if (_selectionIntellisense >= 0 && _selectionIntellisense <= _listIntellisense.Count - 1)
            {
                int pos = PosCursor;
                int decal = 0;
                while (TxtScript()[pos - 1] == '\n' || TxtScript()[pos - 1] == '\r')
                {
                    pos--;
                    decal++;
                }
                int toDel = SelectionLength;
                if (toDel == 0)
                {
                    while (TxtScript().Trim()[pos - 1] != '.')
                    {
                        pos--;
                        toDel++;
                    }
                    if (toDel > 0)
                    {
                        PosCursor = pos + decal;
                        SelectionLength = toDel;
                    }
                }
                if (toDel > 0)
                    _parentWindow.ScriptText.Selection.Text = "";
                string nom = _listIntellisense[_selectionIntellisense].Name;
                _parentWindow.ScriptText.CaretPosition = _parentWindow.ScriptText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
                _parentWindow.ScriptText.CaretPosition.InsertTextInRun(nom);
                ListVisible = false;
            }
        }
        catch (Exception ex)
        {
            Debug(ex);
        }
    }

    public bool IgnoreCase
    {
        get { return string.IsNullOrWhiteSpace(LanguageSelected) || ExtensionsEnum.GetValueFromDescription<SupportedLanguage>(LanguageSelected) == SupportedLanguage.VBNET; }
    }

    private void Debug(Exception ex)
    {
        if (InternalDebug)
            MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
    }

    public event Delegates.DelegateExecuteScript ScriptExecuting;

    #region Manage script

    private void ExecuteScript()
    {
        try
        {
            TxtResult = "";
            ScriptingResult ret = null;

            SupportedLanguage currentLanguage;
            if (string.IsNullOrWhiteSpace(_selectedLanguage))
                currentLanguage = SupportedLanguage.VBNET;
            else
                currentLanguage = ExtensionsEnum.GetValueFromDescription<SupportedLanguage>(_selectedLanguage);

            if (currentLanguage == SupportedLanguage.CSHARP && TxtScript().Trim().IndexOf(Environment.NewLine) < 0 && TxtScript().Trim()[TxtScript().Trim().Length - 1] != ';')
            {
                _parentWindow.ScriptText.CaretPosition = _parentWindow.ScriptText.Document.ContentEnd;
                _parentWindow.ScriptText.CaretPosition.InsertTextInRun(";");
            }

            string[] listeAssembly = [];
            if (ListAssembly != null)
                listeAssembly = [.. ListAssembly.Select(ass => ass.FullName)];
            if (ListAssemblyInMemory != null)
                listeAssembly = [.. listeAssembly, .. ListAssemblyInMemory.Select(ass => ass.FullName).ToArray()];

            ScriptExecuting?.Invoke(this, new ExecuteScriptEventArgs() { Inherits = WithInherits, Language = currentLanguage, ListAssemblies = listeAssembly, ListUsings = Usings.Split(Environment.NewLine), MaxRecursion = MaxRecursiveInherits, Script = SelectionLength > 0 ? SelectedText : TxtScript(), SerializeDataContract = SerializationDataContract, DataContractName = DataContractName, MemberToSerialize = (WithFields ? YAXLib.YAXSerializationFields.AllFields : YAXLib.YAXSerializationFields.None) | (WithProperties ? YAXLib.YAXSerializationFields.PublicProperties : YAXLib.YAXSerializationFields.None) });

            if (!_executeOnProcess)
                return;

            string script = SelectionLength > 0 ? SelectedText : TxtScript();
            if (_otherThread)
            {
                if (currentLanguage == SupportedLanguage.CSHARP)
                    script = "System.Threading.Tasks.Task.Run(() => {" + Environment.NewLine + script + Environment.NewLine + "});";
                else if (currentLanguage == SupportedLanguage.VBNET)
                    script = "System.Threading.Tasks.Task.Run(Sub()" + Environment.NewLine + script + Environment.NewLine + "End Sub)";
            }

            ret = Scripting.Helpers.ExecuteScript.ExecuteScripts(script, _usings?.Split(Environment.NewLine.ToCharArray()).ToList(), currentLanguage, ListAssembly, ListReferences, AutoAddAllUsings, AssemblyDirectory, ListAssemblyInMemory?.ToArray());

            if (ret != null)
            {
                if (!ret.Success || ret.ExceptionThrowed != null)
                {
                    StringBuilder sb = new();
                    sb.AppendLine(Properties.Resources.ERRORS);
                    foreach (Diagnostic error in ret.Errors)
                        sb.AppendLine(error.ToString());
                    sb.AppendLine(ret.ExceptionThrowed?.ToString());
                    TxtResult = sb.ToString();
                }
                else
                {
                    TxtResult = Properties.Resources.SCRIPT_OK_RETURN;
                    TxtResult += ret.Result == null ? Properties.Resources.SCRIPT_RETURN_VOID : ret.Result.ToString();
                    if (ret.Result != null && !ret.Result.GetType().IsPrimitive && ret.Result is not string && ret.Result is not Enum && ret.Result is not DateTime && ret.Result is not TimeSpan && ChkOpenWindow)
                    {
                        Filename = null;
                        ObjectToSerialize = ret.Result;
                        Thread monThread = new(new ThreadStart(OpenWindow));
                        monThread.SetApartmentState(ApartmentState.STA);
                        monThread.Start();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Properties.Resources.ERROR_EXECUTE_SCRIPT);
        }
    }

    private void LoadScript()
    {
        OpenFileDialog dialog = new();
        if (dialog.ShowDialog() == true)
        {
            try
            {
                TextRange documentTextRange = new(_parentWindow.ScriptText.Document.ContentStart, _parentWindow.ScriptText.Document.ContentEnd);
                FileStream fs = File.OpenRead(dialog.FileName);
                documentTextRange.Load(fs, DataFormats.Text);
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Debug(ex);
            }
        }
    }

    private void SaveScript()
    {
        SaveFileDialog dialog = new();
        if (dialog.ShowDialog() == true)
        {
            try
            {
                if (File.Exists(dialog.FileName))
                    File.Delete(dialog.FileName);
                File.AppendAllText(dialog.FileName, TxtScript());
            }
            catch (Exception ex) { TxtResult = Environment.NewLine + Properties.Resources.ERROR_SAVE_SCRIPT + Environment.NewLine + ex.Message; }
        }
    }

    private void LoadObject()
    {
        OpenFileDialog dialog = new();
        if (dialog.ShowDialog() == true)
        {
            ObjectToSerialize = null;
            Filename = dialog.FileName;
            Thread thread = new(new ThreadStart(OpenWindow));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }

    public string Filename { get; set; }

    public object ObjectToSerialize { get; set; }

    public void OpenWindow()
    {
        Serialization.ShowResult(this);
    }

    public bool OpenInNewWindow
    {
        get { return ChkOpenWindow; }
        set { ChkOpenWindow = value; }
    }

    #endregion

    public string FilterScript { get; set; }

    public int NumOverrides { get; set; }

    public MethodInfo[] CurrentMethods { get; set; }

    public void SelectText(int start, int end)
    {
        _parentWindow.ScriptText.InvokeIfRequired(() => _parentWindow.ScriptText.Selection.Select(_parentWindow.ScriptText.Document.ContentStart.GetPositionAtOffset(start), _parentWindow.ScriptText.Document.ContentStart.GetPositionAtOffset(end)));
    }

    public int ReturnCurrentNumLine
    {
        get
        {
            return _parentWindow.ScriptText.GetLineIndexFromCharacterIndex(PosCursor);
        }
    }

    public string ReturnCurrentLine
    {
        get { return _parentWindow.ScriptText.GetLineText(ReturnCurrentNumLine); }
    }

    public void ResetPosIntellisense()
    {
        if (ListVisible && _parentWindow.PopUpIntellisense.Items.Count > 0)
        {
            Rect posCurseur = _parentWindow.ScriptText.GetRectFromCharacterIndex(PosCursor);
            _parentWindow.PopUpIntellisense.Margin = new Thickness(posCurseur.BottomRight.X, posCurseur.BottomRight.Y, 0, 0);
        }
    }

    public void SetSelectionColor(System.Drawing.Color newColor)
    {
        _parentWindow.ScriptText.InvokeIfRequired(() => _parentWindow.ScriptText.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, newColor.ToSolidColorBrush()));
    }

    public char ReadChar(int index)
    {
        try
        {
            return TxtScript()[index];
        }
        catch (Exception)
        {
            return (char)0;
        }
    }

    public string ReadString(int start, int length)
    {
        try
        {
            return TxtScript().Substring(start, length);
        }
        catch (Exception)
        {
            return "";
        }
    }

    public System.Drawing.Color CommentaryColor
    {
        get
        {
            return _parentWindow.InvokeIfRequired(() => { return ((SolidColorBrush)CommentColorInternal).ToSystemColor(); });
        }
    }

    public System.Drawing.Color PropertiesColor
    {
        get
        {
            return _parentWindow.InvokeIfRequired(() => { return ((SolidColorBrush)PropertyColor).ToSystemColor(); });
        }
    }

    public System.Drawing.Color NamespaceColor
    {
        get
        {
            return _parentWindow.InvokeIfRequired(() => { return ((SolidColorBrush)NamespaceColorInternal).ToSystemColor(); });
        }
    }

    public System.Drawing.Color MethodsColor
    {
        get
        {
            return _parentWindow.InvokeIfRequired(() => { return ((SolidColorBrush)MethodColor).ToSystemColor(); });
        }
    }

    public System.Drawing.Color FieldsColor
    {
        get
        {
            return _parentWindow.InvokeIfRequired(() => { return ((SolidColorBrush)FieldColorInternal).ToSystemColor(); });
        }
    }

    public System.Drawing.Color WriteColor
    {
        get
        {
            return _parentWindow.ScriptText.InvokeIfRequired(() => { return ((SolidColorBrush)_parentWindow.ScriptText.Foreground).ToSystemColor(); });
        }
    }

    public int PosFirstCharCurrentLine
    {
        get
        {
            return _parentWindow.ScriptText.InvokeIfRequired(() => { return _parentWindow.ScriptText.Document.ContentStart.GetOffsetToPosition(_parentWindow.ScriptText.CaretPosition.GetLineStartPosition(0)); });
        }
    }

    public int PosLastCharCurrentLine
    {
        get
        {
            return _parentWindow.ScriptText.InvokeIfRequired(() => { return _parentWindow.ScriptText.Document.ContentStart.GetOffsetToPosition(_parentWindow.ScriptText.CaretPosition.GetLineStartPosition(1) ?? _parentWindow.ScriptText.Document.ContentEnd); });
        }
    }

    public System.Drawing.Color SelectionColor
    {
        get
        {
            return _parentWindow.ScriptText.InvokeIfRequired(() => { return ((SolidColorBrush)_parentWindow.ScriptText.Selection.GetPropertyValue(TextElement.ForegroundProperty)).ToSystemColor(); });
        }
    }

    public void FillIntellisenseList(List<OneElementType> list)
    {
        _parentWindow.InvokeIfRequired(() =>
        {
            ListFilteredIntellisense = list;
            if (list != null && list.Count > 0)
                _parentWindow.PopUpIntellisense.ScrollIntoView(_parentWindow.PopUpIntellisense.Items[0]);
            PositionList = 0;
        });
    }
}
