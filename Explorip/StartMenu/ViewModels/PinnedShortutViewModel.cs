using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.ShellFolders;

namespace Explorip.StartMenu.ViewModels;

public partial class PinnedShortutViewModel(ShellFile sf, StartMenuViewModel window) : ObservableObject
{
    private readonly ShellFile _shellFile = sf;
    private readonly StartMenuViewModel _window = window;
    private bool _mouseOver;

    [ObservableProperty()]
    private ImageSource _icon = sf.ExtraLargeIcon;
    [ObservableProperty()]
    private string _name = sf.DisplayName;
    [ObservableProperty()]
    private double _iconWidth = sf.ExtraLargeIcon.Width;
    [ObservableProperty()]
    private double _iconHeight = sf.ExtraLargeIcon.Height;

    [RelayCommand()]
    private void MouseUp(MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            if (ManagedShell.Common.Helpers.ShellHelper.StartProcess(_shellFile.Path, useShellExecute: true))
                _window.HideWindow();
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            // TODO : Context menu when right click
        }
    }

    [RelayCommand()]
    private void MouseEnter()
    {
        _mouseOver = true;
        OnPropertyChanged(nameof(BackgroundItem));
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        _mouseOver = false;
        OnPropertyChanged(nameof(BackgroundItem));
    }

    public Brush BackgroundItem
    {
        get
        {
            if (_mouseOver)
                return ExploripSharedCopy.Constants.Colors.ForegroundColorBrush;
            else
                return Brushes.Transparent;
        }
    }
}
