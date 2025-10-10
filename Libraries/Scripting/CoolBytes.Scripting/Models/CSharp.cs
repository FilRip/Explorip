using System;
using System.Diagnostics;

using CoolBytes.Scripting.Interfaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CoolBytes.Scripting.Models;

public class CSharp : Roslyn<CSharp, CSharpCompilation, CSharpSyntaxTree>, IRoslyn
{
    public string Header
    {
        get
        {
            string ret = "";
            ret += "namespace MyDynamicNamespace" + Environment.NewLine;
            ret += "{" + Environment.NewLine;
            ret += "public class MyDynamicClass" + Environment.NewLine;
            ret += "{" + Environment.NewLine;
            ret += "public static object MyDynamicMethod()" + Environment.NewLine;
            ret += "{" + Environment.NewLine;
            return ret;
        }
    }

    public string Footer
    {
        get
        {
            string ret = "";
            ret += "return null;" + Environment.NewLine;
            ret += "}" + Environment.NewLine;
            ret += "}" + Environment.NewLine;
            ret += "}" + Environment.NewLine;
            return ret;
        }
    }

    public string NamespaceUsing
    {
        get { return "using <%namespace%>;"; }
    }

    public CompilationOptions ReturnCompilerOptions()
    {
        return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 0, platform: Platform.AnyCpu, optimizationLevel: Debugger.IsAttached ? OptimizationLevel.Debug : OptimizationLevel.Release, checkOverflow: Debugger.IsAttached);
    }

    public ParseOptions ReturnParserOptions()
    {
        return new CSharpParseOptions(LanguageVersion.CSharp6, DocumentationMode.None);
    }
}
