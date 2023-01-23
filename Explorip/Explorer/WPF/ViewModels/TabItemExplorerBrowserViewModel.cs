using Explorip.Helpers;

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Explorip.Explorer.WPF.ViewModels
{
    public class TabItemExplorerBrowserViewModel : ViewModelBase
    {
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
                OnPropertyChanged(nameof(ButtonPreviousImage));
            }
        }
        public bool AllowNavigateNext
        {
            get { return _next; }
            set
            {
                _next = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonNextImage));
            }
        }

        public ImageSource ButtonPreviousImage
        {
            get
            {
                if (AllowNavigatePrevious)
                    return Themes.AutoTheme.PreviousButtonEnabled;
                else
                    return Themes.AutoTheme.PreviousButtonDisabled;
            }
        }

        public ImageSource ButtonNextImage
        {
            get
            {
                if (AllowNavigateNext)
                    return Themes.AutoTheme.NextButtonEnabled;
                else
                    return Themes.AutoTheme.NextButtonDisabled;
            }
        }

        public Brush AccentColor
        {
            get
            {
                System.Drawing.Color myColor = WindowsSettings.GetWindowsAccentColor();
                Color mColor = Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);
                return new SolidColorBrush(mColor);
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
            }
        }

        private List<string> _listEditPath;
        public List<string> ComboBoxEditPath
        {
            get { return _listEditPath; }
            set
            {
                _listEditPath = value;
                OnPropertyChanged();
            }
        }
    }
}
