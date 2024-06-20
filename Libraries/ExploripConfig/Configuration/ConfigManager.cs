using System;
using System.IO;

using ExploripConfig.Helpers;

namespace ExploripConfig.Configuration
{
    public static class ConfigManager
    {
        public static string PATH_TO_INI { get; private set; }
        private static ManageIniFile _ini;
        private const string Explorip = "Explorip";
        private const string ExploripCopy = "ExploripCopy";

        public static void Init()
        {
            PATH_TO_INI = Path.Combine(Environment.SpecialFolder.LocalApplicationData.FullPath(), "CoolBytes", "Explorip", "ExploripConfig.ini");
            _ini = ManageIniFile.OpenIniFile(PATH_TO_INI);
        }

        public static bool ShowNotificationCopy
        {
            get { return _ini.ReadBoolean(ExploripCopy, "Notification"); }
            set
            {
                _ini.WriteString(ExploripCopy, "Notification", value.ToString());
            }
        }

        public static bool HookCopy
        {
            get { return _ini.ReadBoolean(Explorip, "HookCopy"); }
            set
            {
                _ini.WriteString(Explorip, "HookCopy", value.ToString());
            }
        }

        public static bool UseOwnCopier
        {
            get { return _ini.ReadBoolean(Explorip, "UseOwnCopier"); }
            set
            {
                _ini.WriteString(Explorip, "UseOwnCopier", value.ToString());
            }
        }

        public static ManageIniFile ManageIniFile
        {
            get { return _ini; }
        }
    }
}
