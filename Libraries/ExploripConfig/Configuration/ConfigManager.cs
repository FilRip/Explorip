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

            // If empty config ini file, set default settings
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "Theme")))
                _ini.WriteString(Explorip, "Theme", "System");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "ShowClock")))
                _ini.WriteString(Explorip, "ShowClock", "True");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "ShowQuickLaunch")))
                _ini.WriteString(Explorip, "ShowQuickLaunch", "True");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "QuickLaunchPath")))
                _ini.WriteString(Explorip, "QuickLaunchPath", "%USERPROFILE%\\QuickLaunch");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "CollapseNotifyIcons")))
                _ini.WriteString(Explorip, "CollapseNotifyIcons", "True");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "AllowFontSmoothing")))
                _ini.WriteString(Explorip, "AllowFontSmoothing", "True");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "Language")))
                _ini.WriteString(Explorip, "Language", "System");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "Edge")))
                _ini.WriteString(Explorip, "Edge", "3");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "HookCopy")))
                _ini.WriteString(Explorip, "HookCopy", "True");
            if (string.IsNullOrWhiteSpace(_ini.ReadString(Explorip, "UseOwnCopier")))
                _ini.WriteString(Explorip, "UseOwnCopier", "True");
        }

        public static string Theme
        {
            get { return _ini.ReadString(Explorip, "Theme"); }
            set { _ini.WriteString(Explorip, "Theme", value); }
        }

        public static bool ShowClock
        {
            get { return _ini.ReadBoolean(Explorip, "ShowClock"); }
            set { _ini.WriteString(Explorip, "ShowClock", value.ToString()); }
        }

        public static bool ShowQuickLaunch
        {
            get { return _ini.ReadBoolean(Explorip, "ShowQuickLaunch"); }
            set { _ini.WriteString(Explorip, "ShowQuickLaunch", value.ToString()); }
        }

        public static string QuickLaunchPath
        {
            get { return _ini.ReadString(Explorip, "QuickLaunchPath"); }
            set { _ini.WriteString(Explorip, "QuickLaunchPath", value); }
        }

        public static bool CollapseNotifyIcons
        {
            get { return _ini.ReadBoolean(Explorip, "CollapseNotifyIcons"); }
            set { _ini.WriteString(Explorip, "CollapseNotifyIcons", value.ToString()); }
        }

        public static bool AllowFontSmoothing
        {
            get { return _ini.ReadBoolean(Explorip, "AllowFontSmoothing"); }
            set { _ini.WriteString(Explorip, "AllowFontSmoothing", value.ToString()); }
        }

        public static string Language
        {
            get { return _ini.ReadString(Explorip, "Language"); }
            set { _ini.WriteString(Explorip, "Language", value); }
        }

        public static int Edge
        {
            get { return _ini.ReadInteger(Explorip, "Edge"); }
            set { _ini.WriteString(Explorip, "Edge", value.ToString()); }
        }

        public static bool ShowNotificationCopy
        {
            get { return _ini.ReadBoolean(ExploripCopy, "Notification"); }
            set { _ini.WriteString(ExploripCopy, "Notification", value.ToString()); }
        }

        public static bool HookCopy
        {
            get { return _ini.ReadBoolean(Explorip, "HookCopy"); }
            set { _ini.WriteString(Explorip, "HookCopy", value.ToString()); }
        }

        public static bool UseOwnCopier
        {
            get { return _ini.ReadBoolean(Explorip, "UseOwnCopier"); }
            set { _ini.WriteString(Explorip, "UseOwnCopier", value.ToString()); }
        }

        public static ManageIniFile ManageIniFile
        {
            get { return _ini; }
        }
    }
}
