namespace ExploripCopy.ViewModels
{
    internal class MainViewModels : ViewModelBase
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
