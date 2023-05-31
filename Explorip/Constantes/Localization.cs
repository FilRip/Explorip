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
        public static string TASK_MANAGER { get; private set; }
        public static string SHOW_TITLE { get; private set; }
        public static string SMALL_ICON { get; private set; }
        public static string LARGE_ICON { get; private set; }
        public static string NEW_TOOLBAR { get; private set; }
        public static string SHOW_VISUAL_KEYBOARD { get; private set; }

        public static void LoadTranslation()
        {
            LIBELLE_COLLER = GestionString.Load("shell32.dll", 33562, "Paste");
            LIBELLE_COLLER_RACCOURCI = GestionString.Load("shell32.dll", 37376, "Paste shortcut");
            POWER_SHELL = GestionString.Load("twinui.dll", 10928, "W_indows PowerShell").Replace("&", "_");
            POWER_SHELL_ADMIN = GestionString.Load("twinui.dll", 10929, "Windows PowerShell (_admin)").Replace("&", "_");
            COMMANDLINE = GestionString.Load("twinui.dll", 10919, "Commands prompt").Replace("&", "_");
            COMMANDLINE_ADMIN = GestionString.Load("twinui.dll", 10920, "Commands prompt (_admin)").Replace("&", "_");
            TASK_MANAGER = GestionString.Load("shell32.dll", 24743, "Tasks manager");
            SHOW_TITLE = GestionString.Load("msutb.dll", 304, "Show title");
            LARGE_ICON = GestionString.Load("shell32.dll", 31062, "Show large icon");
            SMALL_ICON = GestionString.Load("shell32.dll", 31063, "Show small icon");
            NEW_TOOLBAR = GestionString.Load("ieframe.dll", 12388, "New toolbar...");
            SHOW_VISUAL_KEYBOARD = GestionString.Load("ieframe.dll", 12388, "New toolbar...");
        }
    }
}
