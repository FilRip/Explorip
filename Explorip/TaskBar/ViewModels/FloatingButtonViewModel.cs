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
    [ObservableProperty()]
    private double _newYPos;
    [ObservableProperty()]
    private ImageSource _floatingImage;

    public void SetParentTaskbar(Taskbar parentTaskbar)
    {
        ParentTaskbar = parentTaskbar;
        StretchMode = ConfigManager.GetTaskbarConfig(parentTaskbar.NumScreen).FloatingButtonStretchMode;
        double posY = ConfigManager.GetTaskbarConfig(parentTaskbar.NumScreen).FloatingButtonPosY;
        if (posY < 0)
            posY = parentTaskbar.Top;
        NewYPos = posY;
        MaxWidth = Math.Min(ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).FloatingButtonWidth, 52);
    }

    partial void OnNewYPosChanged(double value)
    {
        ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).FloatingButtonPosY = value;
    }

    [RelayCommand()]
    private void ExpandTaskbar()
    {
        ParentTaskbar.DataContext.ExpandCollapseTaskbar(true.ToString());
    }

    public void SetPos()
    {
        if (ConfigManager.GetTaskbarConfig(ParentTaskbar.NumScreen).FloatingButtonSide == System.Windows.HorizontalAlignment.Left)
        {
            ParentTaskbar.Left = ParentTaskbar.Screen.Bounds.Left;
            FloatingImage = Constants.Icons.ExpandButtonToRight;
        }
        else
        {
            ParentTaskbar.Left = ParentTaskbar.Screen.Bounds.Right - MaxWidth;
            FloatingImage = Constants.Icons.ExpandButtonToLeft;
        }
        ParentTaskbar.Top = NewYPos;
        ParentTaskbar.Width = MaxWidth;
    }
}
