using System;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.TaskBar.Controls;

using ExploripConfig.Configuration;

namespace Explorip.TaskBar.ViewModels;

public partial class FloatingButtonViewModel : ObservableObject
{
    public Taskbar ParentTaskbar { get; private set; }

    [ObservableProperty()]
    private Stretch _stretchMode;
    [ObservableProperty()]
    private double _maxWidth;

    public void SetParentTaskbar(Taskbar parentTaskbar)
    {
        ParentTaskbar = parentTaskbar;
        StretchMode = ConfigManager.GetTaskbarConfig(parentTaskbar.NumScreen).FloatingButtonStretchMode;
        MaxWidth = Math.Min(ConfigManager.GetTaskbarConfig(parentTaskbar.NumScreen).FloatingButtonWidth, parentTaskbar.StartButton.Width);
    }

    [RelayCommand()]
    private void ExpandTaskbar()
    {
        ParentTaskbar.MyDataContext.ExpandCollapseTaskbar(true.ToString());
    }
}
