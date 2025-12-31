using System.Windows.Controls;

using VirtualDesktopPlugin.ViewModels;

namespace VirtualDesktopPlugin.Controls;

/// <summary>
/// Logique d'interaction pour VirtualDesktopControl.xaml
/// </summary>
public partial class VirtualDesktopControl : UserControl
{
    public VirtualDesktopControl()
    {
        InitializeComponent();
    }

    public VirtualDesktopControlViewModel MyDataContext
    {
        get { return (VirtualDesktopControlViewModel)DataContext; }
    }
}
