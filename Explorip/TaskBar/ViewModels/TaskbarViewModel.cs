using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Explorip.TaskBar.ViewModels;

public partial class TaskbarViewModel : ObservableObject
{
    private static TaskbarViewModel _instance;

    public static TaskbarViewModel Instance
    {
        get
        {
            _instance ??= new();
            return _instance;
        }
    }

    private TaskbarViewModel() : base()
    {
        ShowTabTip = Visibility.Hidden;
    }

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(LabelUnlock))]
    private bool _resizeOn;

    public string LabelUnlock
    {
        get
        {
            if (ResizeOn)
                return "Lock";
            else
                return "Unlock";
        }
    }

    [ObservableProperty()]
    private Visibility _showTabTip;
}
