using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.Explorer.ViewModels;

public abstract partial class TabItemExploripViewModel : ObservableObject
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
