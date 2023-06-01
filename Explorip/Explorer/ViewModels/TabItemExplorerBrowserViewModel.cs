using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Explorip.Explorer.ViewModels
{
    public class TabItemExplorerBrowserViewModel : TabItemExploripViewModel
    {
        public TabItemExplorerBrowserViewModel() : base()
        {
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
                    return Constants.Colors.AccentColorBrush;
                else
                    return DisabledButtonColor;
            }
        }

        public Brush ForegroundNext
        {
            get
            {
                if (AllowNavigateNext)
                    return Constants.Colors.AccentColorBrush;
                else
                    return DisabledButtonColor;
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
                if (string.IsNullOrWhiteSpace(EditPath))
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

        private bool _modeSearch;
        public bool ModeSearch
        {
            get { return _modeSearch; }
            set
            {
                _modeSearch = value;
                OnPropertyChanged();
            }
        }

        private string _whatToSearch;
        public string SearchTo
        {
            get { return _whatToSearch; }
            set
            {
                _whatToSearch = value;
                OnPropertyChanged();
            }
        }
    }
}
