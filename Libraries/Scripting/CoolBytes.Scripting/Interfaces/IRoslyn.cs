using System.Reflection;

using Microsoft.CodeAnalysis;

namespace CoolBytes.Scripting.Interfaces;

public interface IRoslyn
{
    string Header { get; }

    string Footer { get; }

    string NamespaceUsing { get; }

    ParseOptions ReturnParserOptions();

    CompilationOptions ReturnCompilerOptions();

    MethodInfo ReturnParserMethod(ParseOptions parserOptions);

    SyntaxTree ReturnPrecompiledCode(string code, ParseOptions parserOptions);
}
