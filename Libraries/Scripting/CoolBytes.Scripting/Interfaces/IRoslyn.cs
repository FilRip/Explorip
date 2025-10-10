using Microsoft.CodeAnalysis;

namespace CoolBytes.Scripting.Interfaces;

public interface IRoslyn
{
    string Header { get; }

    string Footer { get; }

    string NamespaceUsing { get; }

    ParseOptions ReturnParserOptions();

    CompilationOptions ReturnCompilerOptions();
}
