namespace CoolBytes.ScriptInterpreter.Models;

internal static class StartComments
{
    internal static readonly string[] VBNET = ["'"];
    internal static readonly string[] CSHARP = ["//", @"/*"];

    internal static string[] ListStartComments(bool ignoreCase)
    {
        return ignoreCase ? VBNET : CSHARP;
    }
}

internal static class EndComments
{
    internal static readonly string[] VBNET = [];
    internal static readonly string[] CSHARP = [@"*/"];

    internal static string[] ListEndComments(bool ignoreCase)
    {
        return ignoreCase ? VBNET : CSHARP;
    }
}
