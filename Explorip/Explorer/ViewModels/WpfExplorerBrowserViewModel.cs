using Explorip.Helpers;

namespace Explorip.Explorer.ViewModels
{
    public class WpfExplorerBrowserViewModel : ViewModelBase
    {
        public WpfExplorerBrowserViewModel() : base()
        {
        }

        private bool _isMaximized;
        public bool WindowMaximized
        {
            get { return _isMaximized; }
            set
            {
                if (value != _isMaximized)
                {
                    _isMaximized = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _selectionLeft;
        public bool SelectionLeft
        {
            get { return _selectionLeft; }
            set
            {
                _selectionLeft = value;
                OnPropertyChanged();
            }
        }

        private bool _selectionRight;
        public bool SelectionRight
        {
            get { return _selectionRight; }
            set
            {
                _selectionRight = value;
                OnPropertyChanged();
            }
        }
    }
}
