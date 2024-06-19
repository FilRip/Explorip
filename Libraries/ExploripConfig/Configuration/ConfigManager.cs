using System;
using System.IO;

using ExploripConfig.Helpers;

namespace ExploripConfig.Configuration
{
    public static class ConfigManager
    {
        public static string PATH_TO_INI { get; private set; }
        private static ManageIniFile _ini;

        public static void Init()
        {
            PATH_TO_INI = Path.Combine(Environment.SpecialFolder.LocalApplicationData.FullPath(), "CoolBytes", "Explorip", "ExploripConfig.ini");
            if (!File.Exists(PATH_TO_INI))
                File.WriteAllText(PATH_TO_INI, "[Explorip]");
            _ini = ManageIniFile.OpenIniFile(PATH_TO_INI);
        }

        public static bool ShowNotificationCopy
        {
            get { return _ini.ReadBoolean("ExploripCopy", "Notification"); }
            set
            {
                _ini.WriteString("ExploripCopy", "Notification", value.ToString());
            }
        }
    }
}
