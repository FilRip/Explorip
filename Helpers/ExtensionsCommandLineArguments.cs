using System;
using System.IO;
using System.Linq;

namespace Explorip.Helpers
{
    public static class ExtensionsCommandLineArguments
    {
        public static bool ArgumentPresent(string argument)
        {
            return ArgumentPresent(argument, StringComparer.OrdinalIgnoreCase);
        }

        public static bool ArgumentPresent(string argument, StringComparer comparateur)
        {
            return Environment.GetCommandLineArgs().Contains(argument, comparateur);
        }

        public static bool ArgumentVariablePresent(string argument)
        {
            return ArgumentVariablePresent(argument, "=", StringComparison.OrdinalIgnoreCase);
        }

        public static bool ArgumentVariablePresent(string argument, StringComparison comparateur)
        {
            return ArgumentVariablePresent(argument, "=", comparateur);
        }

        public static bool ArgumentVariablePresent(string argument, string separateur)
        {
            return ArgumentVariablePresent(argument, separateur, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ArgumentVariablePresent(string argument, string separateur, StringComparison comparateur)
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.Trim().StartsWith((argument + separateur).SupprimeDoublons(' '), comparateur))
                    return true;
            return false;
        }

        public static string ArgumentValeur(string argument)
        {
            return ArgumentValeur(argument, "=", StringComparison.OrdinalIgnoreCase);
        }

        public static string ArgumentValeur(string argument, string separateur)
        {
            return ArgumentValeur(argument, separateur, StringComparison.OrdinalIgnoreCase);
        }

        public static string ArgumentValeur(string argument, StringComparison comparateur)
        {
            return ArgumentValeur(argument, "=", comparateur);
        }

        public static string ArgumentValeur(string argument, string separateur, StringComparison comparateur)
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.Trim().StartsWith((argument + separateur).SupprimeDoublons(' '), comparateur))
                    return arg.Trim().Substring(argument.Length + separateur.Length).SupprimeDoublons(' ').Trim();
            return null;
        }

        public static string ArgumentFichierExe()
        {
            return Environment.GetCommandLineArgs()[0];
        }

        public static string ArgumentNomExe()
        {
            return Path.GetFileName(Environment.GetCommandLineArgs()[0]);
        }

        public static string ArgumentRepertoireExe()
        {
            return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        }

        public static string Arguments()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string retour = "";
                foreach (string arg in Environment.GetCommandLineArgs().RemoveAt(0))
                {
                    if (retour != "")
                        retour += " ";
                    retour += arg;
                }
                return retour;
            }
            else
                return "";
        }
    }
}
