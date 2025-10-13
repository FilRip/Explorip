using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

using CoolBytes.Scripting.Interfaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace CoolBytes.Scripting.Models;

public class VisualBasic : Roslyn<VisualBasic, VisualBasicCompilation, VisualBasicSyntaxTree>, IRoslyn
{
    public string Header
    {
        get
        {
            string ret = "";
            ret += "Namespace MyDynamicNamespace" + Environment.NewLine;
            ret += "Public Class MyDynamicClass" + Environment.NewLine;
            ret += "Public Shared Function MyDynamicMethod() As Object" + Environment.NewLine;
            return ret;
        }
    }

    public string Footer
    {
        get
        {
            string ret = "";
            ret += "return nothing" + Environment.NewLine;
            ret += "End Function" + Environment.NewLine;
            ret += "End Class" + Environment.NewLine;
            ret += "End Namespace" + Environment.NewLine;
            return ret;
        }
    }

    public string NamespaceUsing
    {
        get { return "Imports <%namespace%>"; }
    }

    public CompilationOptions ReturnCompilerOptions()
    {
        return new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, platform: Platform.AnyCpu, optimizationLevel: Debugger.IsAttached ? OptimizationLevel.Debug : OptimizationLevel.Release, checkOverflow: Debugger.IsAttached);
    }

    public ParseOptions ReturnParserOptions()
    {
        return new VisualBasicParseOptions(LanguageVersion.Latest, DocumentationMode.None);
    }

    public override MethodInfo ReturnParserMethod(ParseOptions parserOptions)
    {
        return typeof(VisualBasicSyntaxTree).GetMethod("ParseText", BindingFlags.Static | BindingFlags.Public, null, [typeof(string), parserOptions.GetType(), typeof(string), typeof(Encoding), typeof(ImmutableDictionary<string, ReportDiagnostic>), typeof(CancellationToken)], null);
    }

    public override SyntaxTree ReturnPrecompiledCode(string code, ParseOptions parserOptions)
    {
        return (SyntaxTree)ReturnParserMethod(parserOptions).Invoke(null, [code, parserOptions, Type.Missing, Type.Missing, Type.Missing, Type.Missing]);
    }
}
