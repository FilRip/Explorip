using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Explorip.Helpers;

/// <summary>
/// Class to consult more easily command line arguments
/// </summary>
public static class ExtensionsCommandLineArguments
{
    /// <summary>
    /// Return if an argument is present<br/>
    /// Without case sensitive
    /// </summary>
    /// <param name="argument">Argument to search</param>
    public static bool ArgumentExists(string argument)
    {
        return ArgumentExists(argument, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Return if an argument is present<br/>
    /// With specified string comparer
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="comparer">String comparer to use</param>
    public static bool ArgumentExists(string argument, StringComparer comparer)
    {
        return Environment.GetCommandLineArgs().Contains(argument, comparer);
    }

    /// <summary>
    /// Return if an argument with value is present<br/>
    /// Without case sensitive<br/>
    /// And use equal sign to separate name and value
    /// </summary>
    /// <param name="argument">Argument to search</param>
    public static bool ArgumentVariableExists(string argument)
    {
        return ArgumentVariableExists(argument, "=", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Return if an argument with value is present<br/>
    /// With use equal sign to separate name and value<br/>
    /// And use a custom string comparer
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="comparer">String comparer to use</param>
    public static bool ArgumentVariableExists(string argument, StringComparison comparer)
    {
        return ArgumentVariableExists(argument, "=", comparer);
    }

    /// <summary>
    /// Return if an argument with value is present<br/>
    /// Without case sensitive<br/>
    /// And use a custom string separator
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="separator">Separator to use</param>
    public static bool ArgumentVariableExists(string argument, string separator)
    {
        return ArgumentVariableExists(argument, separator, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Return if an argument with value is present<br/>
    /// With use custom string separator<br/>
    /// And use a custom string comparer
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="separator">Separator to use</param>
    /// <param name="comparer">String comparer to use</param>
    public static bool ArgumentVariableExists(string argument, string separator, StringComparison comparer)
    {
        return Array.Exists(Environment.GetCommandLineArgs(), a => a.Trim().StartsWith((argument + separator).RemoveDuplicate(' '), comparer));
    }

    /// <summary>
    /// Return the value of an argument with a special key<br/>
    /// Without case sensitive<br/>
    /// And use equal sign to separate name and value
    /// </summary>
    /// <param name="argument">Argument to search</param>
    public static string ArgumentValue(string argument)
    {
        return ArgumentValue(argument, "=", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Return the value of an argument with a special key<br/>
    /// Without case sensitive<br/>
    /// With use custom string separator<br/>
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="separator">Separator to use</param>
    public static string ArgumentValue(string argument, string separator)
    {
        return ArgumentValue(argument, separator, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Return the value of an argument with a special key<br/>
    /// With use equal sign to separate name and value<br/>
    /// And use a custom string comparer
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="comparer">String comparer to use</param>
    public static string ArgumentValue(string argument, StringComparison comparer)
    {
        return ArgumentValue(argument, "=", comparer);
    }

    /// <summary>
    /// Return the value of an argument with a special key<br/>
    /// With use custom string separator<br/>
    /// And use a custom string comparer
    /// </summary>
    /// <param name="argument">Argument to search</param>
    /// <param name="separator">Separator to use</param>
    /// <param name="comparer">String comparer to use</param>
    public static string ArgumentValue(string argument, string separator, StringComparison comparer)
    {
        string arg = Array.Find(Environment.GetCommandLineArgs(), arg => arg.Trim().StartsWith((argument + separator).RemoveDuplicate(' '), comparer));
        if (arg != null)
            return arg.Trim().Substring(argument.Length + separator.Length).RemoveDuplicate(' ').Trim().TrimEnd('"');
        return null;
    }

    /// <summary>
    /// Return the full path and the exe filename with extension of current process
    /// </summary>
    public static string ArgumentFullPathExe()
    {
        return Environment.GetCommandLineArgs()[0];
    }

    /// <summary>
    /// Return the current exe filename (with extension) and without the path
    /// </summary>
    public static string ArgumentFileNameExe()
    {
        return Path.GetFileName(Environment.GetCommandLineArgs()[0]);
    }

    /// <summary>
    /// Return the path where the current exe is
    /// </summary>
    public static string ArgumentPathExe()
    {
        return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
    }

    /// <summary>
    /// Return all arguments, separate by space, like they are in current command line
    /// </summary>
    public static string Arguments()
    {
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            StringBuilder retour = new();
            foreach (string arg in Environment.GetCommandLineArgs().RemoveAt(0))
            {
                if (retour.Length > 0)
                    retour.Append(" ");
                retour.Append(arg);
            }
            return retour.ToString();
        }
        else
            return "";
    }
}
