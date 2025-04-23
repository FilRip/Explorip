using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

public class BaseToolbar : UserControl
{
    protected bool _isLoaded;

    public virtual BaseToolbarViewModel BaseDataContext
    {
        get { return (BaseToolbarViewModel)DataContext; }
    }
}
