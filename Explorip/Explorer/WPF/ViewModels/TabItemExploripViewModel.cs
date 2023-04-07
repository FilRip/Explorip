using System.Windows.Media;

using Explorip.Helpers;

namespace Explorip.Explorer.WPF.ViewModels
{
    public abstract class TabItemExploripViewModel : ViewModelBase
    {
        private readonly SolidColorBrush _accentColor, _disabledColor;

        protected TabItemExploripViewModel() : base()
        {
            System.Drawing.Color myColor = WindowsSettings.GetWindowsAccentColor();
            Color mColor = Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);
            _accentColor = new SolidColorBrush(mColor);
            _disabledColor = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
        }

        public virtual Brush AccentColor
        {
            get
            {
                return _accentColor;
            }
        }

        public virtual Brush DisabledButtonColor
        {
            get
            {
                return _disabledColor;
            }
        }
    }
}
