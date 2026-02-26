using System;
using System.Windows.Controls;
using System.Windows.Media;

using ComputerInfo.Controls;

using ExploripPlugins;

using ManagedShell.AppBar;

namespace ComputerInfo;

public class ComputerInfoToolbar : IExploripToolbar
{
    private ComputerInfoControl _control;

    public string Author => "FilRip";

    public string Name => "ComputerInfo";

    public string Description => "Give some informations in real time";

    public Version Version => new(1, 0, 0, 0);

    public UserControl ExploripToolbar
    {
        get
        {
            _control ??= new ComputerInfoControl();
            return _control;
        }
    }

    public Guid GuidKey => new("{4A02DC97-4AF5-4BDA-88EA-C93A4FA96181}");

    public double MinHeight => 0;

    public double MinWidth => 0;

    public void SetGlobalColors(SolidColorBrush background, SolidColorBrush foreground, SolidColorBrush accent)
    {
        ((ComputerInfoControl)ExploripToolbar).DataContext.ChangeColor(background, foreground, accent);
    }

    public void UpdateTaskbar(int numScreen, double width, double height, Brush backgroundColor, AppBarEdge edge)
    {
        // Do nothing for this time
    }
}
