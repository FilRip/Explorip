using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
    [ObservableProperty()]
    private bool _showDropDown;

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

        // Update Virtual Desktop list
        VirtualDesktopEvents.Created += VirtualDesktopEvents_ListChanged;
        VirtualDesktopEvents.Destroyed += VirtualDesktopEvents_Destroyed;
        VirtualDesktopEvents.Renamed += VirtualDesktopEvents_Renamed;
        VirtualDesktopEvents.Moved += VirtualDesktopEvents_Moved;
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

    [RelayCommand()]
    private void DropDownListDesktop()
    {
        ShowDropDown = true;
    }

    public List<MenuItem> ListDesktopName
    {
        get
        {
            List<MenuItem> result = [];
            foreach (VirtualDesktop.Models.VirtualDesktop vd in ListDesktop)
            {
                MenuItem mi = new()
                {
                    Margin = new Thickness(-20, 0, 0, 0),
                    Header = vd.Name,
                    Tag = vd,
                };
                mi.Click += Mi_Click;
                result.Add(mi);
            }
            return result;
        }
    }

    private void Mi_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is VirtualDesktop.Models.VirtualDesktop vd)
        {
            ShowDropDown = false;
            vd.Switch();
        }
    }

    #region External update of Virtual Desktop

    private void RemakeListDesktop()
    {
        ListDesktop = [.. VirtualDesktopManager.GetDesktops()];
        OnPropertyChanged(nameof(ListDesktopName));
    }

    private void VirtualDesktopEvents_Moved(object sender, VirtualDesktop.Models.VirtualDesktopMovedEventArgs e)
    {
        RemakeListDesktop();
    }

    private void VirtualDesktopEvents_Renamed(object sender, VirtualDesktop.Models.VirtualDesktopRenamedEventArgs e)
    {
        RemakeListDesktop();
    }

    private void VirtualDesktopEvents_Destroyed(object sender, VirtualDesktop.Models.VirtualDesktopDestroyEventArgs e)
    {
        RemakeListDesktop();
    }

    private void VirtualDesktopEvents_ListChanged(object sender, VirtualDesktop.Models.VirtualDesktop e)
    {
        RemakeListDesktop();
    }

    #endregion
}
