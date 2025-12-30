using System.Collections.ObjectModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using VirtualDesktop;

namespace VirtualDesktopPlugin.ViewModels;

public partial class VirtualDesktopControlViewModel : ObservableObject
{
    [ObservableProperty()]
    private Brush _background, _foreground, _accentColor;
    [ObservableProperty()]
    private ObservableCollection<VirtualDesktop.Models.VirtualDesktop> _listDesktop;
    [ObservableProperty()]
    private VirtualDesktop.Models.VirtualDesktop _selectedDesktop;

    public VirtualDesktopControlViewModel() : base()
    {
        if (!VirtualDesktopManager.IsInitialized)
        {
            VirtualDesktopCompilerConfiguration config = new();
            config.ChangeDirectory("CoolBytes", "Explorip", "Assemblies");
            VirtualDesktopManager.Configure(config);
        }
        _listDesktop = [.. VirtualDesktopManager.GetDesktops()];
        _selectedDesktop = VirtualDesktopManager.Current;
        VirtualDesktopEvents.CurrentChanged += VirtualDesktopEvents_CurrentChanged;
    }

    partial void OnSelectedDesktopChanged(VirtualDesktop.Models.VirtualDesktop oldValue, VirtualDesktop.Models.VirtualDesktop newValue)
    {
        if (oldValue.Id != newValue.Id)
        {
            newValue.Switch();
        }
    }

    private void VirtualDesktopEvents_CurrentChanged(object sender, VirtualDesktop.Models.VirtualDesktopChangedEventArgs e)
    {
        SelectedDesktop = e.NewDesktop;
    }

    public void ChangeColor(SolidColorBrush background, SolidColorBrush foreground, SolidColorBrush accent)
    {
        Background = background;
        Foreground = foreground;
        AccentColor = accent;
    }

    [RelayCommand()]
    private void MoveToLeftDesktop()
    {
        SelectedDesktop.GetLeft()?.Switch();
    }

    [RelayCommand()]
    private void MoveToRightDesktop()
    {
        SelectedDesktop.GetRight()?.Switch();
    }
}
