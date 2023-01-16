using Explorip.Helpers;

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
    }
}
