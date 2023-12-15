using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Explorip.TaskBar.Utilities
{
    public class DictionaryManager : IDisposable
    {
        private const string DICT_DEFAULT = "System";
        private const string DICT_EXT = "xaml";

        private const string LANG_DEFAULT = DICT_DEFAULT;
        private const string LANG_FALLBACK = "francais";
        private const string LANG_FOLDER = "Taskbar\\Languages";
        private const string LANG_EXT = DICT_EXT;

        public const string THEME_DEFAULT = DICT_DEFAULT;
        private const string THEME_FOLDER = "Taskbar\\Themes";
        private const string THEME_EXT = DICT_EXT;
        private bool disposedValue;

        public DictionaryManager()
        {
            Settings.Instance.PropertyChanged += Settings_PropertyChanged;
        }

        public void SetThemeFromSettings()
        {
            SetTheme(THEME_DEFAULT);
            if (Settings.Instance.Theme != THEME_DEFAULT)
            {
                SetTheme(Settings.Instance.Theme);
            }
        }

        private void SetTheme(string theme)
        {
            SetDictionary(theme, THEME_FOLDER, THEME_DEFAULT, THEME_EXT, 0);
        }

        private static Collection<ResourceDictionary> GetMergedDictionaries()
        {
            return Application.Current.Resources.MergedDictionaries;
        }

        private static ResourceDictionary GetActualThemeDictionary()
        {
            return GetMergedDictionaries().FirstOrDefault(rd => rd.Source.ToString().Contains($"{THEME_FOLDER}/"));
        }

        private void ClearPreviousThemes()
        {
            if (GetActualThemeDictionary() != null)
            {
                _ = GetMergedDictionaries().Remove(GetActualThemeDictionary());
            }
        }

        public void SetLanguageFromSettings()
        {
            SetLanguage(LANG_FALLBACK);
            if (Settings.Instance.Language == LANG_DEFAULT)
            {
                System.Globalization.CultureInfo currentUICulture = System.Globalization.CultureInfo.CurrentUICulture;
                string systemLanguageParent = currentUICulture.Parent.NativeName;
                string systemLanguage = currentUICulture.NativeName;
                ManagedShell.Common.Logging.ShellLogger.Info($"Loading system language (if available): {systemLanguageParent}, {systemLanguage}");
                SetLanguage(systemLanguageParent);
                SetLanguage(systemLanguage);
            }
            else
            {
                SetLanguage(Settings.Instance.Language);
            }
        }

        private void SetLanguage(string language)
        {
            SetDictionary(language, LANG_FOLDER, LANG_FALLBACK, LANG_EXT, 1);
        }

        private void SetDictionary(string dictionary, string dictFolder, string dictDefault, string dictExtension, int dictType)
        {
            string dictFilePath;

            if (dictionary == dictDefault)
            {
                if (dictType == 0)
                {
                    ClearPreviousThemes();
                }
                dictFilePath = Path.ChangeExtension(Path.Combine(dictFolder, dictDefault), dictExtension);
            }
            else
            {
                dictFilePath = Path.ChangeExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dictFolder, dictionary), dictExtension);

                if (!File.Exists(dictFilePath))
                {
                    dictFilePath = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(ExePath.GetExecutablePath()), dictFolder, dictionary), dictExtension);

                    if (!File.Exists(dictFilePath))
                    {
                        return;
                    }
                }
            }

            GetMergedDictionaries().Add(new ResourceDictionary()
            {
                Source = new Uri(dictFilePath, UriKind.RelativeOrAbsolute)
            });
        }

        public List<string> GetThemes()
        {
            return GetDictionaries(THEME_DEFAULT, THEME_FOLDER, THEME_EXT);
        }

        public List<string> GetLanguages()
        {
            List<string> languages =
            [
                LANG_DEFAULT,
                .. GetDictionaries(LANG_FALLBACK, LANG_FOLDER, LANG_EXT)
            ];
            return languages;
        }

        private List<string> GetDictionaries(string dictDefault, string dictFolder, string dictExtension)
        {
            List<string> dictionaries = [dictDefault];

            foreach (string subStr in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dictFolder))
                                               .Where(s => Path.GetExtension(s).Contains(dictExtension)))
            {
                dictionaries.Add(Path.GetFileNameWithoutExtension(subStr));
            }

            // Because Explorip is published as a single-file app, it gets extracted to a temp directory, so custom dictionaries won't be there.
            // Get the executable path to find the custom dictionaries directory when not a debug build.
            string customDictDir = Path.Combine(Path.GetDirectoryName(ExePath.GetExecutablePath()), dictFolder);

            if (Directory.Exists(customDictDir))
            {
                foreach (string subStr in Directory.GetFiles(customDictDir)
                    .Where(s => Path.GetExtension(s).Contains(dictExtension) && !dictionaries.Contains(Path.GetFileNameWithoutExtension(s))))
                {
                    dictionaries.Add(Path.GetFileNameWithoutExtension(subStr));
                }
            }

            return dictionaries;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                SetLanguageFromSettings();
            }
            if (e.PropertyName == "Theme")
            {
                SetThemeFromSettings();
            }
        }

        public bool IsDisposed
        {
            get { return disposedValue; }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Settings.Instance.PropertyChanged -= Settings_PropertyChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}