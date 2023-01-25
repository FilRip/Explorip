using Explorip.Helpers;

namespace Explorip.Explorer.WPF.ViewModels
{
    public class WpfExplorerBrowserViewModel : ViewModelBase
    {
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
    }
}
