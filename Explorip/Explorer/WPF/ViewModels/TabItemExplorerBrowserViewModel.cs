using Explorip.Helpers;

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Explorip.Explorer.WPF.ViewModels
{
    public class TabItemExplorerBrowserViewModel : ViewModelBase
    {
        private readonly SolidColorBrush _accentColor, _disabledColor;

        public TabItemExplorerBrowserViewModel()
        {
            System.Drawing.Color myColor = WindowsSettings.GetWindowsAccentColor();
            Color mColor = Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);
            _accentColor = new SolidColorBrush(mColor);
            _disabledColor = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            _lastFolder = "";
        }

        private string _path;
        public string TabTitle
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        private bool _modeEdit;
        public bool ModeEdit
        {
            get { return _modeEdit; }
            set
            {
                _lastFolder = "";
                _modeEdit = value;
                OnPropertyChanged();
            }
        }

        private bool _previous, _next;
        public bool AllowNavigatePrevious
        {
            get { return _previous; }
            set
            {
                _previous = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ForegroundPrevious));
                OnPropertyChanged(nameof(ForegroundNext));
            }
        }
        public bool AllowNavigateNext
        {
            get { return _next; }
            set
            {
                _next = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ForegroundNext));
                OnPropertyChanged(nameof(ForegroundPrevious));
            }
        }

        public Brush AccentColor
        {
            get
            {
                return _accentColor;
            }
        }

        public Brush DisabledButtonColor
        {
            get
            {
                return _disabledColor;
            }
        }

        private string _editPath;
        public string EditPath
        {
            get { return _editPath; }
            set
            {
                _editPath = value;
                OnPropertyChanged();
                SearchSubFolder();
            }
        }

        private string[] _listEditPath;
        public string[] ComboBoxEditPath
        {
            get { return _listEditPath; }
            set
            {
                _listEditPath = value;
                OnPropertyChanged();
            }
        }

        public Brush ForegroundPrevious
        {
            get
            {
                if (AllowNavigatePrevious)
                    return _accentColor;
                else
                    return _disabledColor;
            }
        }

        public Brush ForegroundNext
        {
            get
            {
                if (AllowNavigateNext)
                    return _accentColor;
                else
                    return _disabledColor;
            }
        }

        private bool _showSuggestions;
        public bool ShowSuggestions
        {
            get { return _showSuggestions && ModeEdit; }
            set
            {
                _showSuggestions = value;
                OnPropertyChanged();
            }
        }

        private string _lastFolder;
        private void SearchSubFolder()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditPath) || EditPath == ((ShellObject)KnownFolders.Computer).Name)
                    return;
                string currentPath;
                if (EditPath.LastIndexOf(@"\") >= 0)
                    currentPath = EditPath.Substring(0, EditPath.LastIndexOf(@"\"));
                else
                    currentPath = EditPath;
                if (currentPath != _lastFolder)
                {
                    _lastFolder = currentPath;
                    Task.Run(() =>
                    {
                        ComboBoxEditPath = Directory.GetDirectories(currentPath);
                        ShowSuggestions = true;
                    });
                }
            }
            catch (Exception) { /* Ignoring errors */ }
        }
    }
}
