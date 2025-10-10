using System;

using CoolBytes.Scripting.Enums;
using CoolBytes.ScriptInterpreter.YAXLib;

namespace CoolBytes.ScriptInterpreter.Models;

public class ExecuteScriptEventArgs : EventArgs
{
    public string Script { get; set; }

    public string[] ListUsings { get; set; }

    public int MaxRecursion { get; set; }

    public bool Inherits { get; set; }

    public bool SerializeDataContract { get; set; }

    public SupportedLanguage Language { get; set; }

    public string[] ListAssemblies { get; set; }

    public string DataContractName { get; set; }

    public YAXSerializationFields MemberToSerialize { get; set; }
}
