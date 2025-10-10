using System;

using CoolBytes.Scripting.Enums;

namespace CoolBytes.ScriptInterpreter.Models;

public class ChangeLanguageEventArgs(SupportedLanguage newLanguage) : EventArgs()
{
    private readonly SupportedLanguage _currentLanguage = newLanguage;

    public SupportedLanguage CurrentLanguage
    {
        get { return _currentLanguage; }
    }
}
