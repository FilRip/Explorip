using ExploripConfig.Helpers;

namespace Explorip.Helpers;

public static class MyDebug
{
    public static string[] RemoveDebugArguments(string[] args, bool removeFirst = false)
    {
        if (removeFirst && args.Length > 0)
            args = args.RemoveAt(0);
        args = args.Remove("explorer");
        args = args.Remove("withouthook");
        args = args.Remove("useowncopier");
        args = args.Remove("newinstance");
        args = args.Remove("disablewriteconfig");
        return args;
    }
}
