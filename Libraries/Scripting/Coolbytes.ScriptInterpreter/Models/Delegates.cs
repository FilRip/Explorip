namespace CoolBytes.ScriptInterpreter.Models;

public static class Delegates
{
    public delegate void DelegateChangeLanguage(object sender, ChangeLanguageEventArgs e);

    public delegate void DelegateExecuteScript(object sender, ExecuteScriptEventArgs e);
}
