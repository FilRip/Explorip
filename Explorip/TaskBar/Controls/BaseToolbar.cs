using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Explorip.TaskBar.ViewModels;

namespace Explorip.TaskBar.Controls;

public class BaseToolbar : UserControl
{
    protected bool _isLoaded;
    protected bool _ignoreReload = false;

    public BaseToolbar()
    {
        RegisterBaseEvents();
    }

    private void BaseToolbar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ignoreReload = true;
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            await Task.Delay(1000);
            _ignoreReload = false;
        });
    }

    protected void RegisterBaseEvents()
    {
        IsVisibleChanged -= BaseToolbar_IsVisibleChanged;
        IsVisibleChanged += BaseToolbar_IsVisibleChanged;
    }

    protected void UnregisterBaseEvents()
    {
        IsVisibleChanged -= BaseToolbar_IsVisibleChanged;
    }

    public virtual BaseToolbarViewModel BaseDataContext
    {
        get { return (BaseToolbarViewModel)DataContext; }
    }
}
