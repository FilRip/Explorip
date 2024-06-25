using ExploripConfig.Helpers;

namespace Explorip.Helpers;

public static class MyDebug
{
    public static string[] RemoveDebugArguments(string[] args)
    {
        args = args.Remove("explorer");
        args = args.Remove("withouthook");
        args = args.Remove("useowncopier");
        args = args.Remove("newinstance");
        args = args.Remove("disablewriteconfig");
        return args;
    }
}
