using System;
using System.Reflection;

using Microsoft.CodeAnalysis;

namespace CoolBytes.Scripting.Models;

public class ScriptingResult
{
    public Diagnostic[] Errors { get; set; }

    public Exception ExceptionThrowed { get; set; }

    public Assembly AssemblyGenerate { get; set; }

    public object Result { get; set; }

    public bool Success { get; set; }
}
