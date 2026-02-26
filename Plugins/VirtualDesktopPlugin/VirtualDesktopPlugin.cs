using System;
using System.Windows.Controls;

using ExploripPlugins;

using ManagedShell.AppBar;

using VirtualDesktopPlugin.Controls;

namespace VirtualDesktopPlugin;

internal class VirtualDesktopPlugin : IExploripToolbar
{
    private UserControl _instance;

    public string Author => "FilRip";

    public string Name => "VirtualDesktopControl";

    public string Description => "Control to manage VirtualDesktop directly on taskbar";

    public Version Version => new("1.0.0.0");

    public UserControl ExploripToolbar
    {
        get
        {
            _instance ??= new VirtualDesktopControl();
            return _instance;
        }
    }

    public Guid GuidKey => new("{556FE767-BC54-4C53-8CFD-F2383B9BD165}");

    public double MinHeight => 0;

    public double MinWidth => 0;

    public void SetGlobalColors(System.Windows.Media.SolidColorBrush background, System.Windows.Media.SolidColorBrush foreground, System.Windows.Media.SolidColorBrush accent)
    {
        ((VirtualDesktopControl)ExploripToolbar).DataContext.ChangeColor(background, foreground, accent);
    }

    public void UpdateTaskbar(int numScreen, double width, double height, System.Windows.Media.Brush backgroundColor, AppBarEdge edge)
    {
        // Do nothing for this time
    }
}
