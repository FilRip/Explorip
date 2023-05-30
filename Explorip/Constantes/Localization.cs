using Explorip.Localization;

namespace Explorip.Constantes
{
    public static class Localization
    {
        public static string LIBELLE_COLLER { get; private set; }
        public static string LIBELLE_COLLER_RACCOURCI { get; private set; }
        public static string POWER_SHELL { get; private set; }
        public static string POWER_SHELL_ADMIN { get; private set; }
        public static string COMMANDLINE { get; private set; }
        public static string COMMANDLINE_ADMIN { get; private set; }

        public static void LoadTranslation()
        {
            LIBELLE_COLLER = GestionString.Load("shell32.dll", 33562, "Paste");
            LIBELLE_COLLER_RACCOURCI = GestionString.Load("shell32.dll", 37376, "Paste shortcut");
            POWER_SHELL = GestionString.Load("twinui.dll", 10928, "W_indows PowerShell").Replace("&", "_");
            POWER_SHELL_ADMIN = GestionString.Load("twinui.dll", 10929, "Windows PowerShell (_admin)").Replace("&", "_");
            COMMANDLINE = GestionString.Load("twinui.dll", 10919, "Invite de _commandes").Replace("&", "_");
            COMMANDLINE_ADMIN = GestionString.Load("twinui.dll", 10920, "Invite de commandes (_admin)").Replace("&", "_");
        }
    }
}
