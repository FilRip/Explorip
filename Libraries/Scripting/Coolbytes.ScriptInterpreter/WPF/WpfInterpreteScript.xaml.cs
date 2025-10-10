using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Windows.Navigation;

using CoolBytes.Helpers;
using CoolBytes.Scripting.Enums;
using CoolBytes.ScriptInterpreter.Models;
using CoolBytes.ScriptInterpreter.WPF.ViewModels;

namespace CoolBytes.ScriptInterpreter.WPF;

public partial class WpfInterpreteScript
{
    public WpfInterpreteScript()
    {
        DataContext = new WpfInterpreteScriptViewModel(this);
        ((WpfInterpreteScriptViewModel)DataContext).ScriptExecuting += WpfInterpreteScript_ExecuterScript;
        ((WpfInterpreteScriptViewModel)DataContext).ChangeLangage += WpfInterpreteScript_ChangeLangage;
        InitializeComponent();
        ScriptText.Document.Blocks.Clear();
    }

    private void WpfInterpreteScript_ChangeLangage(object sender, ChangeLanguageEventArgs e)
    {
        ChangeLanguage?.Invoke(this, e);
    }

    private void WpfInterpreteScript_ExecuterScript(object sender, ExecuteScriptEventArgs e)
    {
        ExecuteScript?.Invoke(this, e);
    }

    public bool SearchInReflection
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).SearchInReflection; }
        set { ((WpfInterpreteScriptViewModel)DataContext).SearchInReflection = value; }
    }

    public void SetStartClass(object classeDemarrage)
    {
        ((WpfInterpreteScriptViewModel)DataContext).StartClass(classeDemarrage);
    }

    public void ReloadAssembly()
    {
        ((WpfInterpreteScriptViewModel)DataContext).ReloadAssembly();
    }

    public void AddUsings(string[] usings)
    {
        if ((usings != null) && (usings.Length > 0))
        {
            StringBuilder sb = new();
            sb.AppendLine();
            foreach (string monUsing in usings.Where(monUsing => !string.IsNullOrWhiteSpace(monUsing) && !((WpfInterpreteScriptViewModel)DataContext).Usings.Contains(monUsing, StringComparison.OrdinalIgnoreCase)))
                sb.AppendLine(monUsing);
            ((WpfInterpreteScriptViewModel)DataContext).Usings += sb.ToString();
        }
    }

    public void RemoveUsing(string usingASupprimer)
    {
        try
        {
            string[] listeUsings = ((WpfInterpreteScriptViewModel)DataContext).Usings.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            ((WpfInterpreteScriptViewModel)DataContext).Usings = listeUsings.RemoveAll(ligne => ligne.Trim().ToLower() == usingASupprimer.Trim().ToLower()).ToListOfString(Environment.NewLine);
        }
        catch (Exception) { /* Ignore les erreurs eventuelles */ }
    }

    public bool AutoAddAllUsings
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).AutoAddAllUsings; }
        set { ((WpfInterpreteScriptViewModel)DataContext).AutoAddAllUsings = value; }
    }

    public List<Assembly> ListAssembly
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).ListAssembly; }
        set { ((WpfInterpreteScriptViewModel)DataContext).ListAssembly = value; }
    }

    public List<Assembly> ListAssemblyInMemory
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).ListAssemblyInMemory; }
        set { ((WpfInterpreteScriptViewModel)DataContext).ListAssemblyInMemory = value; }
    }

    public List<string> ListReferences
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).ListReferences; }
        set { ((WpfInterpreteScriptViewModel)DataContext).ListReferences = value; }
    }

    public string AssemblyDirectory
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).AssemblyDirectory; }
        set { ((WpfInterpreteScriptViewModel)DataContext).AssemblyDirectory = value; }
    }

    public void SetDefaultLanguage(SupportedLanguage langage)
    {
        ((WpfInterpreteScriptViewModel)DataContext).LanguageSelected = langage.ToString("G");
    }

    private void SaisieScript_KeyUp(object sender, KeyEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).Script_KeyUp(sender, e);
    }

    private void SaisieScript_KeyDown(object sender, KeyEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).Script_KeyDown(sender, e);
        PopUpIntellisense.ScrollIntoView(PopUpIntellisense.SelectedItem);
    }

    private void Hyperlink_RequestNavigateChamps(object sender, RequestNavigateEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).ClickField.Execute(null);
    }

    private void Hyperlink_RequestNavigatePropriete(object sender, RequestNavigateEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).ClickProperty.Execute(null);

    }

    private void Hyperlink_RequestNavigateMethode(object sender, RequestNavigateEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).ClickMethod.Execute(null);
    }

    private void Hyperlink_RequestNavigateNameSpace(object sender, RequestNavigateEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).ClickNameSpace.Execute(null);
    }

    private void Hyperlink_RequestNavigateCommentaire(object sender, RequestNavigateEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).ClickCommentary.Execute(null);
    }

    private void ItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).MouseDoubleClick();
    }

    public bool InternalDebug
    {
        get { return ((WpfInterpreteScriptViewModel)DataContext).InternalDebug; }
        set { ((WpfInterpreteScriptViewModel)DataContext).InternalDebug = value; }
    }

    public void ExecuteOnProcess(bool surProcess)
    {
        ((WpfInterpreteScriptViewModel)DataContext).ExecuteOnProcess(surProcess);
    }

    private void SaisieScript_TextInput(object sender, TextCompositionEventArgs e)
    {
        ((WpfInterpreteScriptViewModel)DataContext).TextInput(e);
    }

    public event Delegates.DelegateChangeLanguage ChangeLanguage;

    public event Delegates.DelegateExecuteScript ExecuteScript;
}
