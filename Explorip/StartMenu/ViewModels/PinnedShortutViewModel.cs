using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ManagedShell.ShellFolders;

namespace Explorip.StartMenu.ViewModels;

public partial class PinnedShortutViewModel(ShellFile sf, StartMenuViewModel window, bool panel2 = false) : ObservableObject
{
    private readonly ShellFile _shellFile = sf;
    private readonly StartMenuViewModel _window = window;
    private readonly bool _panel2 = panel2;
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

    public double IconSizeWidth
    {
        get
        {
            if (_panel2)
                return _window.IconSizeWidth2;
            else
                return _window.IconSizeWidth;
        }
    }

    public double IconSizeHeight
    {
        get
        {
            if (_panel2)
                return _window.IconSizeHeight2;
            else
                return _window.IconSizeHeight;
        }
    }
}
