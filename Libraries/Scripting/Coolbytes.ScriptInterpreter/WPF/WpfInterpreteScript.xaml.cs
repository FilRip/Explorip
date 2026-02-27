using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

using CoolBytes.Helpers;
using CoolBytes.Scripting.Enums;
using CoolBytes.ScriptInterpreter.Models;
using CoolBytes.ScriptInterpreter.WPF.ViewModels;

namespace CoolBytes.ScriptInterpreter.WPF;

public partial class WpfInterpreteScript
{
    private bool _isLoaded;

    public WpfInterpreteScript()
    {
        InitializeComponent();
    }

    public new WpfInterpreteScriptViewModel DataContext
    {
        get { return (WpfInterpreteScriptViewModel)base.DataContext; }
    }

    private void MyControlInterpreteScript_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            DataContext.Init(this);
            DataContext.ScriptExecuting += WpfInterpreteScript_ExecuterScript;
            DataContext.ChangeLangage += WpfInterpreteScript_ChangeLangage;
            ScriptText.Document.Blocks.Clear();
        }
    }

    #region Events

    public event Delegates.DelegateChangeLanguage ChangeLanguage;

    public event Delegates.DelegateExecuteScript ExecuteScript;

    private void WpfInterpreteScript_ChangeLangage(object sender, ChangeLanguageEventArgs e)
    {
        ChangeLanguage?.Invoke(this, e);
    }

    private void WpfInterpreteScript_ExecuterScript(object sender, ExecuteScriptEventArgs e)
    {
        ExecuteScript?.Invoke(this, e);
    }

    #endregion

    #region Linked properties

    public bool SearchInReflection
    {
        get { return DataContext.SearchInReflection; }
        set { DataContext.SearchInReflection = value; }
    }

    public void SetStartClass(object initialTypeInstance)
    {
        DataContext.StartClass(initialTypeInstance);
    }

    public void ReloadAssembly()
    {
        DataContext.ReloadAssembly();
    }

    public void AddUsings(string[] usings)
    {
        if ((usings != null) && (usings.Length > 0))
        {
            StringBuilder sb = new();
            sb.AppendLine();
            foreach (string monUsing in usings.Where(monUsing => !string.IsNullOrWhiteSpace(monUsing) && !DataContext.Usings.Contains(monUsing, StringComparison.OrdinalIgnoreCase)))
                sb.AppendLine(monUsing);
            DataContext.Usings += sb.ToString();
        }
    }

    public void RemoveUsing(string usingASupprimer)
    {
        try
        {
            string[] listeUsings = DataContext.Usings.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            DataContext.Usings = listeUsings.RemoveAll(ligne => ligne.Trim().ToLower() == usingASupprimer.Trim().ToLower()).ToListOfString(Environment.NewLine);
        }
        catch (Exception) { /* Ignore les erreurs eventuelles */ }
    }

    public bool AutoAddAllUsings
    {
        get { return DataContext.AutoAddAllUsings; }
        set { DataContext.AutoAddAllUsings = value; }
    }

    public ObservableCollection<Assembly> ListAssembly
    {
        get { return DataContext.ListAssembly; }
        set { DataContext.ListAssembly = value; }
    }

    public ObservableCollection<Assembly> ListAssemblyInMemory
    {
        get { return DataContext.ListAssemblyInMemory; }
        set { DataContext.ListAssemblyInMemory = value; }
    }

    public ObservableCollection<string> ListReferences
    {
        get { return DataContext.ListReferences; }
        set { DataContext.ListReferences = value; }
    }

    public string AssemblyDirectory
    {
        get { return DataContext.AssemblyDirectory; }
        set { DataContext.AssemblyDirectory = value; }
    }

    public void SetDefaultLanguage(SupportedLanguage langage)
    {
        DataContext.LanguageSelected = langage.ToString("G");
    }

    public bool InternalDebug
    {
        get { return DataContext.InternalDebug; }
        set { DataContext.InternalDebug = value; }
    }

    public void ExecuteOnProcess(bool surProcess)
    {
        DataContext.ExecuteOnProcess(surProcess);
    }

    #endregion
}
