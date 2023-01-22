using Explorip.Helpers;

using System.Windows.Media;

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
            }
        }
        public bool AllowNavigateNext
        {
            get { return _next; }
            set
            {
                _next = value;
                OnPropertyChanged();
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
    }
}
