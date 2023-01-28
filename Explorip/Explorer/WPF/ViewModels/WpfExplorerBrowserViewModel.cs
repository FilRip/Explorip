using Explorip.Helpers;

using System.Windows.Media;

namespace Explorip.Explorer.WPF.ViewModels
{
    public class WpfExplorerBrowserViewModel : ViewModelBase
    {
        private readonly SolidColorBrush _accentColor;

        public WpfExplorerBrowserViewModel()
        {
            System.Drawing.Color myColor = WindowsSettings.GetWindowsAccentColor();
            Color mColor = Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);
            _accentColor = new SolidColorBrush(mColor);
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

        public Brush AccentColor
        {
            get { return _accentColor; }
        }
    }
}
