using System.Windows.Media;

using Explorip.Helpers;

namespace Explorip.Explorer.ViewModels
{
    public abstract class TabItemExploripViewModel : ViewModelBase
    {
        private readonly SolidColorBrush _disabledColor;

        protected TabItemExploripViewModel() : base()
        {
            _disabledColor = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
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
