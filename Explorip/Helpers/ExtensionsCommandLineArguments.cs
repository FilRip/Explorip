using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Explorip.Helpers
{
    /// <summary>
    /// Classe permettant de consulter plus facilement les aguments de la ligne de commande du processus en cours
    /// </summary>
    public static class ExtensionsCommandLineArguments
    {
        /// <summary>
        /// Retourne si oui ou non un argument est présent dans la ligne de commande du processus en cours
        /// Sans tenir compte de la casse
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        public static bool ArgumentPresent(string argument)
        {
            return ArgumentPresent(argument, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retourne si oui ou non un argument est présent dans la ligne de commande du processus en cours
        /// En prenant en compte le comparateur de chaine spécifié
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="comparateur">Comparateur de chaine</param>
        public static bool ArgumentPresent(string argument, StringComparer comparateur)
        {
            return Environment.GetCommandLineArgs().Contains(argument, comparateur);
        }

        /// <summary>
        /// Retourne si oui ou non un argument avec valeur est présent dans la ligne de commande du processus en cours
        /// Sans tenir compte de la casse et en supposant que le séparateur entre le nom de l'argument et sa valeur est le signe "=" (égal)
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        public static bool ArgumentVariablePresent(string argument)
        {
            return ArgumentVariablePresent(argument, "=", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retourne si oui ou non un argument avec valeur est présent dans la ligne de commande du processus en cours
        /// En prenant en compte le comparateur de chaine spécifié et en supposant que le séparateur entre le nom de l'argument et sa valeur est le signe "=" (égal)
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="comparateur">Comparateur de chaine</param>
        public static bool ArgumentVariablePresent(string argument, StringComparison comparateur)
        {
            return ArgumentVariablePresent(argument, "=", comparateur);
        }

        /// <summary>
        /// Retourne si oui ou non un argument avec valeur est présent dans la ligne de commande du processus en cours
        /// Sans tenir compte de la casse et en utilisant le séparateur spécifié
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="separateur">Caractère séparateur à utiliser</param>
        public static bool ArgumentVariablePresent(string argument, string separateur)
        {
            return ArgumentVariablePresent(argument, separateur, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retourne si oui ou non un argument avec valeur est présent dans la ligne de commande du processus en cours
        /// En prenant en compte le comparateur de chaine spécifié et en utilisant le séparateur spécifié
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="separateur">Caractère séparateur à utiliser</param>
        /// <param name="comparateur">Comparateur de chaine</param>
        public static bool ArgumentVariablePresent(string argument, string separateur, StringComparison comparateur)
        {
            return Environment.GetCommandLineArgs().Any(a => a.Trim().StartsWith((argument + separateur).SupprimeDoublons(' '), comparateur));
        }

        /// <summary>
        /// Retourne la valeur d'un argument dans la ligne de commande du processus en cours
        /// Sans tenir compte de la casse et en supposant que le séparateur entre le nom de l'argument et sa valeur est le signe "=" (égal)
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        public static string ArgumentValeur(string argument)
        {
            return ArgumentValeur(argument, "=", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retourne la valeur d'un argument dans la ligne de commande du processus en cours
        /// Sans tenir compte de la casse et en utilisant le séparateur spécifié
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="separateur">Caractère séparateur à utiliser</param>
        public static string ArgumentValeur(string argument, string separateur)
        {
            return ArgumentValeur(argument, separateur, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retourne la valeur d'un argument dans la ligne de commande du processus en cours
        /// En prenant en compte le comparateur de chaine spécifié et en supposant que le séparateur entre le nom de l'argument et sa valeur est le signe "=" (égal)
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="comparateur">Comparateur de chaine</param>
        public static string ArgumentValeur(string argument, StringComparison comparateur)
        {
            return ArgumentValeur(argument, "=", comparateur);
        }

        /// <summary>
        /// Retourne la valeur d'un argument dans la ligne de commande du processus en cours
        /// En prenant en compte le comparateur de chaine spécifié et en utilisant le séparateur spécifié
        /// </summary>
        /// <param name="argument">Argument à rechercher</param>
        /// <param name="separateur">Caractère séparateur à utiliser</param>
        /// <param name="comparateur">Comparateur de chaine</param>
        public static string ArgumentValeur(string argument, string separateur, StringComparison comparateur)
        {
            string arg = Environment.GetCommandLineArgs().FirstOrDefault(arg => arg.Trim().StartsWith((argument + separateur).SupprimeDoublons(' '), comparateur));
            if (arg != null)
                return arg.Trim().Substring(argument.Length + separateur.Length).SupprimeDoublons(' ').Trim().TrimEnd('"');
            return null;
        }

        /// <summary>
        /// Retourne le nom, avec son emplacement, de l'exécutable en cours
        /// </summary>
        public static string ArgumentFichierExe()
        {
            return Environment.GetCommandLineArgs()[0];
        }

        /// <summary>
        /// Retourne le nom de l'exécutable en cours (sans le chemin, voir ArgumentFichierExe pour avoir avec le chemin, ou ArgumentRepertoireExe pour n'avoir que le chemin)
        /// </summary>
        public static string ArgumentNomExe()
        {
            return Path.GetFileName(Environment.GetCommandLineArgs()[0]);
        }

        /// <summary>
        /// Retourne le répertoire de l'exécutable en cours
        /// </summary>
        public static string ArgumentRepertoireExe()
        {
            return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        }

        /// <summary>
        /// Retourne tous les arguments de la ligne de commande, séparé par un espace, dans une chaine de caractères, comme à l'appel de ce processus
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
}
